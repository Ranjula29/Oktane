using Microsoft.Extensions.Options;
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

            stationQue.Id = 

            stationQue.ArrivalDateTime = DateTime.Now.ToString();
            var filter2 = Builders<GasStation>.Filter.Eq(a =>
            a.Id, stationQue.StationId);
            var multiUpdate = Builders<GasStation>.Update.Push(u => u.Que, stationQue);
            await _gasStation.UpdateOneAsync(filter, multiUpdate);

        }



    }
}
