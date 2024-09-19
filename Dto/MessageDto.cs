using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;

namespace InstaHub.Dto
{
    public class SendMessageDto // for sending 
    {
        public string Mobile { get; set; }
        public string Language { get; set; }
        public string Template { get; set; }
        public List<WhatsAppComponent>? Components { get; set; }
    }
}