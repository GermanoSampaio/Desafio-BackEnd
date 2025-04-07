using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MotoService.Domain.Entities
{
    public class RentalPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("days")]
        public int Days { get; set; }

        [BsonElement("daily_rate")]
        public double DailyRate { get; set; }

        public RentalPlan(int days, double dailyRate)
        {
            Days = days;
            DailyRate = dailyRate;
        }
    }
}