namespace CapaEntidad
{
    public class clsDetalle_Venta
    {
        public int IdDetalleVenta { get; set; }
        public int IdVenta { get; set; }
        public clsProducto objProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string IdTransaccion { get; set; }

    }
}
