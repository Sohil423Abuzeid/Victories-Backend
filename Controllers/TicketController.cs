using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController(ITicketSercive _ticketService, IMessageService _messageService, ILogger<TicketController> _logger) : ControllerBase
    {




        public async Task<IActionResult> GetTickets(int ticketId)
        {
            var tickets = await _ticketService.GetTicketByIdAsync(ticketId);
            return Ok(tickets);
        }

        [HttpPost("{ticketId}/message")]
        public async Task<IActionResult> SendMessage(int ticketId, [FromBody] MessageDto message)
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
                await _messageService.SendMessageAsync(ticketId, message);

                return Ok(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message.");

                return StatusCode(500, "An error occurred while sending the message.");
            }
        }

        [HttpGet("{ticketId}/messages")]
        public async Task<IActionResult> ReceiveMessages(int ticketId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Invalid ticket ID.");
            }

            try
            {
                var messages = await _messageService.GetMessagesByTicketIdAsync(ticketId);

                if (messages == null || !messages.Any())
                {
                    return NotFound("No messages found for the specified ticket ID.");
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages.");

                return StatusCode(500, "An error occurred while retrieving the messages.");
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
                // Assuming you have a service to handle ticket updates
                var updatedTicket = await _ticketService.UpdateTicketAsync(ticketId, ticketDto);

                if (updatedTicket == null)
                {
                    return NotFound("Ticket not found.");
                }

                return Ok(updatedTicket);
            }
            catch (Exception ex)
            {
                // Log the exception
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
                // Assuming you have a service to handle retrieving message history
                var messageHistory = await _messageService.GetMessageHistoryByTicketIdAsync(ticketId);

                if (messageHistory == null || !messageHistory.Any())
                {
                    return NotFound("No message history found for the specified ticket ID.");
                }

                return Ok(messageHistory);
            }
            catch (Exception ex)
            {
                // Log the exception
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
                // Assuming you have a service to handle finding similar tickets
                var similarTickets = await _ticketService.GetSimilarTicketsAsync(ticketId, 5);

                if (similarTickets == null || !similarTickets.Any())
                {
                    return NotFound("No similar tickets found for the specified ticket ID.");
                }

                return Ok(similarTickets);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving similar tickets.");

                return StatusCode(500, "An error occurred while retrieving similar tickets.");
            }
        }

        [HttpGet("{customerId}/tickets")]
        public async Task<IActionResult> GetTicketsByCustomerId(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            try
            {
                // Assuming you have a service to handle retrieving tickets for a customer
                var tickets = await _ticketService.GetTicketsByCustomerIdAsync(customerId);

                if (tickets == null || !tickets.Any())
                {
                    return NotFound("No tickets found for the specified customer ID.");
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving tickets by customer ID.");

                return StatusCode(500, "An error occurred while retrieving the tickets.");
            }
        }


    }
}
