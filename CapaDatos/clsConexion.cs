using System.Configuration;

namespace CapaDatos
{
    public class clsConexion
    {
        public static readonly string Cadena = ConfigurationManager.ConnectionStrings["CadenaConexion"].ToString();
    }
}
