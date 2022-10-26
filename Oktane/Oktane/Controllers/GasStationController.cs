using Microsoft.AspNetCore.Mvc;
using Oktane.Model;
using Oktane.Services;

namespace Oktane.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GasStationController : ControllerBase
    {
     
        private readonly GasStationService _gasStationService;
        private readonly StationQueService _queueService;

        //service class injection
        public GasStationController(GasStationService gasStationService, StationQueService queueService)
        {
            _gasStationService = gasStationService;
            _queueService = queueService;
        }


        //create fule station API

        [HttpPost]
        public async Task<IActionResult> Post(GasStation gasStation)
        {
            await _gasStationService.CreateAsync(gasStation);

            return CreatedAtAction(nameof(Get), new { id = gasStation.Id }, gasStation);
        }
        //get all fule station API
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
        //delete fule station API
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
        //update fule station API
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, GasStation gasStation)
        {
            var gas = await _gasStationService.GetAsync(id);

            if (gas is null)
            {
                return NotFound();
            }

            gasStation.Id = gas.Id;

            await _gasStationService.UpdateAsync(id, gasStation);

            return NoContent();
        }
        //get fule station By owner API
        [HttpGet]
        [Route("/GetStationByUserId")]
        public async Task<JsonResult> GetStationByUserId(string userId)
        {
            var res = await _gasStationService.GetStationByUserId(userId);
            return res;
        }

       

       

    }

   
}
