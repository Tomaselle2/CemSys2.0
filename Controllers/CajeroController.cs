using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Tarifaria;
using CemSys2.ViewModel.Cajero;
using CemSys2.ViewModel.ConcesionesViewModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class CajeroController : Controller
    {
        private readonly IFacturaBusiness _facturasBusiness;
        private readonly ITarifariaBusiness _tarifariaBusiness;
        private readonly IPdfService _pdfService;


        public CajeroController(IFacturaBusiness facturaBusiness, ITarifariaBusiness tarifariaBusiness, IPdfService pdfService)
        {
            _facturasBusiness = facturaBusiness;
            _tarifariaBusiness = tarifariaBusiness;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> FacturasPendientes()
        {
            FacturasEmitidasVM viewModel = new();
            try
            {
                viewModel.ListaFacturasEmitidas = await _facturasBusiness.ListaTotalFacturasEmitidasYPendientes();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "No se pudo cargar las facturas: " + ex.Message;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> FacturasCobradas(int pagina = 1, string desdeFecha = null, string hastaFecha = null)
        {
            FacturasCobradasVM viewModel = new();


            // Convertir las fechas de string a DateTime
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if (!string.IsNullOrEmpty(desdeFecha) && DateTime.TryParse(desdeFecha, out var tempDesde))
            {
                fechaDesde = tempDesde;
            }

            if (!string.IsNullOrEmpty(hastaFecha) && DateTime.TryParse(hastaFecha, out var tempHasta))
            {
                fechaHasta = tempHasta;
            }

            try
            {
                int registrosPorPagina = 10;
                var resultado = await _facturasBusiness.ListaTotalFacturasCobradas(pagina, registrosPorPagina, fechaDesde, fechaHasta);

                viewModel.ListaFacturasCobradas = resultado.Lista;
                viewModel.PaginaActual = pagina;
                viewModel.TotalRegistros = resultado.TotalRegistros;
                viewModel.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / registrosPorPagina);
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "No se pudo cargar las facturas: " + ex.Message;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> FacturasAnuladas(int pagina = 1, string desdeFecha = null, string hastaFecha = null)
        {
            FacturasAnuladasVM viewModel = new();

            // Convertir las fechas de string a DateTime
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if (!string.IsNullOrEmpty(desdeFecha) && DateTime.TryParse(desdeFecha, out var tempDesde))
            {
                fechaDesde = tempDesde;
            }

            if (!string.IsNullOrEmpty(hastaFecha) && DateTime.TryParse(hastaFecha, out var tempHasta))
            {
                fechaHasta = tempHasta;
            }

            try
            {
                int registrosPorPagina = 10;
                var resultado = await _facturasBusiness.ListaTotalFacturasAnuladas(pagina, registrosPorPagina, fechaDesde, fechaHasta);

                viewModel.ListaFacturasAnuladas = resultado.Lista;
                viewModel.PaginaActual = pagina;
                viewModel.TotalRegistros = resultado.TotalRegistros;
                viewModel.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / registrosPorPagina);
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "No se pudo cargar las facturas: " + ex.Message;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ProcesarFactura(int facturaId)
        {
            ProcesarFacturasVM viewModel = new();
            await LlenarListasProcesarFacturas(viewModel, facturaId);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarFactura(ProcesarFacturasVM viewModel) //cuando da click en cobrar factura
        {
            //ignorar validacion de Factura.Tramite
            ModelState.Remove("Factura.Tramite");

            if (!ModelState.IsValid)
            {
                await LlenarListasProcesarFacturas(viewModel, viewModel.FacturaId ?? 0);
                return View(viewModel);
            }

            try
            {
                await _facturasBusiness.VerificarCobrarFactura(new DTO.Factura.DTO_VerificarCobrarFactura
                {
                    FacturaId = viewModel.FacturaId ?? 0,
                    MetodoPagoId = viewModel.MetodoPagoId ?? 0,
                    EfectivoRecibido = viewModel.EfectivoRecibido ?? 0,
                    MontoTotal = viewModel.MontoTotal ?? 0,
                    TramiteId = viewModel.TramiteId ?? 0,
                    TipoTramiteId = viewModel.TipoTramiteId ?? 0,
                    CajeroId = HttpContext.Session.GetInt32("idUsuario").Value
                });
            }
            catch (ValidationException ex)
            {   
                ModelState.AddModelError(string.Empty, ex.Message);
                await LlenarListasProcesarFacturas(viewModel, viewModel.FacturaId ?? 0);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "No se pudo cobrar la factura: " + ex.Message);
                await LlenarListasProcesarFacturas(viewModel, viewModel.FacturaId ?? 0);
                return View(viewModel);
            }

            //paso el id de la factura a una vista aparte
            return RedirectToAction("CobroExitoso", new {facturaId = viewModel.FacturaId });
        }

        //para la pantalla de procesar factura
        private async Task LlenarListasProcesarFacturas(ProcesarFacturasVM viewModel, int facturaId)
        {
            try
            {
                // Cambiar el estado de la factura a "Pendiente de Cobro"
                await _facturasBusiness.PasarFacturaEstadoPendienteCobro(facturaId);

                viewModel.Factura = await _facturasBusiness.ConsultarFacturaPorId(facturaId);
                viewModel.ListaConceptosFactura = await _facturasBusiness.ListaConceptosFacturaPorFactura(facturaId);
                viewModel.PorcentajeFondo = await _tarifariaBusiness.ConsultarPorcentajeFondoActual();
                viewModel.ListaMetodoPago = await _facturasBusiness.ListaMetodoPago();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "No se pudo cargar la factura: " + ex.Message;
            }
        }

        //vista que muestra que el cobro fue exitoso
        public IActionResult CobroExitoso(int facturaId)
        {
            CobroExitosoVM viewModel = new()
            {
                FacturaId = facturaId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerHistorialFactura(int facturaId)
        {
            var historial = await _facturasBusiness.HistorialEstadoFacturaPorFacturaId(facturaId);

            if (historial == null || historial.Count == 0)
                return Json(new { success = false, mensaje = "No hay historial para esta factura." });

            var resultado = historial.Select(h => new
            {
                facturaId = h.FacturaId,
                estadoNombre = ((EstadosFactura)h.EstadoId).GetDisplayName(),
                fecha = h.FechaCambio.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            return Json(new { success = true, historial = resultado });
        }

        [HttpGet]
        public async Task<IActionResult> GenerarFactura(int facturaId)
        {
            FacturaPDF_VM viewModel = new FacturaPDF_VM();
            viewModel.baseUrl = $"{Request.Scheme}://{Request.Host}";

            try
            {
                viewModel.Factura = await _facturasBusiness.ConsultarFacturaPorId(facturaId);
                viewModel.ListaConceptosFactura = await _facturasBusiness.ListaConceptosFacturaPorFactura(facturaId);
                viewModel.PorcentajeFondo = await _tarifariaBusiness.ConsultarPorcentajeFondoActual();

                // Generar PDF con Puppeteer
                var pdfBytes = await _pdfService.GeneratePdfAsync("FacturaPDF", viewModel, HttpContext);

                return File(pdfBytes, "application/pdf", $"factura {facturaId}.pdf");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return BadRequest($"Error generando PDF: {ex.Message}");
            }


        }
    }
}
