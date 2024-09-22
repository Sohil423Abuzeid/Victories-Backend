
using System.ComponentModel.DataAnnotations;

namespace InstaHub.Models
{
    public class Admin
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string HashPassword { get; set; }

        public DateTime RegisterationDate { get; set; } 

        public String Role { get; set; } = "admin";

        public int CountOfTickets { get; set; }

        // many to mnay relatioships 
        public List<TicketAdmin> TicketAdmins { get; set; } = new List<TicketAdmin>();

    }

    public class TicketAdmin
    {
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}


