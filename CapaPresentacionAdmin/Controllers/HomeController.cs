using CapaEntidad;
using CapaNegocio;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Globalization;
using ClosedXML.Excel;
using System.IO;

namespace CapaPresentacionAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Usuarios()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListarUsuario()
        {
            List<clsUsuario> objLista = new List<clsUsuario>();

            objLista = new clsN_Usuarios().Listar();

            return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarUsuario(clsUsuario objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdUsuario == 0)
            {
                resultado = new clsN_Usuarios().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new clsN_Usuarios().Actualizar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new clsN_Usuarios().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListaReporte(string fechainicio, string fechafin, string idtransaccion)
        {
            List<clsReporte> objLista = new List<clsReporte>();

            objLista = new clsN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            return Json(new { data = objLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VistaDashBoard()
        {
            clsDashBoard objeto = new clsN_Reporte().VerDaschBoard();

            return Json(new { resultado = objeto }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult Exportar_Venta(string fechainicio, string fechafin, string idtransaccion)
        {
            List<clsReporte> objLista = new List<clsReporte>();
            objLista = new clsN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            DataTable dtbl = new DataTable();

            dtbl.Locale = new CultureInfo("es-CO");
            dtbl.Columns.Add("Fecha Venta", typeof(string));
            dtbl.Columns.Add("Cliente", typeof(string));
            dtbl.Columns.Add("Producto", typeof(string));
            dtbl.Columns.Add("Precio", typeof(decimal));
            dtbl.Columns.Add("Cantidad", typeof(int));
            dtbl.Columns.Add("Total", typeof(decimal));
            dtbl.Columns.Add("Id Transacción", typeof(string));

            foreach (clsReporte report in objLista)
            {
                dtbl.Rows.Add(new object[]
                {
                    report.FechaVenta,
                    report.Cliente,
                    report.Producto,
                    report.Precio,
                    report.Cantidad,
                    report.Total,
                    report.IdTransaccion
                });
            }

            dtbl.TableName = "Datos";

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtbl);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    string nombreArchivo = "Reporte_Ventas_" + fechainicio + "_a_" + fechafin + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }
    }
}