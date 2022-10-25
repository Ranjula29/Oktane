using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Oktane.Model;

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



        public async Task CreateOntheWayQue(StationQue stationQue)
        {
            stationQue.OnTheWayDateTime = DateTime.Now.ToString();
            stationQue.Id = ObjectId.GenerateNewId().ToString();
            var filter = Builders<GasStation>.Filter.Eq(a =>
            a.Id, stationQue.StationId);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.OnTheWayQue, stationQue);
            await _gasStation.UpdateOneAsync(filter, multiUpdate);

        }


        public async Task<StationQue> CreateQue(string queueId)
        {

            GasStation gasStation = FilterStationByOnTheWayQueueId(queueId);
            StationQue queue = gasStation.OnTheWayQue[gasStation.OnTheWayQue.Count - 1];
            var StationObject = Builders<GasStation>.Filter.ElemMatch(t => t.OnTheWayQue,queue => queue.Id == queueId);
            var pull = Builders<GasStation>.Update.PullFilter(t => t.OnTheWayQue, queue => queue.Id == queueId);
            var result = await _gasStation.UpdateManyAsync(StationObject, pull);

            queue.ArrivalDateTime = DateTime.Now.ToString();
            queue.Id = ObjectId.GenerateNewId().ToString();

            var filter2 = Builders<GasStation>.Filter.Eq(a => a.Id, gasStation.Id);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.Que, queue);
            await _gasStation.UpdateOneAsync(filter2, multiUpdate);

            GasStation newStation = FilterStationByQueueId(queue.Id);

            return gasStation.Que[0];

        }

        public GasStation FilterStationByOnTheWayQueueId(string queueId)
        {
            var station = Builders<GasStation>.Filter
                .ElemMatch(t => t.OnTheWayQue,
                queue => queue.Id == queueId);

            var res = _gasStation.Find(station).ToList();

            return res[0];
        }

        public GasStation FilterStationByQueueId(string queueId)
        {
            var station = Builders<GasStation>.Filter
                .ElemMatch(t => t.Que,
                queue => queue.Id == queueId);

            var res = _gasStation.Find(station).ToList();

            return res[0];
        }


    }
}
