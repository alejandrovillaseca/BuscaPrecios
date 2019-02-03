using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class StatsContext
    {
        public DataContext contextoProducto;

        public StatsContext()
        {
            contextoProducto = new DataContext();
        }

        public Entities.Stats ObtenerStats(int Id)
        {
            var _obj = (from d in contextoProducto.Stats
                        where d.Id == Id
                        select new Entities.Stats
                        {
                            Id = d.Id,
                            CantidadProductos = d.CantidadProductos,
                            Cargado = d.Cargado,
                            Duracion = d.Duracion,
                            IdURL = d.IdURL,
                            FechaFin = d.FechaFin,
                            FechaInicio = d.FechaInicio,
                            Sistema = d.Sistema,
                            Observacion = d.Observacion,
                        }).FirstOrDefault();
            return _obj;
        }
        public List<Entities.Stats> ObtenerStatsPorUrl(int IdURL)
        {
            var _obj = (from d in contextoProducto.Stats
                        where d.IdURL == IdURL
                        select new Entities.Stats
                        {
                            Id = d.Id,
                            CantidadProductos = d.CantidadProductos,
                            Cargado = d.Cargado,
                            Duracion = d.Duracion,
                            IdURL = d.IdURL,
                            FechaFin = d.FechaFin,
                            FechaInicio = d.FechaInicio,
                            Sistema = d.Sistema,
                            Observacion = d.Observacion,
                        }).OrderByDescending(x => x.FechaFin).ToList();
            return _obj;
        }
        public List<Entities.Stats> ObtenerStatsProcesados(int idPrimerProceso)
        {
            var _obj = (from d in contextoProducto.Stats
                        where d.Id >= idPrimerProceso
                        select new Entities.Stats
                        {
                            Id = d.Id,
                            CantidadProductos = d.CantidadProductos,
                            Cargado = d.Cargado,
                            Duracion = d.Duracion,
                            IdURL = d.IdURL,
                            FechaFin = d.FechaFin,
                            FechaInicio = d.FechaInicio,
                            Sistema = d.Sistema,
                            Observacion = d.Observacion,
                        }).ToList();
            return _obj;
        }
        public bool InsertaStats(Entities.Stats _stats, out int idProceso)
        {
            try
            {

                contextoProducto.Stats.Add(new Models.Stats()
                {
                    Sistema = _stats.Sistema,
                    FechaInicio = _stats.FechaInicio,
                    FechaFin = _stats.FechaFin,
                    Duracion = _stats.Duracion,
                    IdURL = _stats.IdURL,
                    Cargado = _stats.Cargado,
                    CantidadProductos = _stats.CantidadProductos,
                    Observacion = _stats.Observacion
                });

                contextoProducto.SaveChanges();

                idProceso = contextoProducto.Stats.OrderByDescending(u => u.Id).First().Id;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertaStats: " + ex.Message);
            }
        }
        public bool ModificaStats(Entities.Stats _stats)
        {
            try
            {
                var _obj = contextoProducto.Stats.First(x => x.Id == _stats.Id);

                _obj.Sistema = _stats.Sistema;
                _obj.FechaInicio = _stats.FechaInicio;
                _obj.FechaFin = _stats.FechaFin;
                _obj.CantidadProductos = _stats.CantidadProductos;
                _obj.Cargado = _stats.Cargado;
                _obj.IdURL = _stats.IdURL;
                _obj.Duracion = _stats.Duracion;
                _obj.Observacion = _stats.Observacion;
                contextoProducto.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ModificaStats: " + ex.Message);
            }
        }
    }
}
