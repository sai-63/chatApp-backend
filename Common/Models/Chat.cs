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
        public string MessageId { get; set; }

        [BsonElement("SenderId")]
        public string SenderId { get; set; }

        [BsonElement("ReceiverId")]
        public string ReceiverId { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
