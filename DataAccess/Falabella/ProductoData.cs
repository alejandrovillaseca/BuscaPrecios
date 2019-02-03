using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Falabella
{
    public class ProductoData
    {
        public DataContext contextoProducto;

        public ProductoData()
        {
            contextoProducto = new DataContext();
        }

        public bool InsertarProducto(Entities.Falabella.Producto _producto, out int idProducto)
        {
            try
            {
                contextoProducto.FalabellaProducto.Add(new Models.FalabellaProducto()
                {
                    DescuentoCMR = _producto.DescuentoCMR,
                    Link = _producto.Link,
                    idURL = _producto.idURL,
                    CodigoProducto = _producto.CodigoProcudto,
                    Marca = _producto.Marca,
                    NombreProducto = _producto.NombreProducto,
                    Precio = _producto.Precio,
                    PrecioInternet = _producto.PrecioInternet,
                    PrecioNormal = _producto.PrecioNormal,
                    FechaProceso = DateTime.Now,
                    Observaciones = _producto.Observaciones,
                    Correcto = _producto.Correcto
                });

                contextoProducto.SaveChanges();
                idProducto = contextoProducto.FalabellaProducto.OrderByDescending(u => u.id).First().id;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertarProducto: " + ex.Message);
            }
        }

        public List<Entities.Falabella.Producto> ListarProductos()
        {
            try
            {
                var _obj = (from d in contextoProducto.FalabellaProducto
                            select new Entities.Falabella.Producto
                            {
                                DescuentoCMR = d.DescuentoCMR,
                                PrecioNormal = d.PrecioNormal,
                                CodigoProcudto = d.CodigoProducto,
                                PrecioInternet = d.PrecioInternet,
                                Link = d.Link,
                                idURL = d.idURL,
                                Marca = d.Marca,
                                NombreProducto = d.NombreProducto,
                                Precio = d.Precio,
                                Correcto = d.Correcto
                            }).ToList();
                return _obj;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertarProducto: " + ex.Message);
            }
        }

        public Entities.Falabella.Producto ObtenerProducto(string CodigoProducto)
        {
            var _obj = (from d in contextoProducto.FalabellaProducto
                        where d.CodigoProducto == CodigoProducto
                        select new Entities.Falabella.Producto
                        {
                            DescuentoCMR = d.DescuentoCMR,
                            NombreProducto = d.NombreProducto,
                            CodigoProcudto = d.CodigoProducto,
                            Link = d.Link,
                            idURL = d.idURL,
                            Marca = d.Marca,
                            Precio = d.Precio,
                            PrecioInternet = d.PrecioInternet,
                            PrecioNormal = d.PrecioNormal,
                            Correcto = d.Correcto
                        }).FirstOrDefault();
            return _obj;
        }
        public List<Entities.Falabella.Producto> ObtenerProductos(int idURL)
        {
            var _obj = (from d in contextoProducto.FalabellaProducto
                        where d.idURL == idURL
                        select new Entities.Falabella.Producto
                        {
                            DescuentoCMR = d.DescuentoCMR,
                            NombreProducto = d.NombreProducto,
                            CodigoProcudto = d.CodigoProducto,
                            Link = d.Link,
                            idURL = d.idURL,
                            Marca = d.Marca,
                            Precio = d.Precio,
                            PrecioInternet = d.PrecioInternet,
                            PrecioNormal = d.PrecioNormal,
                            Correcto = d.Correcto
                        }).ToList();
            return _obj;
        }
        public bool ModificarProducto(Entities.Falabella.Producto _producto)
        {
            try
            {
                var _obj = contextoProducto.FalabellaProducto.First(x => x.id == _producto.id);

                _obj.Link = _producto.Link;
                _obj.idURL = _producto.idURL;
                _obj.Marca = _producto.Marca;
                _obj.CodigoProducto = _producto.CodigoProcudto;
                _obj.Precio = _producto.Precio;
                _obj.NombreProducto = _producto.NombreProducto;
                _obj.PrecioInternet = _producto.PrecioInternet;
                _obj.PrecioNormal = _producto.PrecioNormal;
                _obj.DescuentoCMR = _producto.DescuentoCMR;
                _obj.Observaciones = _producto.Observaciones;
                _obj.FechaProceso = _producto.FechaProceso;
                _obj.Correcto = _producto.Correcto;
                _obj.idProceso = _producto.idProceso;
                contextoProducto.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ModificarProducto: " + ex.Message);
            }
        }
        public bool EliminaProductosRepetidos(int idProceso, string CodigoProducto)
        {
            try
            {
                var _obj = contextoProducto.FalabellaProducto.Where(x => x.idProceso == idProceso && x.CodigoProducto == CodigoProducto).OrderByDescending(x => x.FechaProceso).ToList();
                if (_obj == null || _obj.Count < 2) return false;
                contextoProducto.FalabellaProducto.Remove(_obj[0]);
                contextoProducto.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public int ProductosCargados(int idProceso)
        {
            try
            {
                var _obj = (from d in contextoProducto.FalabellaProducto
                            where d.idProceso == idProceso
                            && d.Correcto == true
                            select d.id).Count();
                return _obj;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertarProducto: " + ex.Message);
            }
        }
    }
}
