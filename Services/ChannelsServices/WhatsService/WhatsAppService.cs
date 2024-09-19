using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
namespace InstaHub.Services.ChannelsServices.WhatsService
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly WhatsAppSettings _settings;

        public WhatsAppService(IOptions<WhatsAppSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendMessage(SendMessageDto message)
        {
            using HttpClient httpClient = new();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Token);

            WhatsAppRequest body = new()
            {
                to = message.Mobile,
                template = new Template
                {
                    name = message.Template,
                    language = new Language { code = message.Language }
                }
            };

            if (message.Components is not null)
                body.template.components = message.Components;

            HttpResponseMessage response =
                await httpClient.PostAsJsonAsync(new Uri(_settings.ApiUrl), body);

            return response.IsSuccessStatusCode;
        }

        public Task<WhatsAppMessage> ReceiveMessage()
        {
            throw new NotImplementedException();
        }
    }
}
