using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
using RoutesService.Src.Core.Interfaces;

namespace RoutesService.Src.Application.Services
{
    public class RouteService(IRouteRepository routeRepository) : IRouteService
    {
        private readonly IRouteRepository _routeRepository = routeRepository;

        public async Task CreateRouteAsync(RouteEntity route)
        {
            await _routeRepository.CreateRouteAsync(route);
        }


    }
}