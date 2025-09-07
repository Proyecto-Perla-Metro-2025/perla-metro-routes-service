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
        public async Task<ActionResult<ApiResponse<RouteDto>>> CreateRoute([FromBody] CreateRouteDto routeDto)
        {
            try
            {
                // 1. El servicio ahora nos devuelve el DTO de la ruta creada.
                var newRouteDto = await _routeService.CreateRouteAsync(routeDto);

                var response = new ApiResponse<RouteDto>(newRouteDto, "Route created successfully", true);

                // 2. Usamos CreatedAtAction para devolver un 201 Created.
                //    - nameof(GetRouteById): Es el nombre de la acción que puede ser usada para obtener este nuevo recurso.
                //    - new { id = newRouteDto.Id }: Son los parámetros de ruta para esa acción.
                //    - response: Es el cuerpo de la respuesta que enviaremos.
                return CreatedAtAction(nameof(GetRouteById), new { id = newRouteDto.Id }, response);
            }
            catch (ArgumentException ex)
            {
                // Manejamos la validación de negocio que añadimos en el servicio.
                return BadRequest(new ApiResponse<object?>(null, ex.Message, false));
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateRoute(string id, [FromBody] UpdateRouteDto routeDto)
        {
            try
            {
                // 2. Llama al servicio con el id y el DTO
                await _routeService.UpdateRouteAsync(id, routeDto);
                var response = new ApiResponse<object?>(null, "Route updated successfully", true);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object?>(null, ex.Message, false));
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object?>>> DeleteRoute(string id)
        {
            try
            {
                await _routeService.DeleteRouteAsync(id);
                var response = new ApiResponse<object?>(null, "Route deactivated successfully", true);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                // Si el servicio lanza la excepción, devolvemos un 404 Not Found.
                return NotFound(new ApiResponse<object?>(null, ex.Message, false));
            }
        }
    }
}