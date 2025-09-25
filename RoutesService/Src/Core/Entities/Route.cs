using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesService.Src.Core.Entities
{
    public class Route
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public string? OriginStation { get; set; }
        public string? DestinationStation { get; set; }
        public List<string> IntermediateStops { get; set; } = new();
    }
}