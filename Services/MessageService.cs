using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Repositories;
using InstaHub.Services.ChannelsServices.WhatsService;

namespace InstaHub.Services
{
    public class MessageService(IWhatsAppService _whatsAppService, IMessageRepository _messageRepository) : IMessageService
    {
        public Task<IEnumerable<Message>> GetMessageHistoryByTicketIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }


        // will use the azure server api 
        public Task<IEnumerable<Message>> GetMessagesByTicketIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageAsync(int ticketId, string messaging_product, SendMessageDto message)
        {
            // needs refactoring based on the message type: future work
            WhatsAppMessage retunedMessage = new();
            if (messaging_product == "whatsapp")
                 retunedMessage = await _whatsAppService.SendMessage(message);

           await _messageRepository.AddMessageAsync(ticketId, retunedMessage);
        }

      
        public async Task AddMessageToTicketAsync(int ticketId, WhatsAppMessage message)
        {
            // Save the message and link it to the ticket
            await _messageRepository.AddMessageAsync(ticketId, message);
        }

        public async Task StoreMessage(int ticketId, WhatsAppMessage message)
        {
            await _messageRepository.AddMessageAsync(ticketId, message);
        }
    }
}