using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RoutesService.Src.Controllers.DTOs;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
namespace RoutesService.Src.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRouteDto, RouteEntity>();
            CreateMap<RouteEntity, RouteDto>();
            CreateMap<UpdateRouteDto, RouteEntity>();
        }
    }
}