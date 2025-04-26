using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbProvider.Entities
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("groupId")]
        public string GroupId { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; }

    }
}
