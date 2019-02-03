using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Falabella
{
    public class URL
    {
        public int id { get; set; }

        public string URL1 { get; set; }

        public int? CantPaginas { get; set; }

        public bool Activo { get; set; }

        public bool Correcto { get; set; }
        public DateTime FechaStatus { get; set; }
        public string Data { get; set; }

        public string Observaciones { get; set; }
    }
}
