using CemSys2.Interface;
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
        private readonly IPdfService _pdfService;

        public ReportesController(IConcesionesBusiness concesionesBusiness, IPdfService pdfService)
        {
            _concesionesBusiness = concesionesBusiness;
            _pdfService = pdfService;
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
        public async Task<IActionResult> DescargarReporteConcesion(
       [FromForm] string imageBase64,  // Cambiar de IFormFile a string
       [FromForm] string realDesde,
       [FromForm] string realHasta,
       [FromForm] string categoria,
       [FromForm] string chartType,
       [FromForm] string frecuencia,
       [FromForm] string tipoParcela)
        {
            try
            {
                DateTime desde = string.IsNullOrEmpty(realDesde) ? DateTime.MinValue : DateTime.Parse(realDesde);
                DateTime hasta = string.IsNullOrEmpty(realHasta) ? DateTime.MaxValue : DateTime.Parse(realHasta);

                // Procesar la imagen Base64
                string imageBase64ForPdf = "";
                if (!string.IsNullOrEmpty(imageBase64))
                {
                    imageBase64ForPdf = imageBase64;
                    Console.WriteLine($"Imagen Base64 recibida: {imageBase64.Length} caracteres");
                }

                // Crear el ViewModel para el PDF
                var viewModel = new ConcesionesReportePDFVM
                {
                    BaseUrl = $"{Request.Scheme}://{Request.Host}",
                    Categoria = categoria ?? "No especificada",
                    ChartType = ObtenerNombreTipoGrafico(chartType),
                    Frecuencia = ObtenerNombreFrecuencia(frecuencia),
                    TipoParcela = ObtenerNombreTipoParcela(tipoParcela),
                    FechaDesde = DateTime.Now.AddMonths(-1), // O usa las fechas reales si las tienes
                    FechaHasta = DateTime.Now,
                    RealDesde = desde,
                    RealHasta = hasta,
                    ImageBase64 = imageBase64ForPdf,
                    TituloReporte = GenerarTituloReporte(categoria, chartType)
                };

                Console.WriteLine($"Generando PDF con imagen: {!string.IsNullOrEmpty(imageBase64ForPdf)}");

                // Generar PDF
                var pdfBytes = await _pdfService.GeneratePdfAsync("ReporteConcesionesPDF", viewModel, HttpContext);

                // Devolver el PDF
                var fileName = $"reporte_{categoria?.ToLower() ?? "concesiones"}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DescargarReporteConcesion: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Métodos auxiliares para formatear los valores
        private string ObtenerNombreTipoGrafico(string chartType)
        {
            return chartType?.ToLower() switch
            {
                "barras" => "Gráfico de Barras",
                "torta" => "Gráfico de Torta",
                _ => chartType ?? "No especificado"
            };
        }

        private string ObtenerNombreFrecuencia(string frecuencia)
        {
            return frecuencia?.ToLower() switch
            {
                "mes" => "Mensual",
                "semana" => "Semanal",
                "dia" => "Diario",
                _ => frecuencia ?? "No especificada"
            };
        }

        private string ObtenerNombreTipoParcela(string tipoParcela)
        {
            return tipoParcela?.ToLower() switch
            {
                "1" => "Nicho",
                "2" => "Fosa",
                "todos" => "Todos los tipos",
                _ => tipoParcela ?? "No especificado"
            };
        }

        private string GenerarTituloReporte(string categoria, string chartType)
        {
            var titulo = "Reporte de ";

            if (!string.IsNullOrEmpty(categoria))
            {
                titulo += categoria;
            }
            else
            {
                titulo += "Concesiones";
            }

            if (!string.IsNullOrEmpty(chartType))
            {
                titulo += $" - {ObtenerNombreTipoGrafico(chartType)}";
            }

            return titulo;
        }


    }
}
