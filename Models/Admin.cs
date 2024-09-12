
using System.ComponentModel.DataAnnotations;

namespace InstaHub.Models
{
    public class Admin
    {
        public int Id {get; set;}

        public string UserName { get; set; }

        public string FirstName {get; set;}
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string HashPassword {get; set;}


        public DateTime RegisterationDate { get; set; }
    }
}


