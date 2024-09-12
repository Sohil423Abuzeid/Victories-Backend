namespace InstaHub.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int TicketId {get; set;}

        public string Contnet {get; set;}

        public DateTime Date {get; set;}
    }
}