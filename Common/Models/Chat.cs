using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace login.Common.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("MessageId")]
        public String MessageId { get; set; }

        [BsonElement("SenderId")]
        public String SenderId { get; set; }

        [BsonElement("ReceiverId")]
        public String ReceiverId { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
