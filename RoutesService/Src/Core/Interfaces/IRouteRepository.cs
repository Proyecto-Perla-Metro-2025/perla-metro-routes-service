using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
namespace RoutesService.Src.Core.Interfaces
{
    public interface IRouteRepository
    {
        Task CreateRouteAsync(RouteEntity route);
    }
}