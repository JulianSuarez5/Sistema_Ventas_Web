using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CapaDatos
{
    public class clsD_Carrito
    {
        public bool ExistsCart(int idcliente, int idproducto)
        {
            bool resultado = true;

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {

                    SqlCommand cmd = new SqlCommand("USP_EXISTSCART", objConexion);

                    cmd.Parameters.AddWithValue("IdCliente", idcliente);
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    objConexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
            }
            catch (Exception)
            {
                resultado = false;
            }

            return resultado;
        }

        public bool OperacionDelCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {
            bool resultado = true;
            Mensaje = string.Empty;


            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {

                    SqlCommand cmd = new SqlCommand("USP_OperacionDelCarrito", objConexion);

                    cmd.Parameters.AddWithValue("IdCliente", idcliente);
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);
                    cmd.Parameters.AddWithValue("Sumar", sumar);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    objConexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //SELECT COUNT (*) FROM CARRITO_COMPRAS WHERE IdCliente = 1
        public int CantidadEnCarrito(int IdCliente)
        {
            int resultado = 0;

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    // ISNULL se usa para devolver 0 si el carrito está vacío (SUM de nada es NULL).
                    string query = "SELECT ISNULL(SUM(Cantidad), 0) FROM CARRITO_COMPRAS WHERE IdCliente = @IdCliente";

                    SqlCommand cmd = new SqlCommand(query, objConexion);
                    cmd.Parameters.AddWithValue("@IdCliente", IdCliente);
                    cmd.CommandType = CommandType.Text;

                    objConexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }

            return resultado;
        }

        public List<clsCarrito> ProducsList(int IdCliente)
        {
            List<clsCarrito> Lista = new List<clsCarrito>();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    string query = "SELECT * FROM FN_ObtenerCarritoCliente (@IdCliente)";

                    SqlCommand cmd = new SqlCommand(query, objConexion);
                    cmd.Parameters.AddWithValue("@IdCliente", IdCliente);
                    cmd.CommandType = CommandType.Text;

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new clsCarrito()
                            {
                                objProducto = new clsProducto()
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    objMarca = new clsMarca()
                                    {
                                        Descripcion = dr["DescripcionMarca"].ToString()
                                    },
                                    Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-CO")),
                                    RutaImagen = dr["RutaImagen"].ToString(),
                                    NombreImagen = dr["NombreImagen"].ToString()
                                },
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<clsCarrito>();
            }

            return Lista;
        }

        public bool EliminarDelCarrito(int IdCliente, int IdProducto)
        {
            bool resultado = true;

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {

                    SqlCommand cmd = new SqlCommand("USP_EliminarCarrito", objConexion);

                    cmd.Parameters.AddWithValue("IdCliente", IdCliente);
                    cmd.Parameters.AddWithValue("IdProducto", IdProducto);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    objConexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
            }
            catch (Exception)
            {
                resultado = false;
            }

            return resultado;
        }
    }
}
