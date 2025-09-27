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
        /// <summary>
        /// Obtiene todas las rutas, con opción de filtrar por estado (activas/inactivas).
        /// </summary>
        /// <param name="isActive">Filtro opcional para obtener solo rutas activas o inactivas.</param>
        /// <returns>Una lista de rutas.</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RouteDto>>>> GetAllRoutes([FromQuery] bool? isActive)
        {
            var routes = await _routeService.GetAllRoutesAsync(isActive);
            var response = new ApiResponse<IEnumerable<RouteDto>>(routes, "Routes retrieved successfully", true);
            return Ok(response);
        }
        /// <summary>
        /// Obtiene una ruta específica por su ID.
        /// </summary>
        /// <param name="id">El ID único de la ruta.</param>
        /// <returns>Los detalles de la ruta encontrada o un error 404 si no se encuentra.</returns>
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
        /// <summary>
        /// Crea una nueva ruta en el sistema.
        /// </summary>
        /// <param name="routeDto">Los datos para la nueva ruta.</param>
        /// <returns>La ruta recién creada con su ID asignado.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RouteDto>>> CreateRoute([FromBody] CreateRouteDto routeDto)
        {
            try
            {
                var newRouteDto = await routeService.CreateRouteAsync(routeDto);
                var response = new ApiResponse<RouteDto>(newRouteDto, "Route created successfully", true);
                return CreatedAtAction(nameof(GetRouteById), new { id = newRouteDto.Id }, response);
            }
            catch (ArgumentException ex) // Para errores de validación (ej. fechas)
            {
                return BadRequest(new ApiResponse<object>(null, ex.Message, false));
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(new ApiResponse<object>(null, ex.Message, false));
            }
        }
        /// <summary>
        /// Actualiza una ruta existente.
        /// </summary>
        /// <param name="id">El ID de la ruta a actualizar.</param>
        /// <param name="routeDto">Los nuevos datos para la ruta.</param>
        /// <returns>Una respuesta de éxito o un error si la ruta no se encuentra.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateRoute(string id, [FromBody] UpdateRouteDto routeDto)
        {
            try
            {
                await _routeService.UpdateRouteAsync(id, routeDto);
                var response = new ApiResponse<object?>(null, "Route updated successfully", true);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object?>(null, ex.Message, false));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object?>(null, ex.Message, false));
            }
        }
        /// <summary>
        /// Desactiva una ruta (Soft Delete).
        /// </summary>
        /// <param name="id">El ID de la ruta a desactivar.</param>
        /// <returns>Una respuesta de éxito o un error si la ruta no se encuentra.</returns>
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