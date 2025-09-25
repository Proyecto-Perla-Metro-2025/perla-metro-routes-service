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

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto routeDto)
        {
            if (routeDto.EndTime <= routeDto.StartTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }
            var exists = await _routeRepository.RouteExistsAsync(
                routeDto.OriginStation,
                routeDto.DestinationStation,
                routeDto.StartTime,
                routeDto.IntermediateStops
            );
            if (exists)
            {
                throw new InvalidOperationException("An identical route already exists.");
            }
            var routeEntity = _mapper.Map<RouteEntity>(routeDto);

            var createdEntity = await _routeRepository.CreateRouteAsync(routeEntity);

            return _mapper.Map<RouteDto>(createdEntity);
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync(bool? isActive)
        {
            var routes = await _routeRepository.GetAllRoutesAsync(isActive);
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }
        public async Task<RouteDto?> GetRouteByIdAsync(string id)
        {
            var route = await _routeRepository.GetRouteByIdAsync(id);
            return _mapper.Map<RouteDto>(route);
        }
        public async Task UpdateRouteAsync(string id, UpdateRouteDto routeDto)
        {
            if (routeDto.EndTime <= routeDto.StartTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }
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