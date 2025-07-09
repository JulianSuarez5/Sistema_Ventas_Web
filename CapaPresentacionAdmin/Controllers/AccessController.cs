using CapaEntidad;
using CapaNegocio;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace CapaPresentacionAdmin.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Cambiar_Contra()
        {
            return View();
        }
        public ActionResult Restablecer_Contra()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Correo, string Clave)
        {
            clsUsuario objUser = new clsUsuario();

            objUser = new clsN_Usuarios().Listar().Where(u => u.Correo == Correo && u.Clave == clsN_Recursos.ConvertirSHA256(Clave)).FirstOrDefault();

            if (objUser == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }
            else
            {
                if (objUser.Restablecer)
                {
                    TempData["IdUser"] = objUser.IdUsuario;

                    return RedirectToAction("Cambiar_Contra");
                }

                FormsAuthentication.SetAuthCookie(objUser.Correo, false);


                ViewBag.Error = null;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Cambiar_Contra(string IdUser, string contra_actual, string nueva_contra, string confirm_clave)
        {
            clsUsuario objUser = new clsUsuario();

            objUser = new clsN_Usuarios().Listar().Where(u => u.IdUsuario == int.Parse(IdUser)).FirstOrDefault();



            if (objUser.Clave != clsN_Recursos.ConvertirSHA256(contra_actual))
            {
                TempData["IdUser"] = IdUser;
                ViewData["vContra"] = "";

                ViewBag.Error = "La contraseña actual es incorrecta";
                return View();
            }
            else if (nueva_contra != confirm_clave)
            {
                TempData["IdUser"] = IdUser;
                ViewData["vContra"] = contra_actual;

                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            ViewData["vContra"] = "";

            nueva_contra = clsN_Recursos.ConvertirSHA256(nueva_contra);

            string Mensaje = string.Empty;

            bool respuesta = new clsN_Usuarios().Cambiar_Contra(int.Parse(IdUser), nueva_contra, out Mensaje);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdUser"] = IdUser;
                ViewBag.Error = Mensaje;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Restablecer_Contra(string Correo)
        {
            clsUsuario objuser = new clsUsuario();

            objuser = new clsN_Usuarios().Listar().Where(item => item.Correo == Correo).FirstOrDefault();

            if (objuser == null)
            {
                ViewBag.Error = "El correo ingresado no está registrado o no se encontró un usuario con ese correo";
                return View();
            }

            string Mensaje = string.Empty;
            bool respuesta = new clsN_Usuarios().Restablecer_Contra(objuser.IdUsuario, Correo, out Mensaje);

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

        public ActionResult Salir()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Access");
        }
    }
}