namespace InstaHub.Models 
{
    public class Ticket
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }

        public List<Message> Messages { get; set; } = new();

        public Category Category {get; set;}

        public List<int> AdminId { get; set; } = new List<int>();

        public float SentimentAnalysis {get; set;}
        public int Rate {get; set;}
        
        public DateTime CreatedAt {get; set;}
        public DateTime ClosedAt {get; set;}

        public string Label {get; set;}

        public int StateId { get; set; } = (int) States.notStarted;
        public string State { get; set; } = States.notStarted.ToString();

        public string Summary { get; set; }

        public bool Urgent { get; set; }
    }
}
  