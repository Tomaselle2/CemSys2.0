using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using CemSys2.Interface.Introduccion;
using System.Threading.Tasks;
using CemSys2.Models;

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

        [HttpGet]
        public async Task<IActionResult> IntroduccionDifunto()
        {
            IntroduccionDifuntoVM viewModel = new();
            await CargarCombos(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IntroduccionDifunto(IntroduccionDifuntoVM viewModel)
        {

            if (viewModel.NN)
            {
                // Si es NN, limpiar DNI y Nombre para evitar conflictos
                viewModel.Dni = null;
                viewModel.Nombre = null;
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(viewModel);
                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        private async Task CargarCombos(IntroduccionDifuntoVM viewModel)
        {
            try
            {
                viewModel.ListaEstadoDifunto = await _introduccionBusiness.ListaEstadoDifunto();
                viewModel.ListaTipoParcela = await _introduccionBusiness.ListaTipoParcela();
                viewModel.ListaEmpresasSepelio = await _introduccionBusiness.ListaEmpresasFunebres();
                viewModel.ListaEmpleados = await _introduccionBusiness.ListaEmpleados();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "Error al cargar: " + ex.Message;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            var secciones = await _introduccionBusiness.ListaSecciones(tipoParcelaId);
            return Json(secciones);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParcelasPorSeccion(int seccionId, int estadoDifuntoId)
        {
            var parcelas = await _introduccionBusiness.ListaParcelas(seccionId, estadoDifuntoId);
            return Json(parcelas);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarEmpresa(string nombreEmpresa)
        {
            try
            {
                int idEmpresa = await _introduccionBusiness.RegistrarEmpresaSepelio(new EmpresaFunebre { Nombre = nombreEmpresa });
                return Json(new { success = true, idEmpresa = idEmpresa, message = "Empresa agregada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al agregar empresa: " + ex.Message });
            }
        }
    }
}
