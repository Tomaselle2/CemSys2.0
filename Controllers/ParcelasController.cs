using CemSys2.DTO;
using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class ParcelasController : Controller
    {
        private readonly IParcelasBusiness _parcelasBusiness;

        public ParcelasController(IParcelasBusiness parcelasBusiness)
        {
            _parcelasBusiness = parcelasBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RegistrarParcelas(DTO_secciones seccion)
        {
            try
            {
                await _parcelasBusiness.RegistrarParcelas(seccion);
                return RedirectToAction(seccion.Redirigir, "Secciones");
            }
            catch (Exception ex)
            {
                return View(seccion.Redirigir, "Secciones");
            }

        }

        //tabla contrato de concesion, parcela difunto -> si fecharetiro es null quiere decir que esta vigente
        //parcela difunto, include a difunto
        //tramites 
        public async Task<IActionResult> HistorialParcela(int parcelaId)
        {
 
            ParcelaHistorialVM viewModel = new ParcelaHistorialVM();
            try
            {
                viewModel.ListaDifuntosActuales = await _parcelasBusiness.ListaHistorialDifuntosActuales(parcelaId);
                viewModel.EncabezadoParcela = await _parcelasBusiness.EncabezadoParcela(parcelaId);
                viewModel.ListaDifuntosHistoricos = await _parcelasBusiness.ListaHistorialDifuntosHistoricos(parcelaId);
                viewModel.ListaTramites = await _parcelasBusiness.ListaParcelasTramites(parcelaId);
                viewModel.ListaTiposNicho = await _parcelasBusiness.ListaTiposNicho();
                viewModel.ListaTiposPanteon = await _parcelasBusiness.ListaTipoPanteon();
            }
            catch (Exception ex)
            {
                // Manejar excepciones y retornar un mensaje de error
                viewModel.MensajeError = $"Error al obtener el historial de la parcela: {ex.Message}";
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ParcelaHistorialVM viewModel)
        {
            Parcela parcela = await _parcelasBusiness.BuscarParcelaPorId(viewModel.EncabezadoParcela.ParcelaId);
            if (viewModel.EncabezadoParcela.TipoParcela == 1)
            {
                parcela.TipoNicho = viewModel.EncabezadoParcela.TipoNicho;
            }

            if (viewModel.EncabezadoParcela.TipoParcela == 3)
            {
                parcela.TipoPanteonId = viewModel.EncabezadoParcela.TipoPanteon;
                parcela.NombrePanteon = viewModel.EncabezadoParcela.NombrePanteon;
            }

            try
            {
                int modificado = await _parcelasBusiness.ModificarParcela(parcela);
                TempData["MensajeExito"] = "modificación exitosa";
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al modificar: {ex.Message}";
            }
            return RedirectToAction("HistorialParcela", new {parcelaId = viewModel.EncabezadoParcela.ParcelaId});
        }
    }
}
