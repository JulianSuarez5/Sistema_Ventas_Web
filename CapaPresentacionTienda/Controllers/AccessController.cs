using CapaEntidad;
using CapaNegocio;
using CapaPresentacionTienda.Helpers;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace CapaPresentacionTienda.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access
        public ActionResult Index()
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
            return View();
        }

        public ActionResult Registrar()
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
            return View();
        }

        public ActionResult Restablecer_Contra()
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
            return View();
        }

        public ActionResult Cambiar_Contra()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(clsCliente obj)
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

            int Resultado;
            string Mensaje = string.Empty;

            ViewData["Nombres"] = string.IsNullOrEmpty(obj.Nombres) ? "" : obj.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(obj.Apellidos) ? "" : obj.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(obj.Correo) ? "" : obj.Correo;

            // Validar reCAPTCHA
            string captchaResponse = Request.Form["g-recaptcha-response"];
            if (!ReCaptchaHelper.ValidateReCaptcha(captchaResponse))
            {
                ViewBag.Error = "Por favor, completa la verificación de CAPTCHA.";
                return View();
            }

            if (obj.Clave != obj.Confirm_Clave)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            Resultado = new clsN_Clientes().Registrar(obj, out Mensaje);

            if (Resultado > 0)
            {
                ViewBag.Mensaje = null;
                return RedirectToAction("Index", "Access");
            }
            else
            {
                ViewBag.Error = Mensaje;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(string Correo, string Clave)
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

            // Validar reCAPTCHA primero
            string captchaResponse = Request.Form["g-recaptcha-response"];
            if (!ReCaptchaHelper.ValidateReCaptcha(captchaResponse))
            {
                ViewBag.Error = "Por favor, completa la verificación de CAPTCHA.";
                return View();
            }

            clsCliente objCliente = null;

            objCliente = new clsN_Clientes().Listar().Where(item => item.Correo == Correo && item.Clave == clsN_Recursos.ConvertirSHA256(Clave)).FirstOrDefault();

            if (objCliente == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }
            else
            {
                if (objCliente.Restablecer)
                {
                    TempData["IdCliente"] = objCliente.IdCliente;
                    return RedirectToAction("Cambiar_Contra", "Access");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(objCliente.Correo, false);
                    Session["Cliente"] = objCliente;
                    ViewBag.Error = null;
                    return RedirectToAction("Index", "Store");
                }
            }
        }

        [HttpPost]
        public ActionResult Restablecer_Contra(string Correo)
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

            // Validar reCAPTCHA
            string captchaResponse = Request.Form["g-recaptcha-response"];
            if (!ReCaptchaHelper.ValidateReCaptcha(captchaResponse))
            {
                ViewBag.Error = "Por favor, completa la verificación de CAPTCHA.";
                return View();
            }

            clsCliente objclient = new clsCliente();
            objclient = new clsN_Clientes().Listar().Where(item => item.Correo == Correo).FirstOrDefault();

            if (objclient == null)
            {
                ViewBag.Error = "El correo ingresado no está registrado o no se encontró un usuario con ese correo";
                return View();
            }

            string Mensaje = string.Empty;
            bool respuesta = new clsN_Clientes().Restablecer_Contra(objclient.IdCliente, Correo, out Mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Access");
            }
            else
            {
                ViewBag.Error = Mensaje;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Cambiar_Contra(string IdCliente, string contra_actual, string nueva_contra, string confirm_clave)
        {
            clsCliente objclient = new clsCliente();
            objclient = new clsN_Clientes().Listar().Where(u => u.IdCliente == int.Parse(IdCliente)).FirstOrDefault();

            if (objclient.Clave != clsN_Recursos.ConvertirSHA256(contra_actual))
            {
                TempData["IdCliente"] = IdCliente;
                ViewData["vContra"] = "";
                ViewBag.Error = "La contraseña actual es incorrecta";
                return View();
            }
            else if (nueva_contra != confirm_clave)
            {
                TempData["IdCliente"] = IdCliente;
                ViewData["vContra"] = contra_actual;
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            ViewData["vContra"] = "";
            nueva_contra = clsN_Recursos.ConvertirSHA256(nueva_contra);
            string Mensaje = string.Empty;
            bool respuesta = new clsN_Clientes().Cambiar_Contra(int.Parse(IdCliente), nueva_contra, out Mensaje);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdCliente"] = IdCliente;
                ViewBag.Error = Mensaje;
                return View();
            }
        }

        public ActionResult Salir()
        {
            FormsAuthentication.SignOut();
            Session["Cliente"] = null;
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Access");
        }
    }
}