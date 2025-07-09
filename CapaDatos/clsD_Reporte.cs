using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CapaDatos
{
    public class clsD_Reporte
    {

        public List<clsReporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            List<clsReporte> Lista = new List<clsReporte>();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {

                    SqlCommand cmd = new SqlCommand("USP_ReporteVentas", objConexion);

                    cmd.Parameters.AddWithValue("FechaInicio", fechainicio);
                    cmd.Parameters.AddWithValue("FechaFin", fechafin);
                    cmd.Parameters.AddWithValue("IdTransaccion", idtransaccion);

                    cmd.CommandType = CommandType.StoredProcedure;

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new clsReporte()
                            {
                                FechaVenta = dr["FechaVenta"].ToString(),
                                Cliente = dr["Cliente"].ToString(),
                                Producto = dr["Producto"].ToString(),
                                Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-CO")),
                                Cantidad = Convert.ToInt32(dr["Cantidad"].ToString()),
                                Total = Convert.ToDecimal(dr["Total"], new CultureInfo("es-CO")),
                                IdTransaccion = dr["IdTransaccion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<clsReporte>();
            }

            return Lista;
        }

        public clsDashBoard VerDaschBoard()
        {
            clsDashBoard objeto = new clsDashBoard();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {

                    SqlCommand cmd = new SqlCommand("USP_ReporteDashboard", objConexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            objeto = new clsDashBoard();

                            objeto.TotalCliente = Convert.ToInt32(dr["TotalCliente"]);
                            objeto.TotalVenta = Convert.ToInt32(dr["TotalVenta"]);
                            objeto.TotalProducto = Convert.ToInt32(dr["TotalProducto"]);

                        }
                    }
                }
            }
            catch (Exception)
            {

                objeto = new clsDashBoard();
            }

            return objeto;
        }
    }
}
