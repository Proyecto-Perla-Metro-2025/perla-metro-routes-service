using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesService.Src.Core.Interfaces
{
    public interface IRouteRepository
    {
        Task CreateRouteAsync(Route route);
    }
}