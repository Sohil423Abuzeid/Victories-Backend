using InstaHub.Models;

namespace InstaHub.Repositories
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(int ticketId, WhatsAppMessage message);
    }
}