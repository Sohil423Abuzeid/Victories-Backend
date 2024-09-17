namespace InstaHub.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int TicketId {get; set;}

        public string Content {get; set;}

        public DateTime Date {get; set;}
    }
}