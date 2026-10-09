using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; 
using System.Linq;
using System.Web;

namespace console_dto
{
    public class log_dto
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Browsable(true)]
        public ObjectId mongodb_id { get; set; }
        public long id { get; set; }
        public string message { get; set; } 
        public string timestamp { get; set; } 
        public string tag { get; set; }
        public string status { get; set; } 
        public string created_date { get; set; }
    }
}
