using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Usuarios
    {
        private clsD_Usuarios objDatos = new clsD_Usuarios();

        public List<clsUsuario> Listar()
        {
            return objDatos.Listar();
        }

        public int Registrar(clsUsuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "Ingrese el nombre del usuario";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "Ingrese el apellido del usuario";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "Ingrese el correo del usuario";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                string clave = clsN_Recursos.GenerarClave();
                string asunto = "Creación de cuenta";
                string mensaje_correo = "<h3>Su cuenta fue creada con éxito</h3></br><p>Su contraseña para acceder es: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", clave);

                bool respuesta = clsN_Recursos.EnviarCorreo(obj.Correo, asunto, mensaje_correo);

                if (respuesta)
                {
                    obj.Clave = clsN_Recursos.ConvertirSHA256(clave);
                    return objDatos.Registrar(obj, out Mensaje);
                }
                else
                {
                    Mensaje = "Error al enviar el correo";
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public bool Actualizar(clsUsuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "Ingrese el nombre del usuario";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "Ingrese el apellido del usuario";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "Ingrese el correo del usuario";
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

        public bool Cambiar_Contra(int IdUsuario, string Nueva_Clave, out string Mensaje)
        {
            return objDatos.Cambiar_Contra(IdUsuario, Nueva_Clave, out Mensaje);
        }

        public bool Restablecer_Contra(int IdUsuario, string Correo, out string Mensaje)
        {
            Mensaje = string.Empty;
            string Nueva_clave = clsN_Recursos.GenerarClave();
            bool result = objDatos.Restablecer_Contra(IdUsuario, clsN_Recursos.ConvertirSHA256(Nueva_clave), out Mensaje);

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
