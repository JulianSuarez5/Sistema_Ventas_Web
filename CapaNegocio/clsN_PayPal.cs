using CapaEntidad.PayPal;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class clsN_PayPal
    {
        private static string UrlPayPal = ConfigurationManager.AppSettings["UrlPaypal"];
        private static string Clientid = ConfigurationManager.AppSettings["ClientID"];
        private static string Secret = ConfigurationManager.AppSettings["Secret"];

        public async Task<cls_Response_PayPal<cls_Response_Checkout>> CreacionSolicitudPago(cls_Checkout_Order order)
        {
            cls_Response_PayPal<cls_Response_Checkout> response = new cls_Response_PayPal<cls_Response_Checkout>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlPayPal);
                    var byteArray = Encoding.ASCII.GetBytes($"{Clientid}:{Secret}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 

                    var jsonContent = JsonConvert.SerializeObject(order);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync("/v2/checkout/orders", content);
                    response.Status = httpResponse.IsSuccessStatusCode;

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                        cls_Response_Checkout checkoutResponse = JsonConvert.DeserializeObject<cls_Response_Checkout>(jsonResponse);
                        response.Response = checkoutResponse;
                    }
                    else
                    {
                        // *** Malparido error aparece o aparece ***
                        string errorResponse = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"PayPal API Error - Status Code: {httpResponse.StatusCode}");
                        Console.WriteLine($"PayPal API Error - Reason Phrase: {httpResponse.ReasonPhrase}");
                        Console.WriteLine($"PayPal API Error - Response Body: {errorResponse}");
                        // Para producción, podría usar una librería de logging como NLog o Serilog.
                        // ************************************************************
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Response = null;
                // También registra cualquier excepción que ocurra antes de la llamada HTTP
                Console.WriteLine($"Excepción en CreacionSolicitudPago: {ex.Message}");
            }
            return response;
        }
        public async Task<cls_Response_PayPal<cls_Respose_Capture>> ObtenerPago(string token)
        {
            cls_Response_PayPal<cls_Respose_Capture> response = new cls_Response_PayPal<cls_Respose_Capture>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlPayPal);
                var byteArray = Encoding.ASCII.GetBytes($"{Clientid}:{Secret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var data = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await client.PostAsync($"/v2/checkout/orders/{token}/capture", data);
                response.Status = httpResponse.IsSuccessStatusCode;

                if (httpResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    cls_Respose_Capture capture = JsonConvert.DeserializeObject<cls_Respose_Capture>(jsonResponse);
                    response.Response = capture;
                }
            }
            return response;
        }
    }
}
