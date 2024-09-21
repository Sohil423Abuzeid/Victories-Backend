using InstaHub.Controllers;
using InstaHub.Models;

namespace InstaHub.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<Ticket> GetOpenTicketByCustomerIdAsync(string customerId);
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        Task<Ticket> UpdateTicketAsync(int ticketId, TicketDto ticketDto);
        Task<Ticket> UpdateTicketAsync(int ticketId, Ticket ticke);
        Task<bool> CloseTicketAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByCustomerIdAsync(string customerId);
    }
}
