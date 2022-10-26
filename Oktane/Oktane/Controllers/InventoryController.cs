using Microsoft.AspNetCore.Mvc;
using Oktane.Model;
using Oktane.Services;

namespace Oktane.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        //service injection
        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        //save Fule stock API
        [HttpPost]
        [Route("/save/fuel")]
        public async Task<JsonResult> SaveFuel(Inventory inventory)
        {
            var res = await _inventoryService.SaveFuel(inventory);
            return new JsonResult(res);
        }
    }
}
