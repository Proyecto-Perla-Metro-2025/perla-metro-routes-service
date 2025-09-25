using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
namespace RoutesService.Src.Core.Interfaces
{
    public interface IRouteRepository
    {
        Task<RouteEntity> CreateRouteAsync(RouteEntity route);
        Task<IEnumerable<RouteEntity>> GetAllRoutesAsync(bool? isActive);
        Task<RouteEntity?> GetRouteByIdAsync(string id);
        Task UpdateRouteAsync(RouteEntity route);
        Task DeleteRouteAsync(string id);
        Task<bool> RouteExistsAsync(string originStation, string destinationStation, DateTime startTime, List<string> intermediateStops);
    }
}