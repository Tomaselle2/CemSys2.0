using CemSys2.Interface.Facturas;
using CemSys2.Interface.Tarifaria;
using CemSys2.ViewModel.Cajero;
using Microsoft.AspNetCore.Mvc;
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

            try
            {
                // Cambiar el estado de la factura a "Pendiente de Cobro"
                await _facturasBusiness.PasarFacturaEstadoPendienteCobro(facturaId);

                viewModel.Factura = await _facturasBusiness.ConsultarFacturaPorId(facturaId);
                viewModel.ListaConceptosFactura = await _facturasBusiness.ListaConceptosFacturaPorFactura(facturaId);
                viewModel.PorcentajeFondo = await _tarifariaBusiness.ConsultarPorcentajeFondoActual();
                viewModel.ListaMetodoPago = await _facturasBusiness.ListaMetodoPago();
            }
            catch(Exception ex)
            {
                viewModel.MensajeError = "No se pudo cargar la factura: " + ex.Message;
            }

            return View(viewModel);
        }
    }
}
