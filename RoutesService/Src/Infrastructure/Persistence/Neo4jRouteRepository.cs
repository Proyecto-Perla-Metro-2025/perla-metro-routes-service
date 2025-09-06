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

        public async Task<IEnumerable<RouteEntity>> GetAllRoutesAsync()
        {
            var routes = new List<RouteEntity>();
            var session = _connection.Driver.AsyncSession();
            try
            {

                var cypherQuery = "MATCH (r:Route) RETURN r";


                await session.ExecuteReadAsync(async tx =>
                {
                    var result = await tx.RunAsync(cypherQuery);
                    await foreach (var record in result)
                    {

                        var node = record["r"].As<INode>();

                        routes.Add(new RouteEntity
                        {
                            Id = node["id"].As<string>(),
                            OriginStation = node["originStation"].As<string>(),
                            DestinationStation = node["destinationStation"].As<string>(),
                            StartTime = node["startTime"].As<DateTimeOffset>().DateTime,
                            EndTime = node["endTime"].As<DateTimeOffset>().DateTime,
                            IntermediateStops = node["intermediateStops"].As<List<string>>(),
                            IsActive = node["isActive"].As<bool>()
                        });
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return routes;
        }
        public async Task<RouteEntity?> GetRouteByIdAsync(string id)
        {
            RouteEntity? route = null;
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = "MATCH (r:Route {id: $id}) RETURN r";

                await session.ExecuteReadAsync(async tx =>
                {
                    var result = await tx.RunAsync(cypherQuery, new { id });

                    // Usamos ToListAsync() para obtener una lista de todos los registros encontrados.
                    // La lista tendrá 0 o 1 elemento.
                    var records = await result.ToListAsync();

                    // Verificamos si la lista contiene algún registro.
                    if (records.Any())
                    {
                        var record = records.First(); // Tomamos el primer (y único) registro.
                        var node = record["r"].As<INode>();
                        route = new RouteEntity
                        {
                            Id = node["id"].As<string>(),
                            OriginStation = node["originStation"].As<string>(),
                            DestinationStation = node["destinationStation"].As<string>(),
                            StartTime = node["startTime"].As<DateTimeOffset>().DateTime,
                            EndTime = node["endTime"].As<DateTimeOffset>().DateTime,
                            IntermediateStops = node["intermediateStops"].As<List<string>>(),
                            IsActive = node["isActive"].As<bool>()
                        };
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }


            return route;
        }
    }
}