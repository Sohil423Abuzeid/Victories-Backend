using InstaHub.Controllers;
using InstaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaHub.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CloseTicketAsync(int ticketId) // Updated method name to match the pattern in the interface
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null)
            {
                return false;
            }

            ticket.State = States.completed.ToString();
            ticket.StateId = (int) States.completed;
            ticket.Urgent = false;
            ticket.ClosedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            ticket.CreatedAt = DateTime.UtcNow;
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> GetOpenTicketByCustomerIdAsync(string customerId)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.CustomerId == customerId && t.State == States.open.ToString());
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
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
            var ticket = await GetTicketByIdAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            ticket.State = ticketDto.Status;
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> UpdateTicketAsync(int ticketId, Ticket ticket)
        {
            var existingTicket = await GetTicketByIdAsync(ticketId);
            if (existingTicket == null)
            {
                return null;
            }

            // should use AutoMapper, if still there is time, will refactor 
            existingTicket.State = ticket.State;
            existingTicket.Category = ticket.Category;
            existingTicket.Label = ticket.Label;
            existingTicket.Messages = ticket.Messages;
            existingTicket.Rate = ticket.Rate;
            existingTicket.SentimentAnalysis = ticket.SentimentAnalysis;
            existingTicket.State = ticket.State;
            existingTicket.StateId = ticket.StateId;
            existingTicket.Summary = ticket.Summary;
            existingTicket.Urgent = ticket.Urgent;

            await _context.SaveChangesAsync();
            return existingTicket;
        }
    }
}
