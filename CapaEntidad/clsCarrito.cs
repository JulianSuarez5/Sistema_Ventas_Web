namespace CapaEntidad
{
    public class clsCarrito
    {
        public int IdCarrito { get; set; }
        public clsCliente objCliente { get; set; }
        public clsProducto objProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
