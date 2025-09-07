using Microsoft.AspNetCore.Mvc;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
using RoutesService.Src.Core.Interfaces;
using RoutesService.Src.Responses;
using RoutesService.Src.Application.Services;
using RoutesService.Src.Controllers.DTOs;
namespace RoutesService.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController(IRouteService routeService) : ControllerBase
    {
        private readonly IRouteService _routeService = routeService;
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RouteDto>>>> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            var response = new ApiResponse<IEnumerable<RouteDto>>(routes, "Routes retrieved successfully", true);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RouteDto>>> GetRouteById(string id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);

            // Si la ruta no se encuentra, devolvemos un error 404 Not Found.
            if (route == null)
            {
                return NotFound(new ApiResponse<RouteEntity?>(null, "Route not found", false));
            }

            // Si se encuentra, la devolvemos en una respuesta exitosa.
            var response = new ApiResponse<RouteDto>(route, "Route retrieved successfully", true);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto route)
        {

            await _routeService.CreateRouteAsync(route);

            var response = new ApiResponse<CreateRouteDto>(route, "Route created successfully", true);
            return Ok(response);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RouteEntity>>> UpdateRoute(string id, [FromBody] RouteEntity updatedRoute)
        {
            // Asignamos el ID de la URL al objeto para asegurar consistencia.
            updatedRoute.Id = id;

            try
            {
                await _routeService.UpdateRouteAsync(updatedRoute);
                var response = new ApiResponse<RouteEntity?>(null, "Route updated successfully", true);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                // Si el servicio lanza la excepción, devolvemos un 404 Not Found.
                return NotFound(new ApiResponse<RouteEntity?>(null, ex.Message, false));
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRoute(string id)
        {
            try
            {
                await _routeService.DeleteRouteAsync(id);
                var response = new ApiResponse<object>(null, "Route deactivated successfully", true);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                // Si el servicio lanza la excepción, devolvemos un 404 Not Found.
                return NotFound(new ApiResponse<object>(null, ex.Message, false));
            }
        }
    }
}