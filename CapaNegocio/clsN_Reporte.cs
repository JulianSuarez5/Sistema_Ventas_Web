using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class clsN_Reporte
    {
        private clsD_Reporte objDatos = new clsD_Reporte();

        public List<clsReporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            return objDatos.Ventas(fechainicio, fechafin, idtransaccion);
        }
        public clsDashBoard VerDaschBoard()
        {
            return objDatos.VerDaschBoard();
        }
    }
}
