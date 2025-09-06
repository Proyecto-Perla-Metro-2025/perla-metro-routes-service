using Microsoft.AspNetCore.Mvc;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
using RoutesService.Src.Core.Interfaces;

namespace RoutesService.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController(IRouteService routeService) : ControllerBase
    {
        private readonly IRouteService _routeService = routeService;

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] RouteEntity route)
        {
            Console.WriteLine("Creating route...");
            await _routeService.CreateRouteAsync(route);
            Console.WriteLine($"Route created with ID: {route.Id}");
            return CreatedAtAction(nameof(CreateRoute), new { id = route.Id }, route);
        }
    }
}