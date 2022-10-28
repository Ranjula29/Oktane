using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Oktane.Model;
using Oktane.Response;
using static MongoDB.Driver.WriteConcern;

namespace Oktane.Services
{
    public class StationQueService
    {
        private readonly IMongoCollection<GasStation> _stationQue;
        private readonly GasStationService _gasStationService;
        private readonly InventoryService _inventoryService;

        public StationQueService(
        IOptions<DataBaseSetting> databaseSettings, GasStationService gasStationService, InventoryService inventoryService)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _stationQue = mongoDatabase.GetCollection<GasStation>(
                databaseSettings.Value.CollectionName);

            _gasStationService = gasStationService;

             _inventoryService = inventoryService;
        }

        public async Task CreateArrivalQue(StationQue stationQue)
        {
            stationQue.ArrivalDateTime = DateTime.Now.ToString();
            stationQue.Id = ObjectId.GenerateNewId().ToString();
            var filter = Builders<GasStation>.Filter.Eq(a =>
            a.Id, stationQue.StationId);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.Que, stationQue);
            await _stationQue.UpdateOneAsync(filter, multiUpdate);

        }
 
        
        public async Task<GasStation> SaveHistoryQue(string queueId, string type, bool status)
        {
            GasStation gasStation = _gasStationService.FilterStationByQueueId(queueId);
            StationQue? queue = gasStation.Que.Find(x => x.Id == queueId);
            var StationObject = Builders<GasStation>.Filter.ElemMatch(t => t.Que, queue => queue.Id == queueId);
            var pull = Builders<GasStation>.Update.PullFilter(t => t.Que, queue => queue.Id == queueId);
            var result = await _stationQue.UpdateManyAsync(StationObject, pull);

            if (status)
            {
                queue.ArrivalDateTime = DateTime.Now.ToString();
                queue.Id = ObjectId.GenerateNewId().ToString();

                var filter2 = Builders<GasStation>.Filter.Eq(a => a.Id, gasStation.Id);
                var multiUpdate = Builders<GasStation>.Update.Push(u => u.HistoryQue, queue);
                await _stationQue.UpdateOneAsync(filter2, multiUpdate);
            }


            return gasStation;
        }

        public GasStation FilterStationByOnTheWayQueueId(string queueId)
        {
            var station = Builders<GasStation>.Filter
                .ElemMatch(t => t.Que,
                queue => queue.Id == queueId);

            var res = _stationQue.Find(station).ToList();

            return res[0];
        }


        public async Task<JsonResult> GetQueueStatus(string stationId, string type)
        {
            var res = await _gasStationService.GetAsync(stationId);
            int[] results = new int[2];
            if (type == "Diesel")
            {

                var numberOfVehicles = res.TotalDiesel / 20;
                var quantity = res.TotalDiesel;
                results[0] = numberOfVehicles;
                results[1] = quantity;

                return new JsonResult(results);

            }
            else
            {
                var numberOfVehicles = res.TotalPetrol / 20;
                var quantity = res.TotalPetrol;
                results[0] = numberOfVehicles;
                results[1] = quantity;

                return new JsonResult(numberOfVehicles);
            }
        }


        public async Task<FuleDetails> GetQueueDetails(string stationId, string type, string vehicleType)
        {

            List<StationQue> stationQues = new List<StationQue>();
            FuleDetails fuleDetails = new FuleDetails();
            var res = await _gasStationService.GetAsync(stationId);
            if (type == "Diesel")
            {
                fuleDetails.Quantity = res.TotalDiesel;
                foreach (StationQue que in res.Que)
                {
                    if (que.VehicleType == vehicleType)
                    {
                        stationQues.Add(que);
                    }
                }
                fuleDetails.Quecount = stationQues.Count;
                fuleDetails.VehicaleCount = (fuleDetails.Quantity - (fuleDetails.Quecount * 20)) / 20;
                if (fuleDetails.VehicaleCount < 0)
                {
                    fuleDetails.VehicaleCount = 0;
                }

                return fuleDetails;

            }
            else
            {
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

    }
}
