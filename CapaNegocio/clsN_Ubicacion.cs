using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Ubicacion
    {
        private clsD_Ubicacion objDatos = new clsD_Ubicacion();
        public List<clsDepartamento> GetApartment()
        {
            return objDatos.GetApartment();
        }
        public List<clsMunicipio> GetMunicipality(string IdDepartamento)
        {
            return objDatos.GetMunicipality(IdDepartamento);
        }
        public List<clsLocalidad> GetLocality(string IdDepartamento, string IdMunicipio)
        {
            return objDatos.GetLocality(IdDepartamento, IdMunicipio);
        }
    }
}
