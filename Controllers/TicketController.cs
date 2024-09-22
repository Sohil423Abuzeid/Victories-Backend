using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IMessageService _messageService;
        private readonly ILogger<TicketController> _logger;
        private readonly MessageSocketHandler _messageSocketHandler;
        private readonly CategoryService _categoryService;

        // Constructor for dependency injection
        public TicketController(ITicketService ticketService, IMessageService messageService, ILogger<TicketController> logger, MessageSocketHandler messageSocketHandler)
        {
            _ticketService = ticketService;
            _messageService = messageService;
            _logger = logger;
            _messageSocketHandler = messageSocketHandler;
        }

       
        [HttpPost("{ticketId}/{messaging_product}/sendMessage")]
        public async Task<IActionResult> SendMessage(int ticketId, string messaging_product, [FromBody] SendMessageDto message)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }

            if (message == null)
            {
                return BadRequest("Message content cannot be null.");
            }

            try
            {
                await _messageService.SendMessageAsync(ticketId, messaging_product, message);
                return Ok(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex) when (ex.Message.Contains("sending message"))
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { success = false, message = "Message sending failed." });
            }
            catch (Exception ex) when (ex.Message.Contains("storing message"))
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new { success = true, message = "Message sent successfully but couldn't be stored in db." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }

        // In-memory store for messages grouped by customer and timestamp
        private static readonly Dictionary<string, (List<WhatsAppMessage> Messages, DateTime FirstReceived)> _messageBuffer = new();

        // Lock object to prevent concurrent access to the message buffer
        private static readonly object _lock = new();

        [HttpPost("receive-whatsapp-messages")]
        public async Task<IActionResult> ReceiveWhatsAppMessages([FromBody] WhatsAppMessage message)
        {
            if (message == null || string.IsNullOrEmpty(message.MessageBody))
            {
                return BadRequest("Message content cannot be null or empty.");
            }

            try
            {
                bool isFirstMessage = false;

                lock (_lock)
                {
                    // Check if there are existing messages for this customer
                    if (_messageBuffer.TryGetValue(message.CustomerId, out var entry))
                    {
                        // Add the new message to the existing list
                        entry.Messages.Add(message);
                        _messageBuffer[message.CustomerId] = entry;
                    }
                    else
                    {
                        // If this is the first message, create a new entry in the buffer
                        _messageBuffer[message.CustomerId] = (new List<WhatsAppMessage> { message }, DateTime.UtcNow);
                        isFirstMessage = true; // This is the first message from this customer
                    }
                }

                // If this is the first message, start a background task to create the ticket after 5 minutes
                if (isFirstMessage)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        await CreateTicketForCustomerAsync(message.CustomerId);
                    });
                }

                // Notify front-end 
                await _messageSocketHandler.SendMessageToAllAsync("New message received");
                await _messageSocketHandler.SendMessageToAllAsync($"Message received from {message.CustomerId}: {message.MessageBody}");

                return Ok("Message received");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing incoming message.");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task CreateTicketForCustomerAsync(string customerId)
        {
            List<WhatsAppMessage> messages;
            DateTime firstReceived;

            // Safely access and remove messages for the given customer
            lock (_lock)
            {
                if (!_messageBuffer.TryGetValue(customerId, out var entry))
                {
                    return; // No messages found for this customer
                }

                messages = entry.Messages;
                firstReceived = entry.FirstReceived;

                // Remove the entry from the buffer
                _messageBuffer.Remove(customerId);
            }

            // Concatenate all messages into a single string
            var allMessages = string.Join("\n", messages.Select(m => m.MessageBody));

            // Create a new ticket with all the messages received in the 5-minute window
            var ticket = new Ticket
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow, // Set to the current time for ticket creation
                State = States.open.ToString(),
                StateId = (int)States.open,
                Messages = messages, // Assign all messages to the ticket
            };

            try
            {
                // Save the ticket in the database
                ticket = await _ticketService.CreateTicketAsync(ticket);

                // Encode the messages string to be URL-safe
                var encodedMessages = Uri.EscapeDataString(allMessages);

                // Construct URLs for the `summary`, `label`, and `sentiment` endpoints
                var summaryUrl = $"https://instahub-docker-hub-gwdpdpdje3c8daen.germanywestcentral-01.azurewebsites.net/summary?chat={encodedMessages}";
                var labelUrl = $"https://instahub-docker-hub-gwdpdpdje3c8daen.germanywestcentral-01.azurewebsites.net/label?chat={encodedMessages}";
                var sentimentUrl = $"https://instahub-docker-hub-gwdpdpdje3c8daen.germanywestcentral-01.azurewebsites.net/sentiment?chat={encodedMessages}";

                // Get categories from the service
                var categories =await _categoryService.GetCategories();
                var classList = categories.Select(c => c.Name).ToList(); // Assuming 'Name' is the property for category name

                // Create the classification payload
                var classificationPayload = new
                {
                    chat = allMessages,
                    classes = classList
                };

                // Serialize the classification payload to JSON
                var classificationJson = JsonSerializer.Serialize(classificationPayload);
                var classificationUrl = "https://instahub-docker-hub-gwdpdpdje3c8daen.germanywestcentral-01.azurewebsites.net/classification";

                using (var httpClient = new HttpClient())
                {
                    // Make the requests to get summary, label, sentiment, and classification concurrently
                    var summaryResponseTask = httpClient.GetAsync(summaryUrl);
                    var labelResponseTask = httpClient.GetAsync(labelUrl);
                    var sentimentResponseTask = httpClient.GetAsync(sentimentUrl);
                    var classificationResponseTask = httpClient.PostAsync(classificationUrl, new StringContent(classificationJson, Encoding.UTF8, "application/json"));

                    // Await all responses
                    var summaryResponse = await summaryResponseTask;
                    var labelResponse = await labelResponseTask;
                    var sentimentResponse = await sentimentResponseTask;
                    var classificationResponse = await classificationResponseTask;

                    // Check and read the summary response
                    if (summaryResponse.IsSuccessStatusCode)
                    {
                        var summary = await summaryResponse.Content.ReadAsStringAsync();
                        ticket.Summary = summary;
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to retrieve summary for ticket ID {ticket.Id}. Status code: {summaryResponse.StatusCode}");
                    }

                    // Check and read the label response
                    if (labelResponse.IsSuccessStatusCode)
                    {
                        var label = await labelResponse.Content.ReadAsStringAsync();
                        ticket.Label = label;
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to retrieve label for ticket ID {ticket.Id}. Status code: {labelResponse.StatusCode}");
                    }

                    // Check and read the sentiment response
                    if (sentimentResponse.IsSuccessStatusCode)
                    {
                        var sentimentJson = await sentimentResponse.Content.ReadAsStringAsync();
                        var sentiment = JsonSerializer.Deserialize<SentimentResponse>(sentimentJson);
                        if (sentiment != null)
                        {
                            ticket.SentimentAnalysis = sentiment.Result;
                            ticket.DegreeOfSentiment = sentiment.Degree;
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to retrieve sentiment analysis for ticket ID {ticket.Id}. Status code: {sentimentResponse.StatusCode}");
                    }

                    // Check classification response
                    if (classificationResponse.IsSuccessStatusCode)
                    {
                        var classificationJson = await classificationResponse.Content.ReadAsStringAsync();
                        // Process classification response if needed
                        // Example: Use classificationJson to update ticket or log
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to classify messages for ticket ID {ticket.Id}. Status code: {classificationResponse.StatusCode}");
                    }
                }

                // Now update the ticket in the database with both summary and label
                await _ticketService.UpdateTicketAsync(ticket);

                // Notify front-end that a new ticket has been created
                await _messageSocketHandler.SendMessageToAllAsync($"Ticket created with ID {ticket.Id} for customer {customerId}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating ticket for customer {customerId}");
            }
        }


        //add endpoint to make in progress
        [HttpPut("{ticketId}/{adminId}/openTicket")]
        public async Task<IActionResult> OpenTicket(int ticketId, int adminId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }

            try
            {
                var updatedTicket = await _ticketService.OpenTicketAsync(ticketId, adminId);
                if (updatedTicket == null)
                {
                    return NotFound("Ticket not found.");
                }

                return Ok(updatedTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket.");
                return StatusCode(500, "An error occurred while updating the ticket.");
            }
        }
        
        [HttpPut("{ticketId}/{adminId}/closeTicket")]
        public async Task<IActionResult> CloseTicket(int ticketId, int adminId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }
            try
            {
                var closedTicket = await _ticketService.CloseTicketAsync(ticketId);
                if (closedTicket == false)
                {
                    return NotFound("Ticket not found.");
                }

                return Ok(closedTicket);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error closing ticket.");
                return StatusCode(500, "An error occurred while closing the ticket.");
            }
        }

        [HttpPut("{ticketId}/update")]
        public async Task<IActionResult> UpdateTicketById(int ticketId, [FromBody] TicketDto ticketDto)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }

            if (ticketDto == null)
            {
                return BadRequest("Ticket data cannot be null.");
            }

            try
            {
                var updatedTicket = await _ticketService.UpdateTicketAsync(ticketId, ticketDto);
                if (updatedTicket == null)
                {
                    return NotFound("Ticket not found.");
                }

                return Ok(updatedTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket.");
                return StatusCode(500, "An error occurred while updating the ticket.");
            }
        }

        [HttpGet("{ticketId}/history")]
        public async Task<IActionResult> GetMessageHistory(int ticketId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }

            try
            {
                var messageHistory = await _messageService.GetMessageHistoryByTicketIdAsync(ticketId);
                if (messageHistory == null || !messageHistory.Any())
                {
                    return NotFound("No message history found for the specified ticket ID.");
                }

                return Ok(messageHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving message history.");
                return StatusCode(500, "An error occurred while retrieving the message history.");
            }
        }

        [HttpGet("{ticketId}/similar")]
        public async Task<IActionResult> GetSimilarTickets(int ticketId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }

            try
            {
                var similarTickets = await _ticketService.GetSimilarTicketsAsync(ticketId, 5);
                if (similarTickets == null || !similarTickets.Any())
                {
                    return NotFound("No similar tickets found for the specified ticket ID.");
                }

                return Ok(similarTickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving similar tickets.");
                return StatusCode(500, "An error occurred while retrieving similar tickets.");
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetTicketsByCustomerId(string customerId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByCustomerIdAsync(customerId);
                if (tickets == null || !tickets.Any())
                {
                    return NotFound("No tickets found for the specified customer ID.");
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tickets by customer ID.");
                return StatusCode(500, "An error occurred while retrieving the tickets.");
            }
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketsByTicketId(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                {
                    return NotFound("No tickets found for the specified customer ID.");
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tickets by customer ID.");
                return StatusCode(500, "An error occurred while retrieving the tickets.");
            }
        }
       
        [HttpGet("Get-all-States")]
        public async Task<IActionResult> GetStates()
        {
            try
            {
                var enums = Enum.GetValues(typeof(States)) 
                       .Cast<States>()            
                       .Select(e => new
                       {
                           Id = Convert.ToInt32(e),  
                           Name = e.ToString()       
                       })
                       .ToList();

                return Ok(enums);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving states.");
                return StatusCode(500, "An error occurred while retrieving the states.");
            }
        }
      
        [HttpPost("reverse-Urgent")]
        public async Task<IActionResult> ReverseTicketUrgent(int TicketId)
        {
            try
            {
               Ticket ticket =await _ticketService.GetTicketByIdAsync(TicketId);

                if(ticket.Urgent)
                    await _ticketService.MarkTicketAsNotUrgent(TicketId);
                else
                    await _ticketService.MarkTicketAsUrgent(TicketId);

               return Ok(new {message= "Ticket urgent chnaged."});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error change urgent.");
                return StatusCode(500,new { message = "An error occurred while change urgent." });
            }
        }
    }
}
