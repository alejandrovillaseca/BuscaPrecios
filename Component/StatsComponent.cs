using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Component
{
    public class StatsComponent
    {
        public static Entities.Stats ObtenerStats(int id)
        {
            return new DataAccess.StatsContext().ObtenerStats(id);
        }
        public static List<Entities.Stats> ObtenerStatsPorURL(int idURL)
        {
            return new DataAccess.StatsContext().ObtenerStatsPorUrl(idURL);
        }
        public static List<Entities.Stats> ObtenerStatsProcesados(int idPrimerProceso)
        {
            return new DataAccess.StatsContext().ObtenerStatsProcesados(idPrimerProceso);
        }
        public static bool InsertaStats(Entities.Stats _stats, out int idProceso)
        {
            return new DataAccess.StatsContext().InsertaStats(_stats, out idProceso);
        }
        public static bool ModificaStats(Entities.Stats _stats)
        {
            return new DataAccess.StatsContext().ModificaStats(_stats);
        }
    }
}
