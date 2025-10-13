using CemSys2.Interface.Facturas;
using CemSys2.Interface.Tarifaria;
using CemSys2.ViewModel.Cajero;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class CajeroController : Controller
    {
        private readonly IFacturaBusiness _facturasBusiness;
        private readonly ITarifariaBusiness _tarifariaBusiness;

        public CajeroController(IFacturaBusiness facturaBusiness, ITarifariaBusiness tarifariaBusiness)
        {
            _facturasBusiness = facturaBusiness;
            _tarifariaBusiness = tarifariaBusiness;
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

        public IActionResult FacturasCobradas()
        {
            return View();
        }

        public IActionResult FacturasAnuladas()
        {
            return View();
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

        public IActionResult CobroExitoso(int facturaId)
        {
            CobroExitosoVM viewModel = new()
            {
                FacturaId = facturaId
            };

            return View(viewModel);
        }
    }
}
