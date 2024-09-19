using InstaHub.Models;

namespace InstaHub.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        public Task AddMessageAsync(int ticketId, WhatsAppMessage message)
        {
            throw new NotImplementedException();
        }
    }
}