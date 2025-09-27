using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutesService.Src.Core.Interfaces;
using RouteEntity = RoutesService.Src.Core.Entities.Route;
namespace RoutesService.Src.Data
{
    public class DataSeeder
    {
        public static async Task SeedRoutes(IServiceProvider serviceProvider)
        {
            
            using (var scope = serviceProvider.CreateScope())
            {
                var routeRepository = scope.ServiceProvider.GetRequiredService<IRouteRepository>();

                // Verificamos si ya existen rutas en la base de datos
                var existingRoutes = await routeRepository.GetAllRoutesAsync(null);
                if (existingRoutes.Any())
                {
                    // Si ya hay rutas, no hacemos nada.
                    Console.WriteLine("La base de datos de rutas ya contiene datos. No se ejecutará el seeder.");
                    return;
                }

                Console.WriteLine("Base de datos de rutas vacía. Ejecutando seeder...");

                // Creamos una lista de rutas para la precarga
                var routesToSeed = new List<RouteEntity>
                {
                    new RouteEntity
                    {
                        OriginStation = "Estación Central",
                        DestinationStation = "Estación La Portada",
                        StartTime = DateTime.UtcNow.Date.AddHours(6), // 06:00 AM
                        EndTime = DateTime.UtcNow.Date.AddHours(7),   // 07:00 AM
                        IntermediateStops = new List<string> { "Estación Prat", "Estación Latorre" },
                        IsActive = true
                    },
                    new RouteEntity
                    {
                        OriginStation = "Estación La Portada",
                        DestinationStation = "Estación Central",
                        StartTime = DateTime.UtcNow.Date.AddHours(8), // 08:00 AM
                        EndTime = DateTime.UtcNow.Date.AddHours(9),   // 09:00 AM
                        IntermediateStops = new List<string> { "Estación Latorre", "Estación Prat" },
                        IsActive = true
                    },
                    new RouteEntity
                    {
                        OriginStation = "Estación Sur",
                        DestinationStation = "Estación Norte",
                        StartTime = DateTime.UtcNow.Date.AddHours(10), // 10:00 AM
                        EndTime = DateTime.UtcNow.Date.AddHours(11),   // 11:00 AM
                        IntermediateStops = new List<string> { "Estación Centro" },
                        IsActive = false // Una ruta inactiva de ejemplo
                    }
                };

                // Guardamos cada ruta en la base de datos
                foreach (var route in routesToSeed)
                {
                    await routeRepository.CreateRouteAsync(route);
                }

                Console.WriteLine("Seeder de rutas completado. Se han añadido nuevas rutas.");
            }
        }
    }
}