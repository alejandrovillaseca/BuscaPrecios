using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Falabella
{
    public class BuscarResponse: Cabecera
    {
        public List<Producto> Productos { get; set; }
    }
}
