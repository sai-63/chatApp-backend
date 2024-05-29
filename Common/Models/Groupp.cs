﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace login.Common.Models
{
    public class Grp
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("users")]
        public required List<string> Users { get; set; }

        [BsonElement("messages")]
        public required List<Grpmsg> Messages { get; set; }

        [BsonElement("pic")]
        public required string PicUrl { get; set; }
    }
    public class Grpmsg
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("SenderId")]
        public required string SenderId { get; set; }

        [BsonElement("Message")]
        public required string Message { get; set; }

        [BsonElement("FileName")]
        public string? FileName { get; set; } // File name

        [BsonElement("FileType")]
        public string? FileType { get; set; } // File type

        [BsonElement("FileContent")]
        public byte[]? FileContent { get; set; }

        [BsonElement("FileSize")]
        public long? FileSize { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class Groupform
    {
        public string SenderId { get; set; }
        public string Message { get; set; }        
        public IFormFile? File { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Joingrp
    {
        [BsonId]
        [BsonElement("username")]
        public string username { get; set; }

        [BsonElement("groupname")]
        public string groupname { get; set; }
    }

    public class Getid
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("users")]
        public List<string> Users { get; set; }

        [BsonElement("messages")]
        public List<New> Messages { get; set; }
    }

    public class New
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }


    }

}
