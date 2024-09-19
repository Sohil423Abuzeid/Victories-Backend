namespace InstaHub.Services.ChannelsServices.WhatsAppServiceInfra
{
    public class WhatsAppRequest: MessageRequest
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string to { get; set; }
        public string type { get; set; } = "template";
        public Template template { get; set; }
    }

    public class Template
    {
        public string name { get; set; }
        public Language language { get; set; }
        public List<WhatsAppComponent> components { get; set; }
    }

    public class Language
    {
        public string code { get; set; }
    }
}
