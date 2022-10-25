using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Oktane.Model
{
    public class StationQue
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("Id")]
        public string? Id { get; set; }
        public string VehicleType { get; set; } = String.Empty;
        public string? StationId { get; set; }
        public string UserId { get; set; } = String.Empty;
        public string FuleType { get; set; } = String.Empty;
        public string OnTheWayDateTime { get; set; } = String.Empty;
        public string ArrivalDateTime { get; set; } = String.Empty;
        public string DepartDateTime { get; set; } = String.Empty;
        public bool IsDepartAfterPump { get; set; } = false;
    }
}
