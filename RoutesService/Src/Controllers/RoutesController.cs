using Microsoft.AspNetCore.Mvc;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
using RoutesService.Src.Core.Interfaces;
using RoutesService.Src.Responses;
using RoutesService.Src.Application.Services;
namespace RoutesService.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController(IRouteService routeService) : ControllerBase
    {
        private readonly IRouteService _routeService = routeService;
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RouteEntity>>>> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            var response = new ApiResponse<IEnumerable<RouteEntity>>(routes, "Routes retrieved successfully", true);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RouteEntity>>> GetRouteById(string id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);

            // Si la ruta no se encuentra, devolvemos un error 404 Not Found.
            if (route == null)
            {
                return NotFound(new ApiResponse<RouteEntity?>(null, "Route not found", false));
            }

            // Si se encuentra, la devolvemos en una respuesta exitosa.
            var response = new ApiResponse<RouteEntity>(route, "Route retrieved successfully", true);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] RouteEntity route)
        {

            await _routeService.CreateRouteAsync(route);

            var response = new ApiResponse<RouteEntity>(route, "Route created successfully", true);
            return Ok(response);
        }
    }
}