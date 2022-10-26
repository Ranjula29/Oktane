using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Oktane.Model;
using System.Net;

namespace Oktane.Services
{
    public class GasStationService
    {

        private readonly IMongoCollection<GasStation> _gasStation;

        public GasStationService(
        IOptions<DataBaseSetting> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _gasStation = mongoDatabase.GetCollection<GasStation>(
                databaseSettings.Value.CollectionName);
        }

        public async Task CreateAsync(GasStation gasStation) =>
        await _gasStation.InsertOneAsync(gasStation);

        public async Task<List<GasStation>> GetAsync() =>
        await _gasStation.Find(_ => true).ToListAsync();

        public async Task<GasStation?> GetAsync(string id) =>
            await _gasStation.Find(x => x.Id == id).FirstOrDefaultAsync();


        public async Task UpdateAsync(string id, GasStation gasStation) =>
       await _gasStation.ReplaceOneAsync(x => x.Id == id, gasStation);

        public async Task RemoveAsync(string id) =>
            await _gasStation.DeleteOneAsync(x => x.Id == id);

         public async Task CreateOntheWayQue(StationQue stationQue)
        {
            stationQue.OnTheWayDateTime = DateTime.Now.ToString();
            stationQue.Id = ObjectId.GenerateNewId().ToString();
            var filter = Builders<GasStation>.Filter.Eq(a =>
            a.Id, stationQue.StationId);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.OnTheWayQue, stationQue);
            await _gasStation.UpdateOneAsync(filter, multiUpdate);

        }


        public async Task CreateQue(StationQue stationQue)
        {

            //Task<GasStation> gas =  _gasStation.Find(x => x.Id == stationQue.UserId).FirstOrDefaultAsync();

            var filter = Builders<GasStation>.Filter.Where(g => g.Id == stationQue.StationId);
            var update = Builders<GasStation>.Update.PullFilter(ym => ym.OnTheWayQue, Builders<StationQue>.Filter.Where(nm => nm.UserId == stationQue.UserId));
            await _gasStation.UpdateOneAsync(filter, update);


            stationQue.ArrivalDateTime = DateTime.Now.ToString();
            var filter2 = Builders<GasStation>.Filter.Eq(a =>
            a.Id, stationQue.StationId);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.Que, stationQue);
            await _gasStation.UpdateOneAsync(filter, multiUpdate);

        }

        //public async Task<JsonResult> SaveFuel(Inventory inventory)
        //{
        //    inventory.Id = ObjectId.GenerateNewId().ToString();
        //    var Station = Builders<GasStation>.Filter.Eq(a => a.Id, inventory.StationId);
        //    var Update = Builders<GasStation>.Update
        //        .Push(u => u.Inventory, inventory);
        //    var pushNotificationsResult = await _gasStation.UpdateOneAsync(Station, Update);
        //    updateFuelAmount(inventory);
        //    var results = _gasStation.Find(i => i.Id == inventory.StationId).ToList();

        //    return new JsonResult(results[0]);
        //}


        //public async void updateFuelAmount(Inventory inventory)
        //{
        //    GasStation gasStation = new GasStation();
        //    var gasStationFilter = Builders<GasStation>.Filter.Eq(a => a.Id, inventory.StationId);
        //    var amount = await currentFuelAmount(inventory.StationId, inventory.FuleType);

        //    if (inventory.FuleType == "Petrol")
        //    {
        //        gasStation.TotalDiesel = amount + inventory.Stock;
        //        var update = Builders<GasStation>.Update.Set(u => u.TotalPetrol, gasStation.TotalPetrol);
        //        var updatedResult = await _gasStation.UpdateOneAsync(gasStationFilter, update);
        //    }
        //    else
        //    {
        //        gasStation.TotalDiesel = amount + inventory.Stock;
        //        var update = Builders<GasStation>.Update.Set(u => u.TotalDiesel, gasStation.TotalDiesel);
        //        var updatedResult = await _gasStation.UpdateOneAsync(gasStationFilter, update);

        //    }
        //}

        //public async Task<int> currentFuelAmount(string stationId, string type)
        //{
        //    var res = await _gasStation.FindAsync<GasStation>(c => c.Id == stationId);
        //    return (type == "Diesel") ? res.ToList()[0].TotalDiesel : res.ToList()[0].TotalPetrol;
        //}


        public async Task<JsonResult> GetQueueStatus(string stationId, string type)
        {
            var res = await GetAsync(stationId);
            int[] results = new int[2];
            if (type == "Diesel") {

                var numberOfVehicles = res.TotalDiesel/20;
                var quantity = res.TotalDiesel;
                results[0] = numberOfVehicles;
                results[1] = quantity;

                return new JsonResult(results);

            } else {
                var numberOfVehicles = res.TotalPetrol/20;
                var quantity = res.TotalPetrol;
                results[0] = numberOfVehicles;
                results[1] = quantity;

                return new JsonResult(numberOfVehicles);
            }
        }

        public async Task<JsonResult> GetStationByUserId(string userId)
        {
            var station = await _gasStation.FindAsync(c => c.OwnerId == userId);
            return new JsonResult(station.ToList()[0]);
        }

    }
}
