using System;

namespace DataAccess.Models
{
    public class Stats
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TimeSpan? Duracion { get; set; }
        public int? CantidadProductos { get; set; }
        public bool? Cargado { get; set; }
        public string Observacion { get; set; }
        public int? IdURL { get; set; }
        public int Sistema { get; set; }
    }
}
