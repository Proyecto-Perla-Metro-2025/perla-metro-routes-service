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

        public async Task<IEnumerable<RouteEntity>> GetAllRoutesAsync()
        {

            return await _routeRepository.GetAllRoutesAsync();
        }
        public async Task<RouteEntity?> GetRouteByIdAsync(string id)
        {
            return await _routeRepository.GetRouteByIdAsync(id);
        }
        public async Task UpdateRouteAsync(RouteEntity route)
        {
            
            var existingRoute = await _routeRepository.GetRouteByIdAsync(route.Id);
            if (existingRoute == null)
            {
                throw new KeyNotFoundException($"Route with id '{route.Id}' was not found.");
            }
            
            await _routeRepository.UpdateRouteAsync(route);
        }
    }
}