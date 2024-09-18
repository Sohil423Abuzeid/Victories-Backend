namespace InstaHub.Models
{
    public class Message
    {
        public string MessageId { get; set; }                   // Unique ID of the message

        public int TicketId { get; set; }

        public DateTime SendDate { get; set; }

        public DateTime ReceiveDate { get; set; }

        public string TimeStamp { get; set; }
    }

    public class WhatsAppMessage : Message
    {
        public string MessagingProduct { get; set; }            // Messaging product (e.g., WhatsApp)

        public string DisplayPhoneNumber { get; set; }          // Business display phone number

        public string ContactName { get; set; }                 // Contact name (sender)

        public string WaId { get; set; }                        // WhatsApp ID of the contact "reciver"

        public string MessageType { get; set; }                 // Type of the message (e.g., text)

        public string MessageBody { get; set; }                 // Content of the text message
    }
}