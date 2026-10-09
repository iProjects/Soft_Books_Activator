/*
 * Created by: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 01/23/2020
 * Time: 02:54
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace console_dto
{
    public class task_dto
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Browsable(true)]
        public ObjectId mongodb_id { get; set; }
        [Browsable(true)]
        public Int64 id { get; set; }
        [Browsable(true)]
        public string name { get; set; }
        [Browsable(true)]
        public string description { get; set; }
        [Browsable(true)]
        public string status { get; set; }
        [Browsable(true)] 
        public string expected_completion_date { get; set; }
        [Browsable(true)] 
        public string actual_completion_date { get; set; }
        [Browsable(true)]
        public string created_date { get; set; }
    }
}
