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
            WhatsAppMessage retunedMessage = null;

            try
            {
                if (messaging_product == "whatsapp")
                    retunedMessage = await _whatsAppService.SendMessage(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending message", ex);
            }


            // Save the message if it was successfully sent
            if (retunedMessage != null)
            {
                try
                {
                    await _messageRepository.AddMessageAsync(ticketId, retunedMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("Message sent successfully, but storing message failed", ex);
                }
            }
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