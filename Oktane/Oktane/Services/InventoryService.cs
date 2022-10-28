using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Oktane.Model;

namespace Oktane.Services
{
    public class InventoryService
    {

        private readonly IMongoCollection<GasStation> _inventory;

        //create database connection 
        public InventoryService(
        IOptions<DataBaseSetting> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _inventory = mongoDatabase.GetCollection<GasStation>(
                databaseSettings.Value.CollectionName);
        }



        public async void UpdateFuelAmountWhenQueueUpdated(string stationId, int fuelAmount, string type)
        {
            GasStation gasStation = new GasStation();
            var GasStationFilter = Builders<GasStation>.Filter.Eq(a => a.Id, stationId);


            if (type == "Diesel")
            {
                gasStation.TotalDiesel = (fuelAmount - 20);
                var updateDefinition = Builders<GasStation>.Update
                    .Set(u => u.TotalDiesel, gasStation.TotalDiesel);
                var updatedResult = await _inventory
                    .UpdateOneAsync(GasStationFilter, updateDefinition);
            }
            else
            {
                gasStation.TotalPetrol = (fuelAmount - 20);
                var updateDefinition = Builders<GasStation>.Update
                    .Set(u => u.TotalPetrol, gasStation.TotalPetrol);
                var updatedResult = await _inventory
                    .UpdateOneAsync(GasStationFilter, updateDefinition);
            }
        }


        public async Task<JsonResult> SaveFuel(Inventory inventory)
        {
            inventory.Id = ObjectId.GenerateNewId().ToString();
            var Station = Builders<GasStation>.Filter.Eq(a => a.Id, inventory.StationId);
            var Update = Builders<GasStation>.Update
                .Push(u => u.Inventory, inventory);
            var pushNotificationsResult = await _inventory.UpdateOneAsync(Station, Update);
            updateFuelAmount(inventory);
            var results = _inventory.Find(i => i.Id == inventory.StationId).ToList();

            return new JsonResult(results[0]);
        }



        public async void updateFuelAmount(Inventory inventory)
        {
            GasStation gasStation = new GasStation();
            var gasStationFilter = Builders<GasStation>.Filter.Eq(a => a.Id, inventory.StationId);
            var amount = await currentFuelAmount(inventory.StationId, inventory.FuleType);

            if (inventory.FuleType == "Petrol")
            {
                gasStation.TotalPetrol = amount + inventory.Stock;
                var update = Builders<GasStation>.Update.Set(u => u.TotalPetrol, gasStation.TotalPetrol);
                var updatedResult = await _inventory.UpdateOneAsync(gasStationFilter, update);
            }
            else
            {
                gasStation.TotalDiesel = amount + inventory.Stock;
                var update = Builders<GasStation>.Update.Set(u => u.TotalDiesel, gasStation.TotalDiesel);
                var updatedResult = await _inventory.UpdateOneAsync(gasStationFilter, update);

            }
        }

        public async Task<int> currentFuelAmount(string stationId, string type)
        {
            var res = await _inventory.FindAsync<GasStation>(c => c.Id == stationId);
            return (type == "Diesel") ? res.ToList()[0].TotalDiesel : res.ToList()[0].TotalPetrol;
        }





    }
}
