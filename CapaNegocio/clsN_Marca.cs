using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Marca
    {
        private clsD_Marca objDatos = new clsD_Marca();

        public List<clsMarca> Listar()
        {
            return objDatos.Listar();
        }

        public int Registrar(clsMarca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "Ingrese la descripción de la marca";
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
        public bool Actualizar(clsMarca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "Ingrese la descripción de la marca";
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
        public bool Eliminar(int id, out string Mensaje)
        {
            return objDatos.Eliminar(id, out Mensaje);
        }

        public List<clsMarca> ListarMarcaCategoria(int IdCategoria)
        {
            return objDatos.ListarMarcaCategoria(IdCategoria);
        }
    }
}

