using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var allStops = new List<string> { route.OriginStation! };
                if (route.IntermediateStops != null)
                {
                    allStops.AddRange(route.IntermediateStops);
                }
                allStops.Add(route.DestinationStation!);

                var cypherQuery = @"
                // 1. Crea el nodo central de la ruta
                CREATE (route:Route {
                    id: $Id,
                    startTime: $StartTime,
                    endTime: $EndTime,
                    isActive: $IsActive
                })
                // 2. Usando UNWIND, iteramos sobre la lista de paradas que enviamos
                WITH route
                UNWIND range(0, size($stops) - 1) AS i
                WITH route, $stops[i] AS stopName, i AS sequence
                // 3. Buscamos o creamos cada nodo de estación
                MERGE (station:Station { name: stopName })
                // 4. Creamos la relación ordenada desde la ruta a la estación
                CREATE (route)-[:STATION_STOP { sequence: sequence }]->(station)
                RETURN route";

                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(cypherQuery, new
                    {
                        route.Id,
                        StartTime = new ZonedDateTime(route.StartTime),
                        EndTime = new ZonedDateTime(route.EndTime),
                        route.IsActive,
                        stops = allStops
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return route;
        }

        // READ ALL (with filter)
        public async Task<IEnumerable<RouteEntity>> GetAllRoutesAsync(bool? isActive)
        {
            var routes = new List<RouteEntity>();
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = new StringBuilder(@"
                MATCH (r:Route)
                OPTIONAL MATCH (r)-[rel:STATION_STOP]->(s:Station)
            ");
                var parameters = new Dictionary<string, object>();

                if (isActive.HasValue)
                {
                    cypherQuery.Append(" WHERE r.isActive = $isActive");
                    parameters.Add("isActive", isActive.Value);
                }

                cypherQuery.Append(@"
                WITH r, s, rel
                ORDER BY rel.sequence
                RETURN r, collect(s.name) AS stops
            ");

                var result = await session.RunAsync(cypherQuery.ToString(), parameters);
                var records = await result.ToListAsync();
                foreach (var record in records)
                {
                    routes.Add(MapRecordToRouteEntity(record));
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return routes;
        }

        // READ BY ID
        public async Task<RouteEntity?> GetRouteByIdAsync(string id)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = @"
                MATCH (r:Route {id: $id})
                OPTIONAL MATCH (r)-[rel:STATION_STOP]->(s:Station)
                WITH r, s, rel
                ORDER BY rel.sequence
                RETURN r, collect(s.name) AS stops
            ";
                var result = await session.RunAsync(cypherQuery, new { id });
                var records = await result.ToListAsync();
                var record = records.FirstOrDefault();

                return record == null ? null : MapRecordToRouteEntity(record);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        // UPDATE
        public async Task UpdateRouteAsync(RouteEntity route)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var allStops = new List<string> { route.OriginStation! };
                if (route.IntermediateStops != null)
                {
                    allStops.AddRange(route.IntermediateStops);
                }
                allStops.Add(route.DestinationStation!);

                var cypherQuery = @"
                // 1. Encuentra la ruta y borra todas sus conexiones antiguas
                MATCH (route:Route {id: $Id})
                OPTIONAL MATCH (route)-[oldRel:STATION_STOP]->()
                DELETE oldRel

                // 2. Actualiza las propiedades de la ruta
                SET route.startTime = $StartTime,
                    route.endTime = $EndTime,
                    route.isActive = $IsActive
                
                // 3. Recrea las conexiones con la nueva secuencia de paradas
                WITH route
                UNWIND range(0, size($stops) - 1) AS i
                WITH route, $stops[i] AS stopName, i AS sequence
                MERGE (station:Station { name: stopName })
                CREATE (route)-[:STATION_STOP { sequence: sequence }]->(station)";

                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(cypherQuery, new
                    {
                        route.Id,
                        StartTime = new ZonedDateTime(route.StartTime),
                        EndTime = new ZonedDateTime(route.EndTime),
                        route.IsActive,
                        stops = allStops
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        // DELETE (Soft Delete)
        public async Task DeleteRouteAsync(string id)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var cypherQuery = "MATCH (r:Route {id: $id}) SET r.isActive = false";
                await session.ExecuteWriteAsync(tx => tx.RunAsync(cypherQuery, new { id }));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        // VALIDATE DUPLICATES
        public async Task<bool> RouteExistsAsync(string originStation, string destinationStation, DateTime startTime, List<string> intermediateStops)
        {
            var session = _connection.Driver.AsyncSession();
            try
            {
                var allStops = new List<string> { originStation };
                allStops.AddRange(intermediateStops);
                allStops.Add(destinationStation);

                var cypherQuery = @"
                // 1. Busca rutas que comiencen a la misma hora
                MATCH (r:Route {startTime: $startTime})
                // 2. Obtiene todas las paradas de esa ruta en orden
                MATCH (r)-[rel:STATION_STOP]->(s:Station)
                WITH r, s.name AS stationName, rel.sequence AS seq
                ORDER BY seq
                WITH r, collect(stationName) AS existingStops
                // 3. Compara si la secuencia de paradas es idéntica a la nueva
                WHERE existingStops = $stops
                RETURN count(r) > 0 AS exists";

                var result = await session.RunAsync(cypherQuery, new
                {
                    startTime = new ZonedDateTime(startTime),
                    stops = allStops
                });
                var record = await result.SingleAsync();
                return record["exists"].As<bool>();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        // ===================================================================
        // ===== HELPER FUNCTIONS ==========================================
        // ===================================================================
        private RouteEntity MapRecordToRouteEntity(IRecord record)
        {
            var routeNode = record["r"].As<INode>();
            var stops = record["stops"].As<List<string>>();

            return new RouteEntity
            {
                Id = routeNode["id"].As<string>(),
                StartTime = ConvertToDateTime(routeNode["startTime"]),
                EndTime = ConvertToDateTime(routeNode["endTime"]),
                IsActive = routeNode["isActive"].As<bool>(),
                OriginStation = stops.FirstOrDefault(),
                DestinationStation = stops.LastOrDefault(),
                IntermediateStops = stops.Count > 2 ? stops.GetRange(1, stops.Count - 2) : new List<string>()
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