using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
namespace InstaHub.Services.ChannelsServices.WhatsService
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly WhatsAppSettings _settings;

        public WhatsAppService(IOptions<WhatsAppSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<WhatsAppMessage> SendMessage(SendMessageDto message)
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

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error sending message");
            }

            // Directly parse JSON from response content
            using JsonDocument jsonObject = await response.Content.ReadFromJsonAsync<JsonDocument>();

            // Extract the necessary fields from the response
            var messageElement = jsonObject.RootElement.GetProperty("messages")[0];
            var contactElement = jsonObject.RootElement.GetProperty("contacts")[0];

            WhatsAppMessage result = new WhatsAppMessage
            {
                MessageId = messageElement.GetProperty("id").GetString(),
                CustomerId = contactElement.GetProperty("wa_id").GetString(),
                SendDate = DateTime.UtcNow,
                sent = true,
            };

            return result;
        }


    }
}
