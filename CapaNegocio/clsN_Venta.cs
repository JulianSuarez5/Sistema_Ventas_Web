using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace CapaNegocio
{
    public class clsN_Venta
    {
        private clsD_Venta objDatos = new clsD_Venta();
        public bool Registrar(clsVenta obj, DataTable DetalleVenta, out string Mensaje)
        {
            return objDatos.Registrar(obj, DetalleVenta, out Mensaje);
        }
        public List<clsDetalle_Venta> SalesList(int IdCliente)
        {
            return objDatos.SalesList(IdCliente);
        }
    }
}
