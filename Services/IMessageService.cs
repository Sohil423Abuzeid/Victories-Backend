using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessageHistoryByTicketIdAsync(int ticketId);
        Task<IEnumerable<Message>> GetMessagesByTicketIdAsync(int ticketId);
        Task SendMessageAsync(int ticketId, MessageDto message);
    }
}