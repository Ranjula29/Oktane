using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Oktane.Model
{
    public class Vehicle{


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Type { get; set; } = String.Empty;
        public string Model { get; set; } = String.Empty;
        public string Manufacturer { get; set; } = String.Empty;
        public string NumberPlate { get; set; } = String.Empty;
        public string FuleType { get; set; } = String.Empty;


    }
}
