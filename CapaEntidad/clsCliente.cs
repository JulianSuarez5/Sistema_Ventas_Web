namespace CapaEntidad
{
    public class clsCliente
    {
        public int IdCliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string Confirm_Clave { get; set; }
        public bool Restablecer { get; set; }
        //public DateTime FechaRegistro { get; set; } 
    }
}
