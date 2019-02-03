using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Component
{
    public class ErrorLogComponent
    {
        public static Entities.LogErrores ObtenerLog(int id)
        {
            return new DataAccess.LogErroresContext().ObtenerLog(id);
        }
        public static bool InsertaLog(Entities.LogErrores _log)
        {
            return new DataAccess.LogErroresContext().InsertaLog(_log);
        }
    }
}
