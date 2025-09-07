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

        public async Task<RouteEntity> CreateRouteAsync(RouteEntity route)
        {
            route.Id = Guid.NewGuid().ToString();
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = @"
                CREATE (r:Route {
                    id: $Id,
                    originStation: $OriginStation,
                    destinationStation: $DestinationStation,
                    startTime: $StartTime,
                    endTime: $EndTime,
                    intermediateStops: $IntermediateStops,
                    isActive: $IsActive
                })";

                await session.ExecuteWriteAsync(async tx =>
                {
                    
                    await tx.RunAsync(cypherQuery, new
                    {
                        route.Id,
                        route.OriginStation,
                        route.DestinationStation,
                        StartTime = new ZonedDateTime(route.StartTime),
                        EndTime = new ZonedDateTime(route.EndTime),
                        route.IntermediateStops,
                        route.IsActive
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return route;
        }

        public async Task<IEnumerable<RouteEntity>> GetAllRoutesAsync()
        {
            var routes = new List<RouteEntity>();
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = "MATCH (r:Route) RETURN r";
                var result = await session.RunAsync(cypherQuery);
                var records = await result.ToListAsync();
                foreach (var record in records)
                {
                    var node = record["r"].As<INode>();
                    routes.Add(MapNodeToRouteEntity(node));
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return routes;
        }

        public async Task<RouteEntity?> GetRouteByIdAsync(string id)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = "MATCH (r:Route {id: $id}) RETURN r";
                var result = await session.RunAsync(cypherQuery, new { id });
                var record = await result.FetchAsync() && result.Current != null ? result.Current : null;

                if (record == null)
                {
                    return null;
                }

                var node = record["r"].As<INode>();
                return MapNodeToRouteEntity(node);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateRouteAsync(RouteEntity route)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = @"
            MATCH (r:Route {id: $id})
            SET r.originStation = $originStation,
                r.destinationStation = $destinationStation,
                r.startTime = $startTime,
                r.endTime = $endTime,
                r.intermediateStops = $intermediateStops,
                r.isActive = $isActive";

                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(cypherQuery, new
                    {
                        id = route.Id,
                        originStation = route.OriginStation,
                        destinationStation = route.DestinationStation,
                        startTime = new ZonedDateTime(route.StartTime, TimeZoneInfo.Utc.Id),
                        endTime = new ZonedDateTime(route.EndTime, TimeZoneInfo.Utc.Id),
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
        public async Task DeleteRouteAsync(string id)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                // Consulta Cypher para encontrar un nodo por su ID y cambiar una sola propiedad.
                var cypherQuery = @"
            MATCH (r:Route {id: $id})
            SET r.isActive = false";

                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(cypherQuery, new { id });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        private RouteEntity MapNodeToRouteEntity(INode node)
        {
            return new RouteEntity
            {
                Id = node["id"].As<string>(),
                OriginStation = node["originStation"].As<string>(),
                DestinationStation = node["destinationStation"].As<string>(),
                // Usamos nuestra función de ayuda para convertir las fechas de forma segura
                StartTime = ConvertToDateTime(node["startTime"]),
                EndTime = ConvertToDateTime(node["endTime"]),
                IntermediateStops = node["intermediateStops"].As<List<string>>(),
                IsActive = node["isActive"].As<bool>()
            };
        }

        private DateTime ConvertToDateTime(object neo4jDate)
        {
            if (neo4jDate is ZonedDateTime zonedDateTime)
            {

                return zonedDateTime.ToDateTimeOffset().DateTime;
            }
            if (neo4jDate is LocalDateTime localDateTime)
            {

                return localDateTime.ToDateTime();
            }

            return (DateTime)neo4jDate;
        }
    }
}