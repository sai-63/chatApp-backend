using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;

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

        [BsonElement("IsRead")]
        public bool IsRead { get; set; }

        [BsonElement("SenderRemoved")]
        public bool SenderRemoved { get; set; }
        [BsonElement("ReplyToMessageId")]
        public String ReplyToMessageId { get; set; }

        [BsonElement("FileName")]
        public string? FileName { get; set; } // File name

        [BsonElement("FileType")]
        public string? FileType { get; set; } // File type

        [BsonElement("FileContent")]
        public byte[]? FileContent { get; set; }

        [BsonElement("FileSize")]
        public long? FileSize { get; set; } // File size

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class Chatform
    {
        public String MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public String ReplyToMessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public IFormFile? File { get; set; }
    }

    public class Friend
    {
        public string userId { get; set; }
        public string friendId { get; set; }
    }

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

        [BsonElement("UserStatus")]
        public string UserStatus { get; set; }

        [BsonElement("LastSeen")]
        public DateTime LastSeen { get; set; }

        [BsonElement("Friends")]
        public List<string> Friends { get; set; }

        //[BsonElement("UnseenMessages")]
        //public Dictionary<string,int> UnseenMessages { get; set; }

    }

    public class Userupdateprofile
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

}
