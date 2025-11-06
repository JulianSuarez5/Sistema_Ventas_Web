using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
            try
            {
                List<clsProducto> objLista = new List<clsProducto>();
                objLista = new clsN_Producto().Listar();

                // Verificar configuración de Azure antes de usarla
                string azureStorageAccountName = ConfigurationManager.AppSettings["AzureStorageAccountName"];
                string azureStorageContainerName = ConfigurationManager.AppSettings["AzureStorageContainerName"];

                if (!string.IsNullOrEmpty(azureStorageAccountName) && !string.IsNullOrEmpty(azureStorageContainerName))
                {
                    string azureStorageContainerUrl = $"https://{azureStorageAccountName}.blob.core.windows.net/{azureStorageContainerName}/";

                    foreach (var producto in objLista)
                    {
                        if (!string.IsNullOrEmpty(producto.NombreImagen))
                        {
                            producto.RutaImagen = azureStorageContainerUrl + producto.NombreImagen;
                        }
                    }
                }

                return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al listar productos: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivoImagen)
        {
            string mensaje = string.Empty;
            bool operacion_exitosa = true;

            try
            {
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

                if (operacion_exitosa && archivoImagen != null)
                {
                    string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
                    string containerName = ConfigurationManager.AppSettings["AzureStorageContainerName"];

                    if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
                    {
                        mensaje = "Configuración de Azure Storage no encontrada";
                        operacion_exitosa = false;
                    }
                    else
                    {
                        string extension = Path.GetExtension(archivoImagen.FileName);
                        string nombre_imagen = string.Concat(objProducto.IdProducto.ToString(), extension);

                        try
                        {
                            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                            // Crear el contenedor si no existe
                            containerClient.CreateIfNotExists(PublicAccessType.Blob);

                            BlobClient blobClient = containerClient.GetBlobClient(nombre_imagen);

                            // Subir la imagen al blob
                            archivoImagen.InputStream.Position = 0; // Resetear la posición del stream
                            blobClient.Upload(archivoImagen.InputStream, true);

                            objProducto.RutaImagen = blobClient.Uri.ToString();
                            objProducto.NombreImagen = nombre_imagen;
                            new clsN_Producto().GuardarDatosImagen(objProducto, out mensaje);
                        }
                        catch (Exception ex)
                        {
                            mensaje = "Se guardó el producto pero hubo problemas con la imagen: " + ex.Message;
                        }
                    }
                }

                return Json(new { operacion_exitosa = operacion_exitosa, idGenerado = objProducto.IdProducto, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { operacion_exitosa = false, mensaje = "Error general: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ImagenProducto(int id)
        {
            try
            {
                clsProducto objProducto = new clsN_Producto().Listar().Where(p => p.IdProducto == id).FirstOrDefault();

                if (objProducto != null && !string.IsNullOrEmpty(objProducto.NombreImagen))
                {
                    string azureStorageAccountName = ConfigurationManager.AppSettings["AzureStorageAccountName"];
                    string azureStorageContainerName = ConfigurationManager.AppSettings["AzureStorageContainerName"];

                    if (!string.IsNullOrEmpty(azureStorageAccountName) && !string.IsNullOrEmpty(azureStorageContainerName))
                    {
                        string azureStorageBaseUrl = $"https://{azureStorageAccountName}.blob.core.windows.net/{azureStorageContainerName}/";
                        string imageUrl = azureStorageBaseUrl + objProducto.NombreImagen;

                        return Json(new
                        {
                            conversion = true,
                            imageUrl = imageUrl,
                            extension = Path.GetExtension(objProducto.NombreImagen)
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new
                {
                    conversion = false,
                    imageUrl = "",
                    extension = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    conversion = false,
                    imageUrl = "",
                    extension = "",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            try
            {
                // Obtener la información del producto para eliminar la imagen del blob
                clsProducto objProducto = new clsN_Producto().Listar().Where(p => p.IdProducto == id).FirstOrDefault();

                if (objProducto != null && !string.IsNullOrEmpty(objProducto.NombreImagen))
                {
                    string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
                    string containerName = ConfigurationManager.AppSettings["AzureStorageContainerName"];

                    if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(containerName))
                    {
                        try
                        {
                            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                            BlobClient blobClient = containerClient.GetBlobClient(objProducto.NombreImagen);

                            blobClient.DeleteIfExists();
                        }
                        catch (Exception ex)
                        {
                            // Log del error pero continúa con la eliminación del producto
                            System.Diagnostics.Debug.WriteLine($"Error al eliminar imagen de Azure Blob Storage: {ex.Message}");
                        }
                    }
                }

                respuesta = new clsN_Producto().Eliminar(id, out mensaje);
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar producto: " + ex.Message;
            }

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}