using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Component
{
    public class SistemasComponent
    {
        public static Entities.Sistema ObtenerSistema(int id)
        {
            return new DataAccess.SistemasContext().ObtenerSistema(id);
        }

        public static void InsertarSistema(string Nombre, string SiteMapURL, int PorcentMinDcto, bool Activo)
        {
            new DataAccess.SistemasContext().InsertarSistema(Nombre, SiteMapURL, PorcentMinDcto, Activo);
        }
    }
}
