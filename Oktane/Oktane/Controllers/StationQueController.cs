using Microsoft.AspNetCore.Mvc;
using Oktane.Model;
using Oktane.Services;

namespace Oktane.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationQueController : ControllerBase
    {
        private readonly StationQueService _queueService;
        private readonly GasStationService _gasStationService;
        private readonly InventoryService _inventoryService;


        //service injection
        public StationQueController(StationQueService queueService, GasStationService gasStationService, InventoryService inventoryService)
        {
            _gasStationService = gasStationService;
            _queueService = queueService;
            _inventoryService = inventoryService;
        }


        //create on the way que API
        [HttpPost]
        [Route("/save/on-the-way-que")]
        public async Task<IActionResult> SaveQueOfONtheWay(StationQue stationQue)
        {
            await _queueService.CreateOntheWayQue(stationQue);

            return CreatedAtAction(nameof(_gasStationService.GetAsync), new { id = stationQue.Id }, stationQue);
        }

        //create  arrival Que API
        [HttpPost]
        [Route("/save/que")]
        public async Task<JsonResult> SaveQue(string queueId)
        {
            var res = await _queueService.CreateQue(queueId);
            return new JsonResult(res);
        }

        //create history Que API
        [HttpPost]
        [Route("/save/historyque")]
        public async Task<JsonResult> SaveHistoryQue(string queueId, string type, bool status)
        {
            GasStation res = await _queueService.SaveHistoryQue(queueId, type, status);
            var current = await _inventoryService.currentFuelAmount(res.Id, type);
            if (status)
            {
                _inventoryService.UpdateFuelAmountWhenQueueUpdated(res.Id, current, type);
            }

            var result = _gasStationService.GetAsync(res.Id);

            return new JsonResult(result);
        }
        //get Que status  API
        [HttpGet]
        [Route("/GetQueueStatus")]
        public async Task<JsonResult> GetQueueStatus(string stationId, string type)
        {
            var res = await _queueService.GetQueueStatus(stationId, type);
            return res;
        }

    }
}
