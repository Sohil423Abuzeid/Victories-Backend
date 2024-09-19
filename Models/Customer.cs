namespace InstaHub.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerId {get; set;}
        public string ContactWay {get; set;}

        public DateTime RegistrationDate { get; set; }
        public DateTime FirstTicketDate { get; set; }
        public List<Ticket> Tickets {get; set;}
    }
}

