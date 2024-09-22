using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InstaHub.Models
{
    public class Customer
    {
        [Key]
        public string CustomerId {get; set;}
        public DateTime FirstTicketDate { get; set; }
        public List<Ticket> Tickets {get; set;}
    }
}

