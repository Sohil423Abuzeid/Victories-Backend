using InstaHub.Controllers;
using InstaHub.Models;
using Microsoft.EntityFrameworkCore;
namespace InstaHub.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CloseTicket(int ticketId)
        {
            var ticket =await GetTicketByIdAsync(ticketId);

            
            if (ticket == null)
            {
                 return await Task.FromResult(false);
            }

            ticket.State = States.closed.ToString();
            ticket.ClosedAt = DateTime.UtcNow;

            _context.SaveChangesAsync();
            return await  Task.FromResult(false);
        }

        public Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            ticket.CreatedAt = DateTime.UtcNow;
            _context.Tickets.AddAsync(ticket);
            _context.SaveChangesAsync();
            return Task.FromResult(ticket);
        }

        public Task<Ticket> GetOpenTicketByCustomerIdAsync(string customerId)
        {
            var ticketTask = _context.Tickets
            .FirstOrDefaultAsync(t => t.CustomerId == customerId && t.State==States.open.ToString());
            return ticketTask;
        }

        public Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            var ticket = _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByCustomerIdAsync(string customerId)
        {
            return await _context.Tickets
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        }

        public async Task<Ticket> UpdateTicketAsync(int ticketId, TicketDto ticketDto)
        {
            Ticket ticket =await GetTicketByIdAsync(ticketId);
            if (ticket == null)
            {
                return null; 
            }
            ticket.State = ticketDto.Status;
            await _context.SaveChangesAsync();
            return await Task.FromResult<Ticket>(ticket);

        }

        public async Task<Ticket> UpdateTicketAsync(int ticketId, Ticket ticket)
        {
           Ticket ticket1 = await GetTicketByIdAsync(ticketId);
           ticket1 = ticket;
           await _context.SaveChangesAsync();
           return await Task.FromResult<Ticket>(ticket1);

        }
    }
}
