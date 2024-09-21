using Newtonsoft.Json;

namespace InstaHub.Models
{
    public class Message
    {
        public string MessageId { get; set; }                   // Unique ID of the message
        public int TicketId { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string TimeStamp { get; set; }
        public string MessagingProduct { get; set; }            // Messaging product (e.g., WhatsApp)

        public bool sent { get; set; } = false;
    }

    public class WhatsAppMessage : Message
    {
     //   [JsonProperty("value.metadata.display_phone_number")]
        public string DisplayPhoneNumber { get; set; }          // Business display phone number

       // [JsonProperty("value.contacts[0].profile.name")]
        public string ContactName { get; set; }                 // Contact name (sender)

      //  [JsonProperty("value.contacts[0].wa_id")]
        public string CustomerId { get; set; }                  // WhatsApp ID of the sender // customer

     //   [JsonProperty("value.messages[0].type")]
        public string MessageType { get; set; }                 // Type of the message (e.g., text)

      //  [JsonProperty("value.messages[0].text.body")]
        public string MessageBody { get; set; }                 // Content of the text message
    }
}
