using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Falabella
{
    public class URLData
    {
        public DataContext contextoProducto;

        public URLData()
        {
            contextoProducto = new DataContext();
        }

        public bool InsertarURL(Entities.Falabella.URL _url)
        {
            try
            {
                if (ObtenerURL(_url.URL1) != null)
                    return false;

                contextoProducto.FalabellaURL.Add(new Models.FalabellaURL()
                {
                    Activo = _url.Activo,
                    CantPaginas = _url.CantPaginas,
                    Observaciones = _url.Observaciones,
                    URL = _url.URL1,
                    Data = _url.Data,
                    FechaStatus = _url.FechaStatus,
                    Correcto = _url.Correcto
                });

                contextoProducto.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertarURL: " + ex.Message);
            }
        }

        public List<Entities.Falabella.URL> ListarURLs()
        {
            try
            {
                var _obj = (from d in contextoProducto.FalabellaURL
                                //where d.Activo == true
                            select new Entities.Falabella.URL
                            {
                                Activo = d.Activo,
                                Correcto = d.Correcto,
                                URL1 = d.URL,
                                Observaciones = d.Observaciones,
                                id = d.id,
                                Data = d.Data,
                                FechaStatus = d.FechaStatus,
                                CantPaginas = d.CantPaginas
                            }).ToList();
                return _obj;
            }
            catch (Exception ex)
            {
                throw new Exception("ListarURLs: " + ex.Message);
            }
        }

        public Entities.Falabella.URL ObtenerURL(string URL)
        {
            var _obj = (from d in contextoProducto.FalabellaURL
                        where d.URL == URL
                        select new Entities.Falabella.URL
                        {
                            Activo = d.Activo,
                            Correcto = d.Correcto,
                            URL1 = d.URL,
                            Observaciones = d.Observaciones,
                            id = d.id,
                            FechaStatus = d.FechaStatus,
                            Data = d.Data,
                            CantPaginas = d.CantPaginas
                        }).FirstOrDefault();
            return _obj;
        }
        public Entities.Falabella.URL ObtenerURLporID(int idURL)
        {
            var _obj = (from d in contextoProducto.FalabellaURL
                        where d.id == idURL
                        select new Entities.Falabella.URL
                        {
                            Activo = d.Activo,
                            Correcto = d.Correcto,
                            URL1 = d.URL,
                            Observaciones = d.Observaciones,
                            id = d.id,
                            FechaStatus = d.FechaStatus,
                            Data = d.Data,
                            CantPaginas = d.CantPaginas
                        }).FirstOrDefault();
            return _obj;
        }
        public bool ModificarURL(Entities.Falabella.URL _url)
        {
            try
            {
                var _obj = contextoProducto.FalabellaURL.First(x => x.id == _url.id);

                _obj.Observaciones = _url.Observaciones;
                _obj.CantPaginas = _url.CantPaginas;
                _obj.Activo = _url.Activo;
                _obj.Correcto = _url.Correcto;
                _obj.Data = _url.Data;
                _obj.FechaStatus = DateTime.Now;

                contextoProducto.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ModificarURL: " + ex.Message);
            }
        }
        public bool ModificarPagina(string _url, int paginas)
        {
            try
            {
                var _obj = contextoProducto.FalabellaURL.First(x => x.URL == _url);

                _obj.CantPaginas = paginas;
                _obj.FechaStatus = DateTime.Now;

                contextoProducto.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ModificarPagina: " + ex.Message);
            }
        }
    }
}
