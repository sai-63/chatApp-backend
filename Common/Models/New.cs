﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Common.Models
{
    public class New
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; }

        //[BsonElement("ReceiverId")]
        //public string ReceiverId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }


    }
}
