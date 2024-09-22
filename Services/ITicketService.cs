using Azure;
using InstaHub.Controllers;
using InstaHub.Dto;
using InstaHub.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace InstaHub.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetSimilarTicketsAsync(int ticketId, int count);
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByCustomerIdAsync(string customerId);

        Task<Ticket> UpdateTicketAsync(int ticketId, TicketDto ticketDto);

        Task<Ticket> GetOpenTicketByCustomerIdAsync(string customerId);

        Task<bool> CloseTicketAsync(int ticketId);

        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<Ticket> OpenTicketAsync(int ticketId, int adminId);

        Task<bool> MarkTicketAsUrgent(int ticketId);
        Task<bool> MarkTicketAsNotUrgent(int ticketId);

    }
}
