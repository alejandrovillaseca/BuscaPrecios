using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LogErrores
    {
        public int id { get; set; }
        public string Observaciones { get; set; }
        public string URL { get; set; }
        public int idProceso { get; set; }
        public int idSistema { get; set; }
    }  
}
