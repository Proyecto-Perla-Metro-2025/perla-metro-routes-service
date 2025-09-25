using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutesService.Src.Controllers.DTOs;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
namespace RoutesService.Src.Core.Interfaces
{
    public interface IRouteService
    {
        Task<RouteDto> CreateRouteAsync(CreateRouteDto routeDto);
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync(bool? isActive);
        Task<RouteDto?> GetRouteByIdAsync(string id);
        Task UpdateRouteAsync(string id, UpdateRouteDto routeDto);
        Task DeleteRouteAsync(string id);
    }
}