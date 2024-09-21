using InstaHub.Models;

namespace InstaHub.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)

        {
            _context = context;
        }

        public async Task AddMessageAsync(int
 ticketId, WhatsAppMessage message)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
            {
                throw new ArgumentException("Ticket not found.", nameof(ticketId));
            }

            var newMessage = new Message
            {
                MessageId=message.MessageId,
                TicketId = ticketId,
                MessagingProduct = "whatsapp",
            };

            ticket.Messages.Add(newMessage);

            await _context.SaveChangesAsync();
            throw new ArgumentException("done", nameof(ticketId));
        }
    }
}