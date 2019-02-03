using System;
using System.Linq;
using Entities;

namespace DataAccess
{
    public class SistemasContext
    {
        public DataContext contextoProducto;

        public SistemasContext()
        {
            contextoProducto = new DataContext();
        }

        public Entities.Sistema ObtenerSistema(int id)
        {
            var _obj = (from d in contextoProducto.Sistemas
                        where d.id == id
                        select new Entities.Sistema
                        {
                            id = d.id,
                            Activo = d.Activo,
                            Nombre = d.Nombre,
                            SiteMapURL = d.SiteMapURL,
                            PorcentMinDcto = d.PorcentMinDcto
                        }).FirstOrDefault();
            return _obj;
        }
        public void InsertarSistema(string Nombre, string SiteMapURL, int PorcentMinDcto, bool Activo)
        {
            contextoProducto.Sistemas.Add(new Models.Sistemas()
            {
                Activo = Activo,
                Nombre = Nombre,
                PorcentMinDcto = PorcentMinDcto,
                SiteMapURL = SiteMapURL
            });

            contextoProducto.SaveChanges();
        }
    }
}
