using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class clsD_Ubicacion
    {
        public List<clsDepartamento> GetApartment()
        {
            List<clsDepartamento> Lista = new List<clsDepartamento>();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    string query = "SELECT * FROM DEPARTAMENTO";

                    SqlCommand cmd = new SqlCommand(query, objConexion)
                    {
                        CommandType = CommandType.Text
                    };

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new clsDepartamento()
                            {
                                IdDepartamento = dr["IdDepartamento"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<clsDepartamento>();
            }

            return Lista;
        }

        public List<clsMunicipio> GetMunicipality(string IdDepartamento)
        {
            List<clsMunicipio> Lista = new List<clsMunicipio>();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    string query = "SELECT * FROM MUNICIPIO WHERE IdDepartamento = @IdDepartamento";

                    SqlCommand cmd = new SqlCommand(query, objConexion);
                    cmd.Parameters.AddWithValue("@IdDepartamento", IdDepartamento);
                    cmd.CommandType = CommandType.Text;

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new clsMunicipio()
                            {
                                IdMunicipio = dr["IdMunicipio"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<clsMunicipio>();
            }

            return Lista;
        }

        public List<clsLocalidad> GetLocality(string IdDepartamento, string IdMunicipio)
        {
            List<clsLocalidad> Lista = new List<clsLocalidad>();

            try
            {
                using (SqlConnection objConexion = new SqlConnection(clsConexion.Cadena))
                {
                    string query = "SELECT * FROM LOCALIDAD WHERE IdMunicipio = @IdMunicipio AND IdDepartamento = @IdDepartamento";

                    SqlCommand cmd = new SqlCommand(query, objConexion);
                    cmd.Parameters.AddWithValue("@IdMunicipio", IdMunicipio);
                    cmd.Parameters.AddWithValue("@IdDepartamento", IdDepartamento);
                    cmd.CommandType = CommandType.Text;

                    objConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new clsLocalidad()
                            {
                                IdLocalidad = dr["IdLocalidad"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<clsLocalidad>();
            }

            return Lista;
        }
    }
}
