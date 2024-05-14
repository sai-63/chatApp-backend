using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace login.Common.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Nickname")]
        public string Nickname { get; set; }

        [BsonElement("Friends")]
        public List<string> Friends { get; set; }

    }
}
