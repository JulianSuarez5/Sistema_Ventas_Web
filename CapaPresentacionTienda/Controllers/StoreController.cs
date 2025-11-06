using CapaEntidad;
using CapaEntidad.PayPal;
using CapaNegocio;
using CapaPresentacionTienda.Filter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CapaPresentacionTienda.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductDetail(int idproducto = 0)
        {
            clsProducto objProducto = new clsN_Producto().Listar()
                .Where(p => p.IdProducto == idproducto).FirstOrDefault();

            // VALIDACIÓN IMPORTANTE:
            if (objProducto == null)
            {
                // Si no se encontró el producto, redirigir a la página principal.
                return RedirectToAction("Index", "Store");
            }

            bool convert;
            objProducto.Base64 = clsN_Recursos.convertirBase64(Path.Combine(objProducto.RutaImagen, objProducto.NombreImagen), out convert);
            objProducto.Extension = Path.GetExtension(objProducto.NombreImagen);

            return View(objProducto);
        }

        [HttpGet]
        public JsonResult ListCategories()
        {
            List<clsCategoria> categorias = new List<clsCategoria>();

            categorias = new clsN_Categoria().Listar();

            return Json(new { data = categorias }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListMarcaCategoria(int IdCategoria)
        {
            List<clsMarca> categorias = new List<clsMarca>();

            categorias = new clsN_Marca().ListarMarcaCategoria(IdCategoria);

            return Json(new { data = categorias }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListaDeProductos(int IdCategoria, int IdMarca)
        {
            List<clsProducto> productos = new List<clsProducto>();

            // bool convert; // Esta variable ya no será necesaria para este método

            productos = new clsN_Producto().Listar().Select(p => new clsProducto()
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                objMarca = p.objMarca,
                objCategoria = p.objCategoria,
                Precio = p.Precio,
                Stock = p.Stock,
                RutaImagen = p.RutaImagen, // <-- Se mantiene solo la URL completa de la imagen aquí
                // Base64 = clsN_Recursos.convertirBase64(Path.Combine(p.RutaImagen, p.NombreImagen), out convert),
                // Extension = Path.GetExtension(p.NombreImagen),
                Activo = p.Activo,
            }).Where(p => p.objCategoria.IdCategoria == (IdCategoria == 0 ? p.objCategoria.IdCategoria : IdCategoria) &&
                         p.objMarca.IdMarca == (IdMarca == 0 ? p.objMarca.IdMarca : IdMarca) &&
                         p.Stock > 0 && p.Activo == true
                         ).ToList();

            var jsonresult = Json(new { data = productos }, JsonRequestBehavior.AllowGet);

            jsonresult.MaxJsonLength = int.MaxValue; // Permitir respuestas JSON grandes

            return jsonresult;
        }
        [HttpPost]
        public JsonResult AddToCart(int idproducto)
        {
            int idcliente = ((clsCliente)Session["Cliente"]).IdCliente;
            bool existe = new clsN_Carrito().ExistsCart(idcliente, idproducto);
            bool respuesta = false;
            string mensaje = string.Empty;

            if (existe)
            {
                mensaje = "El producto ya está en el carrito.";
            }
            else
            {
                // Si no existe, lo agregamos al carrito.
                respuesta = new clsN_Carrito().OperacionDelCarrito(idcliente, idproducto, true, out mensaje);
            }
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public JsonResult AddToCart(int IdProducto)
        //{
        //    int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
        //    bool respuesta = false;
        //    string mensaje = string.Empty;

        //    respuesta = new clsN_Carrito().OperacionDelCarrito(IdCliente, IdProducto, true, out mensaje);

        //    // El SP ahora devuelve el mensaje correcto tanto para éxito como para error.
        //    return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult ListCart()
        {
            int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
            int Cantidad = new clsN_Carrito().CantidadEnCarrito(IdCliente);
            return Json(new { Cantidad = Cantidad }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListCartProducts()
        {
            int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
            List<clsCarrito> objLista = new List<clsCarrito>();
            bool convert;

            objLista = new clsN_Carrito().ProducsList(IdCliente).Select(p => new clsCarrito()
            {
                objProducto = new clsProducto()
                {
                    IdProducto = p.objProducto.IdProducto,
                    Nombre = p.objProducto.Nombre,
                    //Descripcion = p.objProducto.Descripcion,
                    Precio = p.objProducto.Precio,
                    //Stock = p.objProducto.Stock,
                    RutaImagen = p.objProducto.RutaImagen,
                    Base64 = clsN_Recursos.convertirBase64(Path.Combine(p.objProducto.RutaImagen, p.objProducto.NombreImagen), out convert),
                    Extension = Path.GetExtension(p.objProducto.NombreImagen),
                    //Activo = p.objProducto.Activo,
                    objMarca = p.objProducto.objMarca
                },
                Cantidad = p.Cantidad,
            }).ToList();

            return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int IdProducto)
        {
            int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new clsN_Carrito().OperacionDelCarrito(IdCliente, IdProducto, false, out mensaje);
            // El SP ahora devuelve el mensaje correcto tanto para éxito como para error.
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OperacionDelCarrito(int IdProducto, bool sumar)
        {
            int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;

            bool respuesta = false;

            string mensaje = string.Empty;

            respuesta = new clsN_Carrito().OperacionDelCarrito(IdCliente, IdProducto, sumar /* <- cambiar a true si no funciona*/, out mensaje);

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteCart(int IdProducto)
        {
            int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new clsN_Carrito().EliminarDelCarrito(IdCliente, IdProducto);

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetApartment()
        {
            List<clsDepartamento> objLista = new List<clsDepartamento>();

            objLista = new clsN_Ubicacion().GetApartment();

            return Json(new { lista = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMunicipality(string IdDepartamento)
        {
            _ = new List<clsMunicipio>();
            List<clsMunicipio> objLista = new clsN_Ubicacion().GetMunicipality(IdDepartamento);
            return Json(new { lista = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLocality(string IdDepartamento, string IdMunicipio)
        {
            List<clsLocalidad> objLista = new List<clsLocalidad>();
            objLista = new clsN_Ubicacion().GetLocality(IdDepartamento, IdMunicipio);
            return Json(new { lista = objLista }, JsonRequestBehavior.AllowGet);
        }

        [SessionValidate]
        [Authorize]
        public ActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Checkout(List<clsCarrito> objListCart, clsVenta objVenta)
        {
            decimal total = 0;
            DataTable detalledeventa = new DataTable();
            detalledeventa.Locale = new CultureInfo("es-CO");

            detalledeventa.Columns.Add("IdProducto", typeof(string));
            detalledeventa.Columns.Add("Cantidad", typeof(int));
            detalledeventa.Columns.Add("Precio", typeof(decimal));

            List<Item> objListaItem = new List<Item>();

            foreach (clsCarrito objCarrito in objListCart)
            {
                decimal subtotal = Convert.ToDecimal(objCarrito.Cantidad.ToString()) * objCarrito.objProducto.Precio;

                total += subtotal;

                objListaItem.Add(new Item()
                {
                    name = objCarrito.objProducto.Nombre,
                    quantity = objCarrito.Cantidad.ToString(),
                    unit_amount = new UnitAmount()
                    {
                        currency_code = "USD",
                        value = objCarrito.objProducto.Precio.ToString("0.00", CultureInfo.InvariantCulture)
                    }
                });

                detalledeventa.Rows.Add(new object[]
                {
                    objCarrito.objProducto.IdProducto,
                    objCarrito.Cantidad,
                    subtotal
                });
            }

            PurchaseUnit purchaseUnit = new PurchaseUnit()
            {
                amount = new Amount()
                {
                    currency_code = "USD",
                    value = total.ToString("0.00", CultureInfo.InvariantCulture),
                    breakdown = new Breakdown()
                    {
                        item_total = new ItemTotal()
                        {
                            currency_code = "USD",
                            value = total.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    }
                },
                description = "Compra de productos en la tienda",
                items = objListaItem
            };

            // Construcción de las URLs de forma dinámica
            string scheme = Request.Url.Scheme; // http o https
            string authority = Request.Url.Authority; // localhost:44396 o www.mitienda.com

            string returnUrl = $"{scheme}://{authority}{Url.Action("PagoRealizado", "Store")}";
            string cancelUrl = $"{scheme}://{authority}{Url.Action("Cart", "Store")}";

            cls_Checkout_Order objCheckoutOrder = new cls_Checkout_Order()
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit> { purchaseUnit },
                application_context = new ApplicationContext()
                {
                    brand_name = "AutomateHub.com",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            objVenta.MontoTotal = total;
            objVenta.IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
            TempData["Venta"] = objVenta; // Guardar la venta en TempData para usarla después
            TempData["DetalleVenta"] = detalledeventa; // Guardar el detalle de la venta en TempData temporalmentw

            clsN_PayPal objN_PayPal = new clsN_PayPal();
            cls_Response_PayPal<cls_Response_Checkout> response = new cls_Response_PayPal<cls_Response_Checkout>();
            response = await objN_PayPal.CreacionSolicitudPago(objCheckoutOrder);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [SessionValidate]
        [Authorize]
        public async Task<ActionResult> PagoRealizado()
        {
            string token = Request.QueryString["token"];

            clsN_PayPal objN_PayPal = new clsN_PayPal();

            cls_Response_PayPal<cls_Respose_Capture> response = new cls_Response_PayPal<cls_Respose_Capture>();

            response = await objN_PayPal.ObtenerPago(token);

            ViewData["Status"] = response.Status;

            if (response.Status)
            {
                clsVenta objVenta = (clsVenta)TempData["Venta"];
                DataTable detalledeventa = (DataTable)TempData["DetalleVenta"];
                objVenta.IdTransaccion = response.Response.purchase_units[0].payments.captures[0].id;
                string mensaje = string.Empty;
                bool respuesta = new clsN_Venta().Registrar(objVenta, detalledeventa, out mensaje);
                ViewData["IdTransaccion"] = objVenta.IdTransaccion;
            }
            return View();
        }

        [SessionValidate]
        [Authorize]
        public ActionResult MyPurchases()
        {
            int IdCliente = ((clsCliente)Session["Cliente"]).IdCliente;
            List<clsDetalle_Venta> objLista = new List<clsDetalle_Venta>();
            bool convert;

            objLista = new clsN_Venta().SalesList(IdCliente).Select(p => new clsDetalle_Venta()
            {
                objProducto = new clsProducto()
                {
                    //IdProducto = p.objProducto.IdProducto,
                    Nombre = p.objProducto.Nombre,
                    //Descripcion = p.objProducto.Descripcion,
                    Precio = p.objProducto.Precio,
                    //Stock = p.objProducto.Stock,
                    //RutaImagen = p.objProducto.RutaImagen,
                    Base64 = clsN_Recursos.convertirBase64(Path.Combine(p.objProducto.RutaImagen, p.objProducto.NombreImagen), out convert),
                    Extension = Path.GetExtension(p.objProducto.NombreImagen),
                    //Activo = p.objProducto.Activo,
                    //objMarca = p.objProducto.objMarca
                },
                Cantidad = p.Cantidad,
                Total = p.Total,
                IdTransaccion = p.IdTransaccion,

            }).ToList();

            return View(objLista);
        }
    }
}