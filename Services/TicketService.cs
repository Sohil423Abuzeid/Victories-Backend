using InstaHub.Controllers;
using InstaHub.Models;
using InstaHub.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstaHub.Services
{
    public class TicketService(ITicketRepository _ticketRepository, IMessageService _messageService, ILogger<TicketService> _logger, HttpClient _client) : ITicketService
    {

        public async Task<Ticket> GetOpenTicketByCustomerIdAsync(string customerId)
        {
            // Fetch an open ticket for the customer from the repository
            return await _ticketRepository.GetOpenTicketByCustomerIdAsync(customerId);
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            // Create and return the new ticket
            return await _ticketRepository.CreateTicketAsync(ticket);
        }

        public async Task<Ticket> UpdateTicketAsync(int ticketId, TicketDto ticketDto)
        {
            // Update the ticket in the repository
            return await _ticketRepository.UpdateTicketAsync(ticketId, ticketDto);
        }

        public async Task<bool> CloseTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
            if (ticket != null)
            {
                return await _ticketRepository.CloseTicketAsync(ticketId);
            }
            return false;
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            // Fetch a single ticket by its ID from the repository
            return await _ticketRepository.GetTicketByIdAsync(ticketId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByCustomerIdAsync(string customerId)
        {
            // Fetch tickets associated with a customer
            return await _ticketRepository.GetTicketsByCustomerIdAsync(customerId);
        }

        public async Task<IEnumerable<Ticket>> GetSimilarTicketsAsync(int ticketId, int count)
        {
            // Fetch similar tickets, will be azur api 
            throw new NotImplementedException();
        }

        public async Task<Ticket> OpenTicketAsync(int ticketId, int adminId)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
           
            ticket.State = States.open.ToString();
            ticket.StateId = (int)States.open;
            ticket.AdminsId.Add(adminId);

            var updatedTicket = await _ticketRepository.UpdateTicketAsync(ticketId, ticket);
            return updatedTicket;
        }

        public async Task<bool> MarkTicketAsUrgent(int ticketId)
        {
            try
            {
                var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
                ticket.Urgent = true;
                var updateTicket = await _ticketRepository.UpdateTicketAsync(ticketId, ticket);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> MarkTicketAsNotUrgent(int ticketId)
        {
            try
            {
                var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
                ticket.Urgent = false;
                var updateTicket = await _ticketRepository.UpdateTicketAsync(ticketId, ticket);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
           await _ticketRepository.UpdateTicketAsync(ticket);
        }
    }
}
