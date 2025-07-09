using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class clsD_Clientes
    {
        public int Registrar(clsCliente obj, out string Mensaje)
        {
            int idAutoGenerado = 0;
            Mensaje = string.Empty;


            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {

                    SqlCommand cmd = new SqlCommand("USP_RegistrarClientes", objConexion);

                    cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Clave", obj.Clave);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    objConexion.Open();

                    cmd.ExecuteNonQuery();

                    idAutoGenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idAutoGenerado = 0;
                Mensaje = ex.Message;
            }

            return idAutoGenerado;
        }

        public List<clsCliente> Listar()
        {
            List<clsCliente> Lista = new List<clsCliente>();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    string query = "SELECT IdCliente, Nombres, Apellidos, Correo, Clave, Restablecer FROM CLIENTE";

                    SqlCommand cmd = new SqlCommand(query, objConexion)
                    {
                        CommandType = CommandType.Text
                    };

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new clsCliente()
                            {
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                Nombres = dr["Nombres"].ToString(),
                                Apellidos = dr["Apellidos"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Clave = dr["Clave"].ToString(),
                                Restablecer = Convert.ToBoolean(dr["Restablecer"]),
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<clsCliente>();
            }

            return Lista;
        }
        public bool Cambiar_Contra(int IdCliente, string Nueva_Clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE CLIENTE SET Clave = @Nueva_Clave, Restablecer = 0 WHERE IdCliente = @id", objConexion);

                    cmd.Parameters.AddWithValue("@id", IdCliente);
                    cmd.Parameters.AddWithValue("@Nueva_Clave", Nueva_Clave);
                    cmd.CommandType = CommandType.Text;

                    objConexion.Open();

                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool Restablecer_Contra(int IdCliente, string Clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE CLIENTE SET Clave = @Clave, Restablecer = 1 WHERE IdCliente = @id", objConexion);

                    cmd.Parameters.AddWithValue("@id", IdCliente);
                    cmd.Parameters.AddWithValue("@Clave", Clave);
                    cmd.CommandType = CommandType.Text;

                    objConexion.Open();

                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }
    }
}
