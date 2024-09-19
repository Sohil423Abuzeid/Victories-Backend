
namespace InstaHub.Controllers
{
    public class TicketDto
    {
        public string Status { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public int TicketId { get; internal set; }
        public int CustomerId { get; internal set; }
    }
}