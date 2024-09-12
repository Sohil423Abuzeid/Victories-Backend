using System.ComponentModel.DataAnnotations;

namespace InstaHub.Models
{
    public class Company
    {
        public string CompanyName { get; set; }
        public int CompanySize { get; set; }

        public string CompanyIndustry { get; set; }

        public bool HaveCustomSupportBefore { get; set; }

        public int NumberOfCustomers { get; set; }

        public DateTime LunchTime { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
