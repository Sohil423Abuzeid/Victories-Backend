using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace InstaHub.Dto
{
    public class AdminDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}