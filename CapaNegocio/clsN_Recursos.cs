using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;


namespace CapaNegocio
{
    public class clsN_Recursos
    {

        public static string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 7);
            return clave;
        }

        //Encriptación de texto en SHA256
        public static string ConvertirSHA256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            //Se usa la referencia de System.Security.Cryptography
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool EnviarCorreo(string correo, string asunto, string mensaje)
        {
            bool resultado = false;

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(correo);
                mail.From = new MailAddress("juakosuarez12@gmail.com");
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                var smtp = new SmtpClient()
                {
                    Credentials = new NetworkCredential("juakosuarez12@gmail.com", "tfjy zxws yhsn vuem"),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true
                };

                smtp.Send(mail);
                resultado = true;
            }
            catch (Exception)
            {

                resultado = false;
            }
            return resultado;
        }

        public static string convertirBase64(string ruta, out bool convercion)
        {
            string textoBase64 = string.Empty;
            convercion = true;
            try
            {
                byte[] imagen = File.ReadAllBytes(ruta);
                textoBase64 = Convert.ToBase64String(imagen);
            }
            catch (Exception)
            {
                convercion = false;
            }
            return textoBase64;
        }
    }
}
