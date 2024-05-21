using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Group
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
    }
}