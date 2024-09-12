using InstaHub.Controllers;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface ITicketSercive
    {
        Task<IEnumerable<Ticket>> GetSimilarTicketsAsync(int ticketId, int v);
        Task<IEnumerable<Ticket>> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByCustomerIdAsync(int customerId);
        Task<Ticket> UpdateTicketAsync(int ticketId, TicketDto ticketDto);
    }
}