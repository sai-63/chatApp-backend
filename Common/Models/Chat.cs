using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace login.Common.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("SenderId")]
        public String SenderId { get; set; }

        [BsonElement("ReceiverId")]
        public String ReceiverId { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public long Timestamp { get; set; }
    }
}
