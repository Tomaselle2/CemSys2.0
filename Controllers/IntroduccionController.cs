using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using CemSys2.Interface.Introduccion;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class IntroduccionController : Controller
    {
        private readonly IIntroduccionBusiness _introduccionBusiness;

        public IntroduccionController(IIntroduccionBusiness introduccionBusiness)
        {
            _introduccionBusiness = introduccionBusiness;
        }

        public IActionResult Index(int pagina = 1)
        {
            IntroduccionIndexVM viewModelIndex = new IntroduccionIndexVM();
            return View(viewModelIndex);
        }

        public async Task<IActionResult> IntroduccionDifunto()
        {
            IntroduccionDifuntoVM viewModel = new();
            await CargarCombos(viewModel);

            return View(viewModel);
        }

        private async Task CargarCombos(IntroduccionDifuntoVM viewModel)
        {
            try
            {
                viewModel.ListaEstadoDifunto = await _introduccionBusiness.ListaEstadoDifunto();
                viewModel.ListaTipoParcela = await _introduccionBusiness.ListaTipoParcela();
            }
            catch(Exception ex)
            {
                viewModel.MensajeError = "Error al cargar los estados de difunto: " + ex.Message;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            var secciones = await _introduccionBusiness.ListaSecciones(tipoParcelaId);
            return Json(secciones);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParcelasPorSeccion(int seccionId)
        {
            var parcelas = await _introduccionBusiness.ListaParcelas(seccionId);
            return Json(parcelas);
        }
    }
}
