using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Clientes
    {
        private clsD_Clientes objDatos = new clsD_Clientes();

        public int Registrar(clsCliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "Ingrese el nombre del Cliente";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "Ingrese el apellido del Cliente";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "Ingrese el correo del Cliente";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                obj.Clave = clsN_Recursos.ConvertirSHA256(obj.Clave);
                return objDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }
        }
        public List<clsCliente> Listar()
        {
            return objDatos.Listar();
        }

        public bool Cambiar_Contra(int IdCliente, string Nueva_Clave, out string Mensaje)
        {
            return objDatos.Cambiar_Contra(IdCliente, Nueva_Clave, out Mensaje);
        }

        public bool Restablecer_Contra(int IdCliente, string Correo, out string Mensaje)
        {
            Mensaje = string.Empty;
            string Nueva_clave = clsN_Recursos.GenerarClave();
            bool result = objDatos.Restablecer_Contra(IdCliente, clsN_Recursos.ConvertirSHA256(Nueva_clave), out Mensaje);

            if (result)
            {
                string asunto = "Contraseña Restablecida";
                string mensaje_correo = "<h3>Su contraseña fue restablecida con éxito</h3></br><p>Su contraseña para acceder es: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", Nueva_clave);
                bool respuesta = clsN_Recursos.EnviarCorreo(Correo, asunto, mensaje_correo);

                if (respuesta)
                {
                    return true;
                }
                else
                {
                    Mensaje = "Error al enviar el correo";
                    return false;
                }
            }
            else
            {
                Mensaje = "Error al restablecer la contraseña";
                return false;
            }
        }
    }
}
