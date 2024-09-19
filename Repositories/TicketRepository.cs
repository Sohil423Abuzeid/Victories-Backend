using InstaHub.Controllers;
using InstaHub.Models;

namespace InstaHub.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        public Task<bool> CloseTicket(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetOpenTicketByCustomerIdAsync(string customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetTicketsByCustomerIdAsync(string customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> UpdateTicketAsync(int ticketId, TicketDto ticketDto)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> UpdateTicketAsync(int ticketId, Ticket ticke)
        {
            throw new NotImplementedException();
        }
    }
}
