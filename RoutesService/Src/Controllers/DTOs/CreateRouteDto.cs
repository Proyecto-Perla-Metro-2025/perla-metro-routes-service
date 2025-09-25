using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesService.Src.Controllers.DTOs
{
    public class CreateRouteDto
    {
        [Required(ErrorMessage = "Origin station is required.")]
        [StringLength(100, MinimumLength = 3)]
        public string? OriginStation { get; set; }

        [Required(ErrorMessage = "Destination station is required.")]
        [StringLength(100, MinimumLength = 3)]
        public string? DestinationStation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string>? IntermediateStops { get; set; }
        public bool IsActive { get; set; } = true; // Valor por defecto
    }
}