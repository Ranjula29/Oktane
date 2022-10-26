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


        public StationQueController(StationQueService queueService, GasStationService gasStationService, InventoryService inventoryService)
        {
            _gasStationService = gasStationService;
            _queueService = queueService;
            _inventoryService = inventoryService;
        }



        [HttpPost]
        [Route("/save/que")]
        public async Task<JsonResult> SaveQue(string queueId)
        {
            var res = await _queueService.CreateQue(queueId);
            return new JsonResult(res);
        }

        [HttpPost]
        [Route("/save/historyque")]
        public async Task<JsonResult> SaveHistoryQue(string queueId, string type, bool status)
        {
            var res = await _queueService.SaveHistoryQue(queueId, type, status);
            var current = await _inventoryService.currentFuelAmount(res.Id, type);
            if (status)
            {
                _inventoryService.UpdateFuelAmountWhenQueueUpdated(res.Id, current, type);
            }
            return new JsonResult(res.Que[0]);
        }


        [HttpPost]
        [Route("/save/on-the-way-que")]
        public async Task<IActionResult> SaveQueOfONtheWay(StationQue stationQue)
        {
            await _gasStationService.CreateOntheWayQue(stationQue);

            return CreatedAtAction(nameof(_gasStationService.GetAsync), new { id = stationQue.Id }, stationQue);
        }


    }
}
