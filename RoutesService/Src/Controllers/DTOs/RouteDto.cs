using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesService.Src.Controllers.DTOs
{
    public class RouteDto
    {
        public string? Id { get; set; }
        public string? OriginStation { get; set; }
        public string? DestinationStation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string>? IntermediateStops { get; set; }
        public bool IsActive { get; set; }
    }
}