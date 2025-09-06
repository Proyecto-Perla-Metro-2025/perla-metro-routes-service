using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
using RoutesService.Src.Core.Interfaces;
using RouteEntity = RoutesService.Src.Core.Entities.Route;

namespace RoutesService.Src.Infrastructure.Persistence
{
    public class Neo4jRouteRepository(Neo4jConnection connection) : IRouteRepository
    {
        private readonly Neo4jConnection _connection = connection;

        public async Task CreateRouteAsync(RouteEntity route)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = @"
                CREATE (r:Route {
                    id: $id,
                    originStation: $originStation,
                    destinationStation: $destinationStation,
                    startTime: $startTime,
                    endTime: $endTime,
                    intermediateStops: $intermediateStops,
                    isActive: $isActive
                })";

                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(cypherQuery, new
                    {
                        id = Guid.NewGuid().ToString(),
                        originStation = route.OriginStation,
                        destinationStation = route.DestinationStation,
                        startTime = route.StartTime,
                        endTime = route.EndTime,
                        intermediateStops = route.IntermediateStops,
                        isActive = route.IsActive
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public Task CreateRouteAsync(Microsoft.AspNetCore.Routing.Route route)
        {
            throw new NotImplementedException();
        }
    }
}