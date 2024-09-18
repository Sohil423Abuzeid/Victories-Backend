namespace InstaHub.Models 
{
    public class Ticket
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        public List<Message> Messages {get; set;}

        public Category Category {get; set;}

        public List<int> AdminId {get; set;}

        public float SentimentAnalysis {get; set;}
        public int Rate {get; set;}
        
        public DateTime CreateAt {get; set;}
        public DateTime ClosedAt {get; set;}


        public string Label {get; set;}

        public int StateId {get; set;}
        public string State { get; set;}

        public string Summary { get; set; }

        public bool Urgent { get; set; }
    }
}
  