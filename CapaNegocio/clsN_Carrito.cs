using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Carrito
    {
        private clsD_Carrito objDatos = new clsD_Carrito();

        public bool ExistsCart(int idcliente, int idproducto)
        {
            return objDatos.ExistsCart(idcliente, idproducto);
        }
        public bool OperacionDelCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {
            return objDatos.OperacionDelCarrito(idcliente, idproducto, sumar, out Mensaje);
        }
        public int CantidadEnCarrito(int IdCliente)
        {
            return objDatos.CantidadEnCarrito(IdCliente);
        }
        public List<clsCarrito> ProducsList(int IdCliente)
        {
            return objDatos.ProducsList(IdCliente);
        }
        public bool EliminarDelCarrito(int IdCliente, int IdProducto)
        {
            return objDatos.EliminarDelCarrito(IdCliente, IdProducto);
        }
    }
}
