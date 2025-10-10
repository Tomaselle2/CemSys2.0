using CemSys2.Interface.Facturas;
using CemSys2.ViewModel.Cajero;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class CajeroController : Controller
    {
        private readonly IFacturaBusiness _facturasBusiness;

        public CajeroController(IFacturaBusiness facturaBusiness)
        {
            _facturasBusiness = facturaBusiness;
        }

        public async Task<IActionResult> FacturasPendientes()
        {
            FacturasEmitidasVM viewModel = new();
            try
            {
                viewModel.ListaFacturasEmitidas = await _facturasBusiness.ListaTotalFacturasEmitidasYPendientes();
            }
            catch (Exception ex) {
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
    }
}
