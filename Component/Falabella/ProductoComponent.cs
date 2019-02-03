using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component
{
    public class ProductoComponent
    {
        public static List<Entities.Falabella.Producto> ListarProductos()
        {
            return new DataAccess.Falabella.ProductoData().ListarProductos();
        }

        public static bool InsertarProducto(Entities.Falabella.Producto _producto, out int idProducto)
        {
            return new DataAccess.Falabella.ProductoData().InsertarProducto(_producto, out idProducto);
        }

        public static bool ModificarProducto(Entities.Falabella.Producto _producto)
        {
            return new DataAccess.Falabella.ProductoData().ModificarProducto(_producto);
        }
        public static bool EliminaProductosRepetidos(int idProceso, string CodigoProducto)
        {
            return new DataAccess.Falabella.ProductoData().EliminaProductosRepetidos(idProceso, CodigoProducto);
        }
        public static int ProductosCargados(int idProceso)
        {
            return new DataAccess.Falabella.ProductoData().ProductosCargados(idProceso);
        }
        public static Entities.Falabella.Producto ObtenerProducto(string CodigoProducto)
        {
            return new DataAccess.Falabella.ProductoData().ObtenerProducto(CodigoProducto);
        }
        public static List<Entities.Falabella.Producto> ObtenerProductos(int idURL)
        {
            return new DataAccess.Falabella.ProductoData().ObtenerProductos(idURL);
        }

        public static List<Entities.Falabella.URL> ListarURLs()
        {
            return new DataAccess.Falabella.URLData().ListarURLs();
        }

        public static bool ModificarURL(Entities.Falabella.URL _url)
        {
            return new DataAccess.Falabella.URLData().ModificarURL(_url);
        }
        public static bool ModificarPagina(string _url, int paginas)
        {
            return new DataAccess.Falabella.URLData().ModificarPagina(_url, paginas);
        }
        public static bool InsertarURL(Entities.Falabella.URL _url)
        {
            return new DataAccess.Falabella.URLData().InsertarURL(_url);
        }
        public static Entities.Falabella.URL ObtenerURL(string URL)
        {
            return new DataAccess.Falabella.URLData().ObtenerURL(URL);
        }
        public static Entities.Falabella.URL ObtenerURLporID(int idURL)
        {
            return new DataAccess.Falabella.URLData().ObtenerURLporID(idURL);
        }
        public static List<Entities.Falabella.URL> ListarURLs(string URL)
        {
            return new DataAccess.Falabella.URLData().ListarURLs();
        }
    }
}
