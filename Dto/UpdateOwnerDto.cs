using System.ComponentModel.DataAnnotations;

namespace InstaHub.Dto
{
    public class UpdateOwnerDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}