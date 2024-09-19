using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface IMessageService
    {
        Task AddMessageToTicketAsync(int ticketId, WhatsAppMessage message);
        Task<IEnumerable<Message>> GetMessageHistoryByTicketIdAsync(int ticketId);
        Task<IEnumerable<Message>> GetMessagesByTicketIdAsync(int ticketId);
        Task SendMessageAsync(int ticketId, string messaging_product , SendMessageDto message);
        Task StoreMessage(int ticketId, WhatsAppMessage message);


    }
}