using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
namespace RoutesService.Src.Core.Interfaces
{
    public interface IRouteService
    {
        Task CreateRouteAsync(RouteEntity route);
        Task<IEnumerable<RouteEntity>> GetAllRoutesAsync();
        Task<RouteEntity?> GetRouteByIdAsync(string id);
    }
}