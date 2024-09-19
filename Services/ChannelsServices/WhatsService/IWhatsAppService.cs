using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services.ChannelsServices.WhatsService
{
    public interface IWhatsAppService
    {
        public Task<WhatsAppMessage> SendMessage(SendMessageDto message);

        public Task<WhatsAppMessage> ReceiveMessage();
    }
}
