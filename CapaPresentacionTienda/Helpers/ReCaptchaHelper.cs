using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace CapaPresentacionTienda.Helpers
{
    public static class ReCaptchaHelper
    {
        public static bool ValidateReCaptcha(string response)
        {
            try
            {
                string secretKey = ConfigurationManager.AppSettings["ReCaptcha:SecretKey"];

                if (string.IsNullOrEmpty(response))
                    return false;

                var request = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify");

                string postData = $"secret={secretKey}&response={response}";
                byte[] data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (WebResponse webResponse = request.GetResponse())
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    return result.success == true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}