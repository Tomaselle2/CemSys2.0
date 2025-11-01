using CemSys2.Interface.Concesiones;
using CemSys2.Models;
using CemSys2.ViewModel.Reportes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IConcesionesBusiness _concesionesBusiness;

        public ReportesController(IConcesionesBusiness concesionesBusiness)
        {
            _concesionesBusiness = concesionesBusiness;
        }
        public IActionResult Index()
        {
            return View();
        }


        //reportes de conesiones
        [HttpGet]
        public async Task<IActionResult> ReporteConcesiones(ConcesionesReportesVM model)
        {
            //
            try
            {
                var concesiones = await _concesionesBusiness.ListaConcesionesReportes(model.FechaDesde, model.FechaHasta);
                model.ListaConcesiones = concesiones;
            }
            catch (Exception ex)
            {
                // Mensaje de error general
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "No se pudo obtener las concesiones: " + ex.Message;
                return View("VistaReportesConcesiones", model);
            }

            return View("VistaReportesConcesiones", model);
        }

        public IActionResult VistaReportesConcesiones()
        {
            ConcesionesReportesVM viewModel = new ConcesionesReportesVM();
            DateTime fechaHasta = DateTime.Today;
            DateTime fechaDesde = new DateTime(DateTime.Now.Year, 1, 1);

            viewModel.FechaDesde = fechaDesde;
            viewModel.FechaHasta = fechaHasta;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DescargarReporte(IFormFile image, DateTime realDesde, DateTime realHasta, string categoria, string chartType, string frecuencia, string tipoParcela)
        {
            try
            {
                // image: el gráfico como archivo (png)
                // realDesde/realHasta: las fechas reales encontradas en los registros
                // categoria: "Concesiones" o "ConcesionesPorTiempo"
                // chartType: "barras" o "torta"
                // frecuencia / tipoParcela: según desplegables

                // Ejemplo: guardar imagen temporalmente o procesar para PDF luego
                if (image != null && image.Length > 0)
                {
                    var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // aquí podés enviar filePath y demás parámetros a tu servicio de generación de PDF
                    // por ahora devolvemos OK
                }

                // devolver éxito
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
