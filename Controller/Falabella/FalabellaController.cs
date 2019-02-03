using System;
using System.IO;
using System.Net;
using Entities;
using Component;
using Handlers;
using System.Collections.Generic;
using static Handlers.SiteMap;
using System.Linq;

namespace Controller
{
    public class FalabellaController
    {
        public static Entities.Falabella.BuscarResponse ProcesaSoloPorURL(Entities.Falabella.URL _url, out HtmlAgilityPack.HtmlDocument _htmlDocument, out int CantidadProductos, int idProceso)
        {
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            string source = string.Empty;
            try
            {
                WebRequest req = HttpWebRequest.Create(_url.URL1);
                req.Method = "GET";

                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    source = reader.ReadToEnd();
                }

                htmlDocument.LoadHtml(source);
            }
            catch (Exception ex)
            {
                //Marcamos el log como error:
                Entities.LogErrores _log = new Entities.LogErrores()
                {
                    idProceso = idProceso,
                    idSistema = 1,
                    URL = _url.URL1,
                    Observaciones = "ProcesaSoloPorURL: Error al obtener información de la URL. " + ex.Message
                };
                Component.ErrorLogComponent.InsertaLog(_log);

                CantidadProductos = 0;
                _htmlDocument = htmlDocument;
                return new Entities.Falabella.BuscarResponse()
                {
                    Correcto = false,
                    FechaProceso = DateTime.Now,
                    Observaciones = _log.Observaciones,
                    Productos = null
                };
            }

            //ahora que tenemos todo el codigo, seleccionamos donde deberían estar los productos
            var nodes = htmlDocument.DocumentNode.SelectNodes(string.Format("//div[@class='cajaLP4x']"));
            if (nodes == null)
            {
                _htmlDocument = null;
                CantidadProductos = 0;
                return new Entities.Falabella.BuscarResponse()
                {
                    Correcto = false,
                    FechaProceso = DateTime.Now,
                    Observaciones = "No se encontraron productos",
                    Productos = null
                };
            }

            List<Entities.Falabella.Producto> _listProductos = new List<Entities.Falabella.Producto>();

            foreach (var item in nodes)
            {
                //Ahora estoy en el producto
                Entities.Falabella.Producto _obj = new Entities.Falabella.Producto();
                //Inserto el producto en null
                _obj.Correcto = false;
                _obj.NombreProducto = "Producto Sin Nombre";
                _obj.Marca = "Producto Sin Marca";
                _obj.Link = _url.URL1;
                _obj.idURL = _url.id;
                _obj.Observaciones = "Producto para ser procesado";
                _obj.FechaProceso = DateTime.Now;
                _obj.idProceso = idProceso;
                int idProducto;
                Component.ProductoComponent.InsertarProducto(_obj, out idProducto);
                _obj.id = idProducto;
                try
                {
                    HtmlAgilityPack.HtmlDocument htmlProduct = new HtmlAgilityPack.HtmlDocument();
                    htmlProduct.LoadHtml(item.InnerHtml);

                    //Precios
                    var precios = htmlProduct.DocumentNode.SelectNodes(string.Format("//span[@class='unitPriceD']"));
                    if (precios == null)
                    {
                        //No hay preicos aqui, intentaremos buscar de otra forma
                        precios = htmlProduct.DocumentNode.SelectNodes(string.Format("//div[@class='wishlistPrice1']"));
                    }
                    if (precios.Count > 2)
                    {
                        //Es por que tiene "Oportunidad única en CMR"
                        _obj.Precio = Convert.ToInt32(precios[0].InnerHtml.Replace("$", "").Replace(".", ""));
                        _obj.PrecioInternet = Convert.ToInt32(precios[1].InnerHtml.Replace("$", "").Replace(".", ""));
                        _obj.PrecioNormal = Convert.ToInt32(precios[2].InnerHtml.Replace("$", "").Replace(".", ""));
                        _obj.DescuentoCMR = true;
                    }
                    else if (precios.Count == 1)
                    {
                        _obj.Precio = 0;
                        try
                        {
                            int _preciointernet;
                            if (precios[0].InnerHtml.Contains("div class"))
                            {
                                _preciointernet = Convert.ToInt32(precios[0].InnerHtml.Substring(precios[0].InnerHtml.IndexOf("$"), precios[0].InnerHtml.Length - precios[0].InnerHtml.IndexOf("$") - 19).Replace("$", "").Replace(".", ""));
                            }
                            else
                            {
                                _preciointernet = Convert.ToInt32(precios[0].InnerHtml.Replace("$", "").Replace(".", ""));
                            }
                            _obj.PrecioInternet = _preciointernet;
                        }
                        catch (Exception)
                        {
                            _obj.PrecioInternet = 0;
                            _obj.Observaciones = "No se pudo obtener el precio";
                        }

                        _obj.PrecioNormal = 0;
                        _obj.DescuentoCMR = false;
                    }
                    else
                    {
                        _obj.Precio = 0;
                        _obj.PrecioInternet = Convert.ToInt32(precios[0].InnerHtml.Replace("$", "").Replace(".", ""));
                        _obj.PrecioNormal = Convert.ToInt32(precios[1].InnerHtml.Replace("$", "").Replace(".", ""));
                        _obj.DescuentoCMR = false;
                    }

                    //Nombre
                    var nombreProducto = htmlProduct.DocumentNode.SelectNodes(string.Format("//div[@class='detalle']"));
                    _obj.NombreProducto = nombreProducto[0].InnerText.Replace("\r\n\t\t", String.Empty);

                    //Código producto
                    var codigoProducto = htmlProduct.DocumentNode.OuterHtml.ToString().Substring(htmlProduct.DocumentNode.OuterHtml.ToString().IndexOf(@"<div id=""desc_") + 14, 16);
                    _obj.CodigoProcudto = codigoProducto.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("\\", "").Replace(">", "").Replace(@"""", "").TrimEnd();
                    //Si ya existe el mismo producto en el mismo proceso, lo eliminamos
                    bool repetido = Component.ProductoComponent.EliminaProductosRepetidos(_obj.idProceso, _obj.CodigoProcudto);
                    if (repetido == true) continue;


                    //Marca
                    var marcaProducto = htmlProduct.DocumentNode.SelectNodes(string.Format("//div[@class='marca']"));
                    _obj.Marca = marcaProducto[0].InnerText.Replace("\r\n\t\t", String.Empty);

                    //Link
                    _obj.Link = nombreProducto[0].InnerHtml;
                    _obj.Link = "http://www.falabella.com" + _obj.Link.Substring(_obj.Link.IndexOf("href") + 6, _obj.Link.IndexOf(@""">") - _obj.Link.IndexOf("href") - 6);

                    //Lo agregamos a la base de datos
                    ModificaProducto(_obj);
                    _listProductos.Add(_obj);
                }
                catch (Exception ex)
                {
                    //Marcamos como error
                    _obj.Observaciones = "ProcesaSoloPorURL: " + ex.Message;
                    _obj.Correcto = false;
                    Component.ProductoComponent.ModificarProducto(_obj);
                    //throw new Exception(_obj.Observaciones);
                    CantidadProductos = _listProductos.Count;
                    _htmlDocument = htmlDocument;
                    return new Entities.Falabella.BuscarResponse()
                    {
                        Correcto = false,
                        FechaProceso = DateTime.Now,
                        Observaciones = _obj.Observaciones,
                        Productos = null
                    };
                }

            }

            CantidadProductos = _listProductos.Count;
            _htmlDocument = htmlDocument;
            return new Entities.Falabella.BuscarResponse()
            {
                Correcto = true,
                FechaProceso = DateTime.Now,
                Observaciones = "Lista De Productos Generada Correctamente",
                Productos = _listProductos
            };

        }
        public static List<Entities.Falabella.BuscarResponse> ProcesaPorLotes(string RutaXML)
        {
            List<Entities.Falabella.BuscarResponse> list = new List<Entities.Falabella.BuscarResponse>();
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(RutaXML);
            int CantidadProductos = 0;

            new Handlers.SiteMap();
            //sitemap.WriteSitemapToFile(@"C:\Users\Alejandro\Documents\Visual Studio 2013\Projects\Busca\Busca.WebServicesa\SiteMap\sitemap.xml");

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType == System.Xml.XmlNodeType.Text)
                {
                    if (reader.Value.Contains("www"))
                    {
                        try
                        {
                            list.Add(ProcesaTodosPorCategoria(out CantidadProductos));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            return list;
        }
        public static Entities.Falabella.BuscarResponse ProcesaPorSiteMap()
        {
            Component.SistemasComponent.InsertarSistema("", "", 1, true);
            //Registramos el inicio
            var _stats = new Entities.Stats()
            {
                Cargado = false,
                IdURL = null,
                FechaInicio = DateTime.Now,
                Sistema = 1,
                Observacion = "Lista de URL aún no cargada" //Falabella CL Retail
            };
            int idProceso;
            var stats = Component.StatsComponent.InsertaStats(_stats, out idProceso);
            _stats.Id = idProceso;

            //Buscamos el SiteMap
            var _URL = Component.SistemasComponent.ObtenerSistema(1);
            if (_URL == null)
                Component.SistemasComponent.InsertarSistema("Falabella", "", 5, true);

            string URL = _URL.SiteMapURL;
            WebRequest req = HttpWebRequest.Create(URL);
            req.Method = "GET";

            string source;
            using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(source);

            //int CantidadProductos = 0;
            var nodes = htmlDocument.DocumentNode.SelectNodes(string.Format("//a[@class='linkgrismapasitio']"));
            if (nodes == null)
            {
                ////_stats.FechaFin = DateTime.Now;
                ////_stats.CantidadProductos = 0;
                ////_stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
                ////_stats.Observaciones = "No se encontraron URL";
                ////Component.StatsComponent.ModificaStats(_stats);
                return new Entities.Falabella.BuscarResponse()
                {
                    Correcto = false,
                    FechaProceso = DateTime.Now,
                    Observaciones = "No se encontraron URL's",
                    Productos = null
                };
            }
            else
            {
                try
                {
                    //Buscamos las categorías
                    foreach (var item in nodes)
                    {
                        //Obtenemos la URL de la categoría
                        string urlCateg = "http://www.falabella.com" + item.OuterHtml.Substring(item.OuterHtml.IndexOf("href") + 6, item.OuterHtml.IndexOf(@""">") - item.OuterHtml.IndexOf("href") - 6);
                        if (urlCateg.Contains(";")) urlCateg = urlCateg.Substring(0, urlCateg.IndexOf(";"));
                        //ProcesaPorCategoriaURL(urlCateg, out CantidadProductos, idProceso);
                        //Guardamos las URL para escanear
                        //Primero vemos si existe
                        var obj = Component.ProductoComponent.ObtenerURL(urlCateg);
                        if (obj == null)
                        {
                            //No existe, la insertamos
                            var _url = new Entities.Falabella.URL()
                            {
                                Activo = true,
                                CantPaginas = null,
                                Correcto = true,
                                Data = null,
                                FechaStatus = DateTime.Now,
                                Observaciones = "URL Agregada correctamente",
                                URL1 = urlCateg
                            };
                            Component.ProductoComponent.InsertarURL(_url);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Marcamos error
                    _stats.FechaFin = DateTime.Now;
                    _stats.CantidadProductos = 0;
                    _stats.Cargado = false;
                    _stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
                    _stats.Observacion = ex.Message;
                    Component.StatsComponent.ModificaStats(_stats);
                    throw new Exception(ex.Message);
                }

            }
            //Registramos el final
            _stats.FechaFin = DateTime.Now;
            _stats.CantidadProductos = 0;
            _stats.Cargado = true;
            _stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
            _stats.Observacion = "Lista de URL Actualizada correctamente";
            Component.StatsComponent.ModificaStats(_stats);

            return new Entities.Falabella.BuscarResponse()
            {
                Correcto = true,
                FechaProceso = DateTime.Now,
                Observaciones = "Proceso ejecutado correctamente",
                Productos = null
            };
        }
        public static void ModificaProducto(Entities.Falabella.Producto _producto)
        {
            try
            {
                //Antes de insertar, vemos si existe el producto a insertar para compararlo
                var _obj = Component.ProductoComponent.ObtenerProducto(_producto.CodigoProcudto);
                if (_obj != null)
                {
                    //El producto existe en base de datos
                    _producto.FechaProceso = DateTime.Now;
                    _producto.Correcto = true;

                    if ((_obj.PrecioInternet != null) && (_producto.PrecioInternet < _obj.PrecioInternet))
                    {
                        //El precio internet bajo!!!
                        //Buscammos el porcentaje mínimo configurado
                        var _sist = Component.SistemasComponent.ObtenerSistema(1);

                        _producto.Observaciones = "El precio internet bajo de " + _obj.PrecioInternet + " a " + _producto.PrecioInternet;
                        Component.ProductoComponent.ModificarProducto(_producto);

                        //Notificamos si cumple
                        var porcentaje = (_producto.PrecioInternet * 100) / _obj.PrecioInternet;
                        if (porcentaje >= _sist.PorcentMinDcto)
                        {
                            //Notifico por correo
                            Correos.EnviarCorreo(porcentaje + "% DCTO!! " + _producto.NombreProducto, "El precio internet bajo de " + _obj.PrecioInternet + " a " + _producto.PrecioInternet + " LINK: " + _producto.Link);
                        }
                    }
                    else
                    {
                        _producto.Observaciones = "Producto sin cambios en precio internet";
                        _producto.FechaProceso = DateTime.Now;
                        Component.ProductoComponent.ModificarProducto(_producto);
                    }
                }
                else
                {
                    //No existe otro producto simil, solo lo modificamos como insertado
                    _producto.FechaProceso = DateTime.Now;
                    _producto.Correcto = true;
                    _producto.Observaciones = "Producto nuevo agregado";
                    Component.ProductoComponent.ModificarProducto(_producto);
                }
            }
            catch (Exception ex)
            {
                _producto.FechaProceso = DateTime.Now;
                _producto.Observaciones = "InsertarProducto: " + ex.Message;
                _producto.Correcto = false;
                Component.ProductoComponent.ModificarProducto(_producto);

            }
        }
        public static Entities.Falabella.BuscarResponse ProcesaTodosPorCategoria(out int CantidadProductos)
        {
            try
            {
                //Registramos el inicio
                var _stats = new Entities.Stats()
                {
                    Cargado = false,
                    IdURL = null,
                    FechaInicio = DateTime.Now,
                    Sistema = 1 //Falabella CL Retail
                };
                int idProceso;
                var stats = Component.StatsComponent.InsertaStats(_stats, out idProceso);
                _stats.Id = idProceso;

                int TotalProductos = 0;
                var _Caturl = Component.ProductoComponent.ListarURLs();
                foreach (var _item in _Caturl)
                {
                    //Primero vemos si es una URL con Sub-Categorías
                    WebRequest req = HttpWebRequest.Create(_item.URL1);
                    req.Method = "GET";


                    string source = string.Empty;
                    try
                    {
                        using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                        {
                            source = reader.ReadToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        var _log = new Entities.LogErrores()
                        {
                            idProceso = idProceso,
                            idSistema = 1,
                            Observaciones = ex.Message,
                            URL = _item.URL1
                        };
                        Component.ErrorLogComponent.InsertaLog(_log);

                        continue;
                    }


                    HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(source);
                    var subcat = htmlDocument.DocumentNode.SelectNodes(string.Format("//ul[@class='subCategorias']"));
                    if (subcat != null)
                    {
                        //Es una página que contiene categorías, entonces las leemos
                        HtmlAgilityPack.HtmlDocument _htmlDocument = new HtmlAgilityPack.HtmlDocument();
                        _htmlDocument.LoadHtml(subcat.First().OuterHtml);
                        var lst = _htmlDocument.DocumentNode.SelectNodes(string.Format("//a"));
                        //Parallel.ForEach(lst, item =>
                        //{
                        //    //Obtenemos la URL
                        //    _item.URL1 = "http://www.falabella.com" + item.OuterHtml.Substring(item.OuterHtml.IndexOf("href") + 6, item.OuterHtml.IndexOf(@""">") - item.OuterHtml.IndexOf("href") - 6);
                        //    if (_item.URL1.Contains("?"))
                        //    {
                        //        _item.URL1 = _item.URL1 + "&Nrpp=999";
                        //    }
                        //    else
                        //    {
                        //        _item.URL1 = _item.URL1 + "?Nrpp=999";
                        //    }
                        //    ProcesaSubCategoriaURL(_item, idProceso);
                        //    //TotalProductos = TotalProductos + CantidadProductos;
                        //});
                        foreach (var item in lst)
                        {
                            //Obtenemos la URL
                            _item.URL1 = "http://www.falabella.com" + item.OuterHtml.Substring(item.OuterHtml.IndexOf("href") + 6, item.OuterHtml.IndexOf(@""">") - item.OuterHtml.IndexOf("href") - 6);
                            if (_item.URL1.Contains("?"))
                            {
                                _item.URL1 = _item.URL1 + "&Nrpp=999";
                            }
                            else
                            {
                                _item.URL1 = _item.URL1 + "?Nrpp=999";
                            }
                            ProcesaSubCategoriaURL(_item, idProceso, out TotalProductos);
                            //TotalProductos = TotalProductos + CantidadProductos;
                        }

                    }
                    else
                    {
                        //Ahora procesamos la SubCategoría
                        if (_item.URL1.Contains("?"))
                        {
                            _item.URL1 = _item.URL1 + "&Nrpp=999";
                        }
                        else
                        {
                            _item.URL1 = _item.URL1 + "?Nrpp=999";
                        }
                        ProcesaSubCategoriaURL(_item, idProceso, out TotalProductos);
                        //TotalProductos = TotalProductos + CantidadProductos;
                    }
                }


                CantidadProductos = TotalProductos;

                //Registramos el final
                _stats.FechaFin = DateTime.Now;
                _stats.CantidadProductos = CantidadProductos;
                _stats.Cargado = true;
                _stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
                _stats.Observacion = "Proceso ejecutado correctamente";
                Component.StatsComponent.ModificaStats(_stats);

                return new Entities.Falabella.BuscarResponse()
                {
                    Correcto = true,
                    FechaProceso = DateTime.Now,
                    Observaciones = "Lista De Productos Generada Correctamente",
                    Productos = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception("ProcesaPorCategoriaURL: " + ex.Message);
            }
        }
        private static void ProcesaSubCategoriaURL(Entities.Falabella.URL _url, int idProceso, out int TotalProductos)
        {
            try
            {
                //Procesamos solo la pagina principal
                int CantidadProductos = 0;
                TotalProductos = 0;
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                ProcesaSoloPorURL(_url, out htmlDocument, out CantidadProductos, idProceso);
                TotalProductos = TotalProductos + CantidadProductos;
                var _stats = Component.StatsComponent.ObtenerStats(idProceso);
                _stats.CantidadProductos = _stats.CantidadProductos + TotalProductos;
                Component.StatsComponent.ModificaStats(_stats);
                if (htmlDocument == null)
                {
                    CantidadProductos = 0;
                    return;
                }

                //Necesitamos buscar por cada sub-pagina
                var objPaginacion = htmlDocument.DocumentNode.SelectNodes(string.Format("//div[@id='paginador']"));
                if (objPaginacion == null)
                {
                    //Marcamos el log como error:
                    Entities.LogErrores _log = new Entities.LogErrores()
                    {
                        idProceso = idProceso,
                        idSistema = 1,
                        Observaciones = "ProcesaSubCategoriaURL: Error al obtener la paginación: " + _url.URL1
                    };
                    Component.ErrorLogComponent.InsertaLog(_log);
                    return;
                    //throw new Exception("Error al obtener la paginación");
                }
                HtmlAgilityPack.HtmlDocument htmlPaginacion = new HtmlAgilityPack.HtmlDocument();
                htmlPaginacion.LoadHtml(objPaginacion[0].InnerHtml);
                var objPaginas = htmlPaginacion.DocumentNode.SelectNodes(string.Format("//a[@rel='nofollow']"));
                int ultimaPagina;
                if (objPaginas == null)
                {
                    ultimaPagina = 0;
                }
                else
                {
                    ultimaPagina = Convert.ToInt32(objPaginas.Last().InnerHtml);
                }

                //Modificamos las páginas
                if (ultimaPagina == 0) ultimaPagina = 1;
                _url.CantPaginas = ultimaPagina;
                Component.ProductoComponent.ModificarURL(_url);

                for (int i = 0; i <= ultimaPagina - 1; i++)
                {
                    //Actualizamos la paginacion
                    objPaginacion = htmlDocument.DocumentNode.SelectNodes(string.Format("//div[@id='paginador']"));
                    if (objPaginacion == null)
                    {
                        //Marcamos el log como error:
                        Entities.LogErrores _log = new Entities.LogErrores()
                        {
                            idProceso = idProceso,
                            idSistema = 1,
                            Observaciones = "ProcesaSubCategoriaURL: Error al obtener la paginación: " + _url.URL1
                        };
                        Component.ErrorLogComponent.InsertaLog(_log);
                        return;
                    }
                    HtmlAgilityPack.HtmlDocument _htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(objPaginacion[0].InnerHtml);
                    objPaginas = htmlDocument.DocumentNode.SelectNodes(string.Format("//a[@rel='nofollow']"));
                    //La ir a la página 2, vamos al index = 0
                    int index = i;
                    if (i > 2) index = 2;

                    //buscamos en ésta URL
                    _url.URL1 = string.Empty;
                    try
                    {
                        _url.URL1 = "http://www.falabella.com" + objPaginas[index].OuterHtml.Substring(objPaginas[index].OuterHtml.IndexOf("href") + 6, objPaginas[index].OuterHtml.IndexOf(@""">") - objPaginas[index].OuterHtml.IndexOf("href") - 6);
                    }
                    catch (Exception)
                    {
                        //Ya terminó; hay que corregir.
                    }

                    if (_url.URL1.Length != 0)
                    {
                        if (_url.URL1 != null)
                        {
                            ProcesaSoloPorURL(_url, out htmlDocument, out CantidadProductos, idProceso);
                            TotalProductos = TotalProductos + CantidadProductos;
                            _stats.CantidadProductos = _stats.CantidadProductos + TotalProductos;
                            Component.StatsComponent.ModificaStats(_stats);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Marcamos el log como error:
                TotalProductos = 0;
                Entities.LogErrores _log = new Entities.LogErrores()
                {
                    idProceso = idProceso,
                    idSistema = 1,
                    Observaciones = "ProcesaSubCategoriaURL: Error al procesar la categoría URL: " + _url.URL1 + " " + ex.Message
                };
                Component.ErrorLogComponent.InsertaLog(_log);
                //throw new Exception("ProcesaSubCategoriaURL: " + ex.Message + " URL: " + _url.URL1);
            }

        }
        public static Entities.Falabella.BuscarResponse ProcesaPorURL(int idURL)
        {
            try
            {
                //Registramos el inicio
                var _stats = new Entities.Stats()
                {
                    Cargado = false,
                    IdURL = idURL,
                    FechaInicio = DateTime.Now,
                    CantidadProductos = 0,
                    Sistema = 1 //Falabella CL Retail
                };
                int idProceso;
                var stats = Component.StatsComponent.InsertaStats(_stats, out idProceso);
                _stats.Id = idProceso;
                int TotalProductos = 0;
                int CantidadProductos = 0;

                var _Caturl = Component.ProductoComponent.ObtenerURLporID(idURL);
                if (_Caturl == null)
                {
                    //Marcamos error
                    _stats.FechaFin = DateTime.Now;
                    _stats.CantidadProductos = _stats.CantidadProductos;
                    _stats.Cargado = false;
                    _stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
                    _stats.Observacion = "No existe la url con el id " + idURL;
                    Component.StatsComponent.ModificaStats(_stats);
                    return new Entities.Falabella.BuscarResponse()
                    {
                        Correcto = false,
                        FechaProceso = DateTime.Now,
                        Observaciones = _stats.Observacion,
                        Productos = null
                    };
                }
                //Primero vemos si es una URL con Sub-Categorías
                WebRequest req = HttpWebRequest.Create(_Caturl.URL1);
                req.Method = "GET";


                string source = string.Empty;
                try
                {
                    using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                    {
                        source = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    //Marcamos el log como error:
                    Entities.LogErrores _log = new Entities.LogErrores()
                    {
                        idProceso = idProceso,
                        idSistema = 1,
                        Observaciones = "Error de conexion. URL: " + _Caturl.URL1 + " " + ex.Message
                    };
                    Component.ErrorLogComponent.InsertaLog(_log);


                    //Marcamos error
                    _stats.FechaFin = DateTime.Now;
                    _stats.CantidadProductos = _stats.CantidadProductos;
                    _stats.Cargado = false;
                    _stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
                    _stats.Observacion = _log.Observaciones;
                    Component.StatsComponent.ModificaStats(_stats);
                    return new Entities.Falabella.BuscarResponse()
                    {
                        Correcto = false,
                        FechaProceso = DateTime.Now,
                        Observaciones = _stats.Observacion,
                        Productos = null
                    };
                }


                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(source);
                var subcat = htmlDocument.DocumentNode.SelectNodes(string.Format("//ul[@class='subCategorias']"));
                if (subcat != null)
                {
                    //Es una página que contiene categorías, entonces las leemos
                    HtmlAgilityPack.HtmlDocument _htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    _htmlDocument.LoadHtml(subcat.First().OuterHtml);
                    var lst = _htmlDocument.DocumentNode.SelectNodes(string.Format("//a"));
                    foreach (var item in lst)
                    {
                        //Obtenemos la URL
                        var _url = "http://www.falabella.com" + item.OuterHtml.Substring(item.OuterHtml.IndexOf("href") + 6, item.OuterHtml.IndexOf(@""">") - item.OuterHtml.IndexOf("href") - 6);
                        if (_url.Contains("?"))
                        {
                            _url = _url + "&Nrpp=999";
                        }
                        else
                        {
                            _url = _url + "?Nrpp=999";
                        }
                        var _objURL = new Entities.Falabella.URL()
                        {
                            URL1 = _url,
                            FechaStatus = DateTime.Now,
                            Correcto = true,
                            id = idURL
                        };
                        ProcesaSubCategoriaURL(_objURL, idProceso, out CantidadProductos);
                        TotalProductos = TotalProductos + CantidadProductos;
                    }

                }
                else
                {
                    //Ahora procesamos la SubCategoría
                    if (_Caturl.URL1.Contains("?"))
                    {
                        _Caturl.URL1 = _Caturl.URL1 + "&Nrpp=999";
                    }
                    else
                    {
                        _Caturl.URL1 = _Caturl.URL1 + "?Nrpp=999";
                    }

                    ProcesaSubCategoriaURL(_Caturl, idProceso, out CantidadProductos);
                    TotalProductos = TotalProductos + CantidadProductos;
                }



                //Registramos el final
                _stats.FechaFin = DateTime.Now;
                _stats.CantidadProductos = TotalProductos;
                _stats.Cargado = true;
                _stats.Duracion = _stats.FechaFin - _stats.FechaInicio;
                _stats.Observacion = "Categoría ejecutada correctamente. Productos cargados: " + _stats.CantidadProductos.ToString();
                Component.StatsComponent.ModificaStats(_stats);

                return new Entities.Falabella.BuscarResponse()
                {
                    Correcto = true,
                    FechaProceso = DateTime.Now,
                    Observaciones = "Lista De Productos Generada Correctamente",
                    Productos = null
                };
            }
            catch (Exception ex)
            {

                throw new Exception("ProcesaPorURL: " + ex.Message);
            }
        }
        public static List<Entities.Falabella.URL> ListarURL()
        {
            try
            {
                var _lst = Component.ProductoComponent.ListarURLs();
                return _lst;
            }
            catch (Exception ex)
            {
                throw new Exception("ListarURL: " + ex.Message);
            }
        }
        public static Entities.Porcentaje ObtenerPorcentaje(int idURL)
        {
            try
            {
                //Obtenemos el porcentaje solo en base a la última carga
                //Mejorar: estimar el porcentaje en base a la cantidad de producto según las páginas....
                //Perimero obtenemos los 5 últimos stats del idURL
                var _stats = Component.StatsComponent.ObtenerStatsPorURL(idURL);

                //De los 5, obtenemos el stats en proceso...
                var _statsSinPorcentaje = _stats.Where(x => x.Cargado == false).OrderByDescending(x => x.FechaInicio).FirstOrDefault();
                if (_statsSinPorcentaje == null)
                {
                    throw new Exception("No se pudo obtener el stats del proceso. Es posible que no exista ninguno en proceso.");
                }

                //De los 5, obtenemos el último cargado para calcular el porcentaje...
                var _statsUltimoCargado = _stats.Where(x => x.Cargado == true).OrderByDescending(x => x.FechaFin).FirstOrDefault();
                if (_statsUltimoCargado == null)
                {
                    throw new Exception("No se pudo obtener el stats para calcular el porcentaje. Es posible que no exista ninguno cargado correctamente.");
                }
                if (_statsUltimoCargado.CantidadProductos == 0)
                {
                    throw new Exception("El stats de la última carga, registra CantidadProductos = 0, verificar.");
                }

                //Ahora calculamos el porcentaje...
                decimal Porcentaje = (Convert.ToDecimal(_statsSinPorcentaje.CantidadProductos) * 100) / (Convert.ToDecimal(_statsUltimoCargado.CantidadProductos));

                var _return = new Entities.Porcentaje()
                {
                    Correcto = true,
                    Observaciones = "Porcentaje obtenido correctamente",
                    Valor = Porcentaje
                };
                return _return;
            }
            catch (Exception ex)
            {
                return new Entities.Porcentaje()
                {
                    Correcto = true,
                    Observaciones = "ObtenerPorcentaje: " + ex.Message,
                    Valor = 0
                };
            }
        }
        public static List<Entities.Stats> ObtenerStatsProcesados(int idPrimerProceso)
        {
            try
            {
                var _lst = Component.StatsComponent.ObtenerStatsProcesados(idPrimerProceso);
                return _lst;
            }
            catch (Exception ex)
            {
                throw new Exception("ObtenerStatsProcesados: " + ex.Message);
            }
        }
    }
}
