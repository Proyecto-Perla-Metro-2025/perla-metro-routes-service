using RoutesService.Src.Application.Services;
using RoutesService.Src.Core.Interfaces;
using RoutesService.Src.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<Neo4jConnection>(); // Servicio para la conexión a Neo4j
builder.Services.AddScoped<IRouteRepository, Neo4jRouteRepository>(); // Servicio del Repositorio
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddAutoMapper(typeof(Program));
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://*:{port}";
builder.WebHost.UseUrls(url);
var app = builder.Build();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();


