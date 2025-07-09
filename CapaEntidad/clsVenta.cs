using System.Collections.Generic;

namespace CapaEntidad
{
    public class clsVenta
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public int TotalProducto { get; set; }
        public decimal MontoTotal { get; set; }
        public string Contacto { get; set; }
        public string IdLocalidad { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string IdTransaccion { get; set; }
        public string FechaTexto { get; set; }
        //public DateTime FechaRegistro { get; set; }

        public List<clsDetalle_Venta> objDetalleVenta { get; set; }
    }
}
