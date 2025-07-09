using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacionAdmin.Controllers
{
    [Authorize]
    public class MantenedorController : Controller
    {
        // GET: Mantenedor
        public ActionResult Categoria()
        {
            return View();
        }

        public ActionResult Marca()
        {
            return View();
        }

        public ActionResult Producto()
        {
            return View();
        }

        //-----------------------MÉTODOS PARA LAS CATEGORÍAS--------------------------//
        #region [CATEGORIA]
        public JsonResult ListarCategorias()
        {
            List<clsCategoria> objLista = new List<clsCategoria>();

            objLista = new clsN_Categoria().Listar();

            return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarCategoria(clsCategoria objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdCategoria == 0)
            {
                resultado = new clsN_Categoria().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new clsN_Categoria().Actualizar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarCategoria(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new clsN_Categoria().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //-----------------------MÉTODOS PARA LAS MARCAS--------------------------//
        #region [MARCA]
        public JsonResult ListarMarca()
        {
            List<clsMarca> objLista = new List<clsMarca>();

            objLista = new clsN_Marca().Listar();

            return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarMarca(clsMarca objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdMarca == 0)
            {
                resultado = new clsN_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new clsN_Marca().Actualizar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new clsN_Marca().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //-----------------------MÉTODOS PARA LOS PRODUCTOS--------------------------//
        #region[PRODUCTOS]
        public JsonResult ListarProducto()
        {
            List<clsProducto> objLista = new List<clsProducto>();

            objLista = new clsN_Producto().Listar();

            return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivoImagen)
        {
            string mensaje = string.Empty;
            bool operacion_exitosa = true;
            bool guardar_imagen_exito = true;

            clsProducto objProducto = new clsProducto();
            objProducto = JsonConvert.DeserializeObject<clsProducto>(objeto);

            decimal precio;

            if (decimal.TryParse(objProducto.PrecioTexto, NumberStyles.AllowDecimalPoint, new CultureInfo("es-CO"), out precio))
            {
                objProducto.Precio = precio;
            }
            else
            {
                return Json(new { operacion_exitosa = false, mensaje = "El formato del precio debe ser con enteros sin puntos ni comas." }, JsonRequestBehavior.AllowGet);
            }


            if (objProducto.IdProducto == 0)
            {
                int idproductogenerado = new clsN_Producto().Registrar(objProducto, out mensaje);

                if (idproductogenerado != 0)
                {
                    objProducto.IdProducto = idproductogenerado;
                }
                else
                {
                    operacion_exitosa = false;
                }
            }
            else
            {
                operacion_exitosa = new clsN_Producto().Actualizar(objProducto, out mensaje);
            }

            if (operacion_exitosa)
            {
                if (archivoImagen != null)
                {
                    string ruta_guardar = ConfigurationManager.AppSettings["ServidorDeFotos"].ToString();
                    string extension = Path.GetExtension(archivoImagen.FileName);
                    string nombre_imagen = string.Concat(objProducto.IdProducto.ToString(), extension);


                    try
                    {
                        archivoImagen.SaveAs(Path.Combine(ruta_guardar, nombre_imagen));
                    }
                    catch (Exception ex)
                    {
                        string mensaje_error = ex.Message;
                        guardar_imagen_exito = false;
                    }


                    if (guardar_imagen_exito)
                    {
                        objProducto.RutaImagen = ruta_guardar;
                        objProducto.NombreImagen = nombre_imagen;
                        bool respuesta = new clsN_Producto().GuardarDatosImagen(objProducto, out mensaje);
                    }
                    else
                    {
                        mensaje = "Se guardó el producto pero hubo problemas con la imagen.";
                    }
                }
            }


            return Json(new { operacion_exitosa = operacion_exitosa, idGenerado = objProducto.IdProducto, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImagenProducto(int id)
        {
            bool conversion;
            clsProducto objProducto = new clsN_Producto().Listar().Where(p => p.IdProducto == id).FirstOrDefault();

            string textoBase64 = clsN_Recursos.convertirBase64(Path.Combine(objProducto.RutaImagen, objProducto.NombreImagen), out conversion);

            return Json(new
            {
                conversion = conversion,
                textoBase64 = textoBase64,
                extension = Path.GetExtension(objProducto.NombreImagen)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new clsN_Producto().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}