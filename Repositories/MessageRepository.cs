using InstaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InstaHub.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(int ticketId, WhatsAppMessage message)
        {
            // Find the ticket associated with the message
            var ticket = await _context.Tickets
                .Include(t => t.Messages) // Include messages to track the state properly
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                throw new ArgumentException("Ticket not found.", nameof(ticketId));
            }

         
            // Add the new message to the ticket's message collection
            ticket.Messages.Add(message);
            _context.WhatsAppMessages.Add(message);
            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
