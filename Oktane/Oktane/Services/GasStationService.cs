using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Oktane.Model;
using Oktane.Response;
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


        public async Task<JsonResult> SaveFuel(Inventory inventory)
        {
            inventory.StockUpdateDateTime = DateTime.Now.ToString();
            inventory.Id = ObjectId.GenerateNewId().ToString();
            var Station = Builders<GasStation>.Filter.Eq(a => a.Id, inventory.StationId);
            var Update = Builders<GasStation>.Update
                .Push(u => u.Inventory, inventory);
            var pushNotificationsResult = await _gasStation.UpdateOneAsync(Station, Update);
            updateFuelAmount(inventory);
            var results = _gasStation.Find(i => i.Id == inventory.StationId).ToList();

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
                var updatedResult = await _gasStation.UpdateOneAsync(gasStationFilter, update);
            }
            else
            {
                gasStation.TotalDiesel = amount + inventory.Stock;
                var update = Builders<GasStation>.Update.Set(u => u.TotalDiesel, gasStation.TotalDiesel);
                var updatedResult = await _gasStation.UpdateOneAsync(gasStationFilter, update);

            }
        }

        public async Task<int> currentFuelAmount(string stationId, string type)
        {
            var res = await _gasStation.FindAsync<GasStation>(c => c.Id == stationId);
            return (type == "Diesel") ? res.ToList()[0].TotalDiesel : res.ToList()[0].TotalPetrol;
        }


        public async Task<FuleDetails> GetQueueDetails(string stationId, string type,string vehicleType)
        {

            List<StationQue> stationQues = new List<StationQue>();
            FuleDetails fuleDetails = new FuleDetails();
            var res = await GetAsync(stationId);
            if (type == "Diesel") {
                fuleDetails.Quantity  = res.TotalDiesel;
                foreach(StationQue que in res.Que){
                    if(que.VehicleType == vehicleType)
                    {
                        stationQues.Add(que);
                    }
                }
                fuleDetails.Quecount = stationQues.Count;
                fuleDetails.VehicaleCount = (fuleDetails.Quantity - (fuleDetails.Quecount * 20)) / 20;
                if(fuleDetails.VehicaleCount < 0)
                {
                    fuleDetails.VehicaleCount = 0;
                }

                return fuleDetails;

            } else {
                foreach (StationQue que in res.Que)
                {
                    if (que.VehicleType == vehicleType)
                    {
                        stationQues.Add(que);
                    }
                }
                fuleDetails.Quantity = res.TotalPetrol;
                fuleDetails.Quecount = stationQues.Count;
                fuleDetails.VehicaleCount = (fuleDetails.Quantity - (fuleDetails.Quecount * 20)) / 20;

                return fuleDetails;
            }
        }

        public async Task<JsonResult> GetStationByUserId(string userId)
        {
            var station = await _gasStation.FindAsync(c => c.OwnerId == userId);
            return new JsonResult(station.ToList()[0]);
        }

        internal async Task ChangeStatus(string stationId, bool status,GasStation gasStation)
        {
                gasStation.IsOpen = status;
                await _gasStation.ReplaceOneAsync(x => x.Id == stationId, gasStation);
            
        }
    }
}
