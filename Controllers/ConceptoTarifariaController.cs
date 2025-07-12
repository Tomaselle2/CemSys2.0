using Microsoft.AspNetCore.Mvc;
using CemSys2.Models;
using CemSys2.Interface;
using CemSys2.ViewModel;
using CemSys2.Business;
using CemSys2.Interface.Tarifaria;
namespace CemSys2.Controllers
{
    public class ConceptoTarifariaController : Controller
    {
        private readonly IRepositoryBusiness<ConceptosTarifaria> _conceptoTarifariaRepositoryBusiness;
        private readonly IRepositoryBusiness<TiposConceptoTarifarium> _tipoConceptoRepositoryBusiness;
        private readonly ITarifariaBusiness _tarifariaBusiness;

        public ConceptoTarifariaController(IRepositoryBusiness<ConceptosTarifaria> conceptoTarifariaRepositoryBusiness, IRepositoryBusiness<TiposConceptoTarifarium> tipoConceptoRepositoryBusiness, ITarifariaBusiness tarifariaBusiness)
        {
            _conceptoTarifariaRepositoryBusiness = conceptoTarifariaRepositoryBusiness;
            _tipoConceptoRepositoryBusiness = tipoConceptoRepositoryBusiness;
            _tarifariaBusiness = tarifariaBusiness;
        }

        private const int CANTIDAD_POR_PAGINA = 20;


        public async Task<IActionResult> Index(int pagina = 1)
        {
            if (HttpContext.Session.GetInt32("idUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Validar parámetros
            if (pagina < 1) pagina = 1;

            ConceptoTarifariaVM viewModel = new();

            await ListaConceptos(viewModel, pagina);

            return View(viewModel);
        }

        private async Task ListaConceptos(ConceptoTarifariaVM viewModel, int pagina = 1)
        {
            try
            {
                await CargarCombo(viewModel);
                // Obtener total de registros
                int totalRegistros = await _conceptoTarifariaRepositoryBusiness.ContarTotalAsync();
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.ListaConceptos = await _conceptoTarifariaRepositoryBusiness.ObtenerPaginadoAsync(pagina, CANTIDAD_POR_PAGINA);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
        }

        private async Task CargarCombo(ConceptoTarifariaVM model)
        {
            try
            {
                model.ListaTiposConcepto = await _tipoConceptoRepositoryBusiness.EmitirListado();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar los combos: {ex.Message}", ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(ConceptoTarifariaVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.EsEdicion) // Es una edición
                {
                    var concepto = await _conceptoTarifariaRepositoryBusiness.Consultar(model.Id.Value);
                    concepto.Nombre = model.Nombre.ToLower();
                    concepto.TipoConceptoId = model.TipoConceptoId.Value;

                    int modificacion = await _conceptoTarifariaRepositoryBusiness.Modificar(concepto);
                    TempData["MensajeExito"] = "Concepto actualizado correctamente";
                    model.EsEdicion = false; // Resetear el estado de edición
                }
                else // Es un registro nuevo
                {
                    ConceptosTarifaria  concepto = new()
                    {
                        Nombre = model.Nombre.ToLower(),
                        TipoConceptoId = model.TipoConceptoId.Value,
                        Visibilidad = true,
                    };

                    await _tarifariaBusiness.RegistrarConceptoTarifaria(concepto);
                    TempData["MensajeExito"] = "Concepto registrado correctamente";
                }
                return RedirectToAction("Index");
            }

            await ListaConceptos(model); // Vuelve a cargar listas si hay errores de validación
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(ConceptoTarifariaVM model)
        {
            try
            {
                ConceptosTarifaria modelo = await _conceptoTarifariaRepositoryBusiness.Consultar(model.Id.Value);
                modelo.Visibilidad = false;
                await _conceptoTarifariaRepositoryBusiness.Modificar(modelo);
                TempData["MensajeExito"] = "Concepto eliminado correctamente";
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
            return RedirectToAction(model.Redirigir);
        }

        [HttpGet]
        public async Task<IActionResult> Modificar(ConceptoTarifariaVM model)
        {
            ConceptosTarifaria concepto = await _conceptoTarifariaRepositoryBusiness.Consultar(model.Id.Value);
            model.Id = concepto.Id;
            model.Nombre = concepto.Nombre;
            model.TipoConceptoId = concepto.TipoConceptoId;

            await ListaConceptos(model);
            model.EsEdicion = true;
            ModelState.Clear();

            return View("Index", model);
        }
    }
}
