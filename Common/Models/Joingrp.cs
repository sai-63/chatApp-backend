using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Joingrp
    {
        [BsonId]
        [BsonElement("username")]
        public required string username { get; set; }

        [BsonElement("groupname")]
        public required string groupname { get; set; }
    }
}