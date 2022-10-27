using Microsoft.AspNetCore.Mvc;
using Oktane.Model;
using Oktane.Response;
using Oktane.Services;

namespace Oktane.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GasStationController : ControllerBase
    {
        private readonly GasStationService _gasStationService;
        private readonly StationQueService _queueService;


        public GasStationController(GasStationService gasStationService, StationQueService queueService)
        {
            _gasStationService = gasStationService;
            _queueService = queueService;
        }


        [HttpPost]
        public async Task<IActionResult> Post(GasStation gasStation)
        {
            await _gasStationService.CreateAsync(gasStation);

            return CreatedAtAction(nameof(Get), new { id = gasStation.Id }, gasStation);
        }

        [HttpGet]
        public async Task<List<GasStation>> Get() =>
        await _gasStationService.GetAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<GasStation>> Get(string id)
        {
            var gas = await _gasStationService.GetAsync(id);

            if (gas is null)
            {
                return NotFound();
            }

            return gas;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var gas = await _gasStationService.GetAsync(id);

            if (gas is null)
            {
                return NotFound();
            }
            await _gasStationService.RemoveAsync(id);
            return NoContent();
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, GasStation gasStation)
        {
            var gas = await _gasStationService.GetAsync(id);

            if (gas is null)
            {
                return NotFound();
            }

            gasStation.Id = gas.Id;

            await _gasStationService.UpdateAsync(id, gas);

            return NoContent();
        }

        [HttpPost]
        [Route("/save/on-the-way-que")]
        public async Task<IActionResult> SaveQueOfONtheWay(StationQue stationQue)
         {
            await _gasStationService.CreateOntheWayQue(stationQue);

            return CreatedAtAction(nameof(Get), new { id = stationQue.Id }, stationQue);
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
            GasStation res = await _queueService.SaveHistoryQue(queueId, type, status);
            var current = await _gasStationService.currentFuelAmount(res.Id, type);
            if (status)
            {
                _queueService.UpdateFuelAmountWhenQueueUpdated(res.Id, current, type);
            }

            var result = _gasStationService.GetAsync(res.Id);

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("/save/fuel")]
        public async Task<JsonResult> SaveFuel(Inventory inventory)
        {
            var res = await _gasStationService.SaveFuel(inventory);
            return new JsonResult(res);
        }

        [HttpGet]
        [Route("/GetQueueStatus")]
        public async Task<FuleDetails> GetQueueStatus(string stationId, string type)
        {
            var res = await _gasStationService.GetQueueDetails(stationId, type);
            return res;
        }

        [HttpGet]
        [Route("/GetStationByUserId")]
        public async Task<JsonResult> GetStationByUserId(string userId)
        {
            var res = await _gasStationService.GetStationByUserId(userId);
            return res;
        }

        [HttpGet]
        [Route("/fule-Details")]
        public async Task<FuleDetails> Getq(string stationId, string type)
        {
            var res = await _gasStationService.GetQueueDetails(stationId, type);
            return res;
        }

    }

   
}
