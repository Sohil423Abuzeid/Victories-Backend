namespace InstaHub.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }

        public List<Message> Messages { get; set; } = new();

        public Category Category { get; set; }
        public List<int> AdminsId { get; set; } = new List<int>();

        public float DegreeOfSentiment { get; set; }

        public string SentimentAnalysis { get; set; }
        public int Rate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ClosedAt { get; set; }

        public string Label { get; set; }

        public int StateId { get; set; }
        public string State { get; set; }

        public string Summary { get; set; }

        public bool Urgent { get; set; }
    }
}
