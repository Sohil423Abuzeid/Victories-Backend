using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
   
        // This API will be hit by Meta, not by us
        [HttpPost("receive-whatsapp-messages")]
        public async Task<IActionResult> ReceiveWhatsAppMessages([FromBody] WhatsAppMessage message)
        {
            if (message == null || string.IsNullOrEmpty(message.MessageBody))
            {
                return BadRequest("Message content cannot be null or empty.");
            }

            try
            {
                // Check for an open ticket within 5 minutes
                var ticket = await _ticketService.GetOpenTicketByCustomerIdAsync(message.CustomerId);

                if (ticket == null || (DateTime.UtcNow - ticket.CreatedAt).TotalMinutes > 5)
                {
                    // Close the old ticket if necessary
                    if (ticket != null && (DateTime.UtcNow - ticket.CreatedAt).TotalMinutes > 5)
                    {
                        await _ticketService.CloseTicketAsync(ticket.Id);
                    }
                    // Save the message in the database 

                    // Create a new ticket
                    ticket = new Ticket
                    {
                        CustomerId = message.CustomerId,
                        CreatedAt = DateTime.UtcNow,
                        State = States.open.ToString(),
                        StateId = (int)States.open,
                    };
                    ticket.Messages.Add(message);
                    ticket = await _ticketService.CreateTicketAsync(ticket);
                    await _messageService.StoreMessage(ticket.Id,message);

                }

                // Associate the message with the ticket
                await _messageService.AddMessageToTicketAsync(ticket.Id, message);

                // Send to front-end 
                await _messageSocketHandler.SendMessageToAllAsync($"New message");
                await _messageSocketHandler.SendMessageToAllAsync($" with ticket id {message.TicketId} from {message.CustomerId}: {message.MessageBody}");
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing incoming message.");
                return StatusCode(500, "Internal server error");
            }
        }

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
    }
}
