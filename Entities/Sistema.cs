using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Sistema
    {
        public int id { get; set; }

        public string Nombre { get; set; }
        public string SiteMapURL { get; set; }

        public int PorcentMinDcto { get; set; }
        public bool Activo { get; set; }

    }
}
