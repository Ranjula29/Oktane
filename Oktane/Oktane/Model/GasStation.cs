using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Oktane.Model
{
    public class GasStation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("Id")]
        public string? Id { get; set; }
        [JsonPropertyName("OwnerId")]
        public string OwnerId { get; set; } = String.Empty;
        [JsonPropertyName("Name")]
        public string Name { get; set; } = String.Empty;
        [JsonPropertyName("LicenceNumber")]
        public string LicenceNumber { get; set; } = String.Empty;
        [JsonPropertyName("Address")]
        public string Address { get; set; } = String.Empty;
        [JsonPropertyName("City")]
        public string City { get; set; } = String.Empty;
        [JsonPropertyName("IsAir")]
        public bool IsAir { get; set; } = true;
        [JsonPropertyName("IsOpen")]
        public bool IsOpen { get; set; } = true;
        [JsonPropertyName("OpenDateTime")]
        public string OpenDateTime { get; set; } = String.Empty;
        [JsonPropertyName("CloseDateTime")]
        public string CloseDateTime { get; set; } = String.Empty;
        [JsonPropertyName("Inventory")]
        public List<Inventory> Inventory { get; set; } = new List<Inventory>();
        [JsonPropertyName("OnTheWayQue")]
        public List<StationQue> OnTheWayQue { get; set; } = new List<StationQue>();
        [JsonPropertyName("Que")]
        public List<StationQue> Que { get; set; } = new List<StationQue>();
        [JsonPropertyName("HistoryQue")]
        public List<StationQue> HistoryQue { get; set; } = new List<StationQue>();
        [JsonPropertyName("TotalPetrol")]
        public int TotalPetrol { get; set; } = 0;
        [JsonPropertyName("TotalDiesel")]
        public int TotalDiesel { get; set; } = 0;


    }
}
