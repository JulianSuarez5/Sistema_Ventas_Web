namespace CapaEntidad.PayPal
{
    public class cls_Response_PayPal<T>
    {
        public bool Status { get; set; }
        public T Response { get; set; }
    }
}
