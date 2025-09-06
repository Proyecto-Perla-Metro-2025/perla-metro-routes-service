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

            await _routeService.CreateRouteAsync(route);

            var response = new Responses.ApiResponse<RouteEntity>(route, "Route created successfully", true);
            return Ok(response);
        }
    }
}