using InstaHub.Dto;
using InstaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
namespace InstaHub.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public MessageService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public Task<IEnumerable<Message>> GetMessageHistoryByTicketIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessagesByTicketIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageAsync(int ticketId, MessageDto messageDto)
        {
            // Facebook API URL and access token
            var apiUrl = "https://graph.facebook.com/v20.0/399411823263404/messages";
            var accessToken = "<your-access-token>";




            // customer ContactWay
            var ticket = await _context.Tickets
            .Include(t => t.CustomerId)  
            .FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket  not found.");
            }

            var customer = await _context.Customers
            .FirstOrDefaultAsync(t => t.Id == ticket.CustomerId);

            if (customer == null) { 
                
                throw new Exception("customer  not found.");
            }


            var payload = new
            {
                messaging_product = "whatsapp",
                to = customer.ContactWay,
                type = "template",
                template = new
                {
                    name = messageDto.Content,
                    language = new { code = "en_US" }
                }
            };

            // Prepare the HTTP request
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            
            var message = new Message
            {
                TicketId = ticketId,
                Content = messageDto.Content,
                Date = messageDto.Date
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

    }

}
