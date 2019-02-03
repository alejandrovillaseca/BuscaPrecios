using System;
using System.Linq;

namespace DataAccess
{
    public class LogErroresContext
    {
        public DataContext contextoProducto;

        public LogErroresContext()
        {
            contextoProducto = new DataContext();
        }

        public Entities.LogErrores ObtenerLog(int id)
        {
            var _obj = (from d in contextoProducto.LogErrores
                        where d.id == id
                        select new Entities.LogErrores
                        {
                            id = d.id,
                            idProceso = d.idProceso,
                            URL = d.URL,
                            idSistema = d.idSistema,
                            Observaciones = d.Observaciones
                        }).FirstOrDefault();
            return _obj;
        }
        public bool InsertaLog(Entities.LogErrores _log)
        {
            try
            {

                contextoProducto.LogErrores.Add(new Models.LogErrores()
                {
                    idProceso = _log.idProceso,
                    idSistema = _log.idSistema,
                    Observaciones = _log.Observaciones,
                    URL = _log.URL
                });

                contextoProducto.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertaLog: " + ex.Message);
            }
        }
    }
}
