using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
using RoutesService.Src.Core.Interfaces;
using AutoMapper;
using RoutesService.Src.Controllers.DTOs;
namespace RoutesService.Src.Application.Services
{
    public class RouteService(IRouteRepository routeRepository, IMapper mapper) : IRouteService
    {
        private readonly IRouteRepository _routeRepository = routeRepository;
        private readonly IMapper _mapper = mapper;

        public async Task CreateRouteAsync(CreateRouteDto routeDto)
        {
            var route = _mapper.Map<RouteEntity>(routeDto);
            await _routeRepository.CreateRouteAsync(route);
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllRoutesAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }
        public async Task<RouteDto?> GetRouteByIdAsync(string id)
        {
            var route = await _routeRepository.GetRouteByIdAsync(id);
            return _mapper.Map<RouteDto>(route);
        }
        public async Task UpdateRouteAsync(string id, UpdateRouteDto routeDto)
        {
            var existingRoute = await _routeRepository.GetRouteByIdAsync(id);
            if (existingRoute == null)
            {
                throw new KeyNotFoundException($"Route with id '{id}' was not found.");
            }

            _mapper.Map(routeDto, existingRoute);

            await _routeRepository.UpdateRouteAsync(existingRoute);
        }
        public async Task DeleteRouteAsync(string id)
        {

            var existingRoute = await _routeRepository.GetRouteByIdAsync(id);
            if (existingRoute == null)
            {

                throw new KeyNotFoundException($"Route with id '{id}' was not found.");
            }


            await _routeRepository.DeleteRouteAsync(id);
        }
    }
}