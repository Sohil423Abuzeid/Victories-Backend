﻿using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface IMessageService
    {
        Task AddMessageToTicketAsync(int ticketId, WhatsAppMessage message);
        Task<IEnumerable<Message>> GetMessageHistoryByTicketIdAsync(int ticketId);
        Task<IEnumerable<Message>> GetMessagesByTicketIdAsync(int ticketId);
        Task<bool> SendMessageAsync(int ticketId, string messaging_product , SendMessageDto message);

        Task<bool> StoreMessage(int ticketId, WhatsAppMessage message); 
    }
}