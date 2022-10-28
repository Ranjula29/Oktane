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

        //create database connection 
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


        public GasStation FilterStationByQueueId(string queueId)
        {
            var station = Builders<GasStation>.Filter
                .ElemMatch(t => t.Que,
                queue => queue.Id == queueId);

            var res = _gasStation.Find(station).ToList();

            return res[0];
        }


        public async Task<JsonResult> GetStationByUserId(string userId)
        {
            var station = await _gasStation.FindAsync(c => c.OwnerId == userId);
            return new JsonResult(station.ToList()[0]);
        }


        internal async Task ChangeStatus(string stationId, bool status, GasStation gasStation)
        {
            gasStation.IsOpen = status;
            await _gasStation.ReplaceOneAsync(x => x.Id == stationId, gasStation);

        }


    }


}
