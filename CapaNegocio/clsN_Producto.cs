using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Producto
    {
        private clsD_Producto objDatos = new clsD_Producto();

        public List<clsProducto> Listar()
        {
            return objDatos.Listar();
        }

        public int Registrar(clsProducto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "Ingrese el nombre de la Producto";
            }

            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "Ingrese la descripción de la Producto";
            }

            else if (obj.objMarca.IdMarca == 0)
            {
                Mensaje = "Debe seleccionar un marca";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "Ingrese el precio de la Producto";
            }
            else if (obj.objCategoria.IdCategoria == 0)
            {
                Mensaje = "Debe seleccionar una categoria";
            }
            else if (obj.Stock == 0)
            {
                Mensaje = "Ingrese el stock de la Producto";
            }


            if (string.IsNullOrEmpty(Mensaje))
            {
                return objDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }
        }
        public bool Actualizar(clsProducto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "Ingrese el nombre de la Producto";
            }

            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "Ingrese la descripción de la Producto";
            }

            else if (obj.objMarca.IdMarca == 0)
            {
                Mensaje = "Debe seleccionar un marca";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "Ingrese el precio de la Producto";
            }
            else if (obj.objCategoria.IdCategoria == 0)
            {
                Mensaje = "Debe seleccionar una categoria";
            }
            else if (obj.Stock == 0)
            {
                Mensaje = "Ingrese el stock de la Producto";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objDatos.Actualizar(obj, out Mensaje);
            }
            else
            {
                return false;
            }
        }


        public bool GuardarDatosImagen(clsProducto obj, out string Mensaje)
        {
            return objDatos.GuardarDatosImagen(obj, out Mensaje);
        }
        public bool Eliminar(int id, out string Mensaje)
        {
            return objDatos.Eliminar(id, out Mensaje);
        }
    }
}
