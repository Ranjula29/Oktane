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


        public GasStationController(GasStationService gasStationService) =>
        _gasStationService = gasStationService;

        [HttpPost]
        public async Task<IActionResult> Post(GasStation gasStation)
        {
            await _gasStationService.CreateAsync(gasStation);

            return CreatedAtAction(nameof(Get), new { id = gasStation.Id }, gasStation);
        }

        [HttpGet]
        public async Task<List<GasStation>> Get() =>
        await _gasStationService.GetAsync();

        [HttpGet("{id:length(24)}")]
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
        public async Task<IActionResult> SaveQue(StationQue stationQue)
        {
            await _gasStationService.CreateQue(stationQue);

            return CreatedAtAction(nameof(Get), new { id = stationQue.Id }, stationQue);
        }

    }

   
}
