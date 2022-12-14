using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Oktane.Model;
using static MongoDB.Driver.WriteConcern;

namespace Oktane.Services
{
    public class StationQueService
    {
        private readonly IMongoCollection<GasStation> _gasStation;

        public StationQueService(
        IOptions<DataBaseSetting> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _gasStation = mongoDatabase.GetCollection<GasStation>(
                databaseSettings.Value.CollectionName);
        }



        public async Task CreateArrivalQue(StationQue stationQue)
        {
            stationQue.ArrivalDateTime = DateTime.Now.ToString();
            stationQue.Id = ObjectId.GenerateNewId().ToString();
            var filter = Builders<GasStation>.Filter.Eq(a =>
            a.Id, stationQue.StationId);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.Que, stationQue);
            await _gasStation.UpdateOneAsync(filter, multiUpdate);

        }


        public async Task<GasStation> SaveHistoryQue(string queueId, bool status)
        {
            GasStation gasStation = FilterStationByQueueId(queueId);
            StationQue? queue = gasStation.Que.Find(x => x.Id == queueId);

            var StationObject = Builders<GasStation>.Filter.ElemMatch(t => t.Que, queue => queue.Id == queueId);
            var pull = Builders<GasStation>.Update.PullFilter(t => t.Que, queue => queue.Id == queueId);
            var result = await _gasStation.UpdateManyAsync(StationObject, pull);

            if (status)
            {
                queue.DepartDateTime = DateTime.Now.ToString();
                queue.Id = ObjectId.GenerateNewId().ToString();

                var filter2 = Builders<GasStation>.Filter.Eq(a => a.Id, gasStation.Id);
                var multiUpdate = Builders<GasStation>.Update.Push(u => u.HistoryQue, queue);
                await _gasStation.UpdateOneAsync(filter2, multiUpdate);
            }


            return gasStation;
        }

        //public GasStation FilterStationByOnTheWayQueueId(string queueId)
        //{
        //    var station = Builders<GasStation>.Filter
        //        .ElemMatch(t => t.OnTheWayQue,
        //        queue => queue.Id == queueId);

        //    var res = _gasStation.Find(station).ToList();

        //    return res[0];
        //}

        public GasStation FilterStationByQueueId(string queueId)
        {
            var station = Builders<GasStation>.Filter
                .ElemMatch(t => t.Que,
                queue => queue.Id == queueId);

            var res = _gasStation.Find(station).ToList();

            return res[0];
        }

        public async void UpdateFuelAmountWhenQueueUpdated(string stationId,int fuelAmount, string type)
        {
            GasStation gasStation = new GasStation();
            var GasStationFilter = Builders<GasStation>.Filter.Eq(a => a.Id, stationId);


            if (type == "Diesel")
            {
                gasStation.TotalDiesel = (fuelAmount - 20);
                var updateDefinition = Builders<GasStation>.Update
                    .Set(u => u.TotalDiesel, gasStation.TotalDiesel);
                var updatedResult = await _gasStation
                    .UpdateOneAsync(GasStationFilter, updateDefinition);
            }
            else
            {
                gasStation.TotalPetrol = (fuelAmount - 20);
                var updateDefinition = Builders<GasStation>.Update
                    .Set(u => u.TotalPetrol, gasStation.TotalPetrol);
                var updatedResult = await _gasStation
                    .UpdateOneAsync(GasStationFilter, updateDefinition);
            }
        }


    }
}
