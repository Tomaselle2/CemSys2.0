using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace CemSys2.Controllers
{
    public class TipoTramiteController : Controller
    {
        private readonly IRepositoryBusiness<TipoTramite> _tipoTramiteRepositoryBusiness;
        public TipoTramiteController(IRepositoryBusiness<TipoTramite> tipoTramiteRepositoryBusiness)
        {
            _tipoTramiteRepositoryBusiness = tipoTramiteRepositoryBusiness;
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

            TipoTramiteViewModel viewModel = new();

            await ListaTipoTramites(viewModel, pagina);

            return View(viewModel);
        }

        private async Task ListaTipoTramites(TipoTramiteViewModel viewModel, int pagina = 1)
        {
            try
            {
                // Definir filtro una sola vez
                //Expression<Func<Usuario, bool>> filtro = s => s.Visibilidad == true;
                //Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = q => q.OrderByDescending(s => s.Id);

                // Obtener total de registros
                int totalRegistros = await _tipoTramiteRepositoryBusiness.ContarTotalAsync();
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.ListaTipoTramite = await _tipoTramiteRepositoryBusiness.ObtenerPaginadoAsync(pagina, CANTIDAD_POR_PAGINA);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(TipoTramiteViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.EsEdicion) // Es una edición
                {
                    // Cargar el usuario existente para obtener la contraseña actual si no se cambió
                    var tipoTramite = await _tipoTramiteRepositoryBusiness.Consultar(model.Id.Value);
                    tipoTramite.Tipo = model.Nombre.ToLower();

                    int modificacion = await _tipoTramiteRepositoryBusiness.Modificar(tipoTramite);
                    TempData["MensajeExito"] = "Tipo de trámite actualizado correctamente";
                    model.EsEdicion = false; // Resetear el estado de edición
                }
                else // Es un registro nuevo
                {
                    TipoTramite tipoTramite = new()
                    {
                        Id = model.Id ?? 0, // Si es nuevo, Id será 0
                        Tipo = model.Nombre.ToLower(),
                    };

                    await _tipoTramiteRepositoryBusiness.Registrar(tipoTramite);
                    TempData["MensajeExito"] = "Tipo de trámite registrado correctamente.";
                }
                return RedirectToAction("Index");
            }

            await ListaTipoTramites(model); // Vuelve a cargar listas si hay errores de validación
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(TipoTramiteViewModel model)
        {
            try
            {
                await _tipoTramiteRepositoryBusiness.Eliminar(model.Id.Value);
                TempData["MensajeExito"] = "Tipo de trámite eliminado correctamente";
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
            return RedirectToAction(model.Redirigir);
        }

        [HttpGet]
        public async Task<IActionResult> Modificar(TipoTramiteViewModel model)
        {
            TipoTramite tipoTramite = await _tipoTramiteRepositoryBusiness.Consultar(model.Id.Value);
            model.Id = tipoTramite.Id;
            model.Nombre = tipoTramite.Tipo;

            await ListaTipoTramites(model);
            model.EsEdicion = true;
            ModelState.Clear();

            return View("Index", model);
        }

    }
}
