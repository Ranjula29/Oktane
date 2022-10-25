using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Oktane.Model
{
    public class Inventory
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string? FuleType { get; set; }
        public string? StationId { get; set; }
        public int Stock { get; set; } = 0;
        public double Price { get; set; } = 0.0;
        public bool IsStockOut { get; set; } = false;
        public string StockUpdateDateTime  { get; set; } = String.Empty;

    }
}
