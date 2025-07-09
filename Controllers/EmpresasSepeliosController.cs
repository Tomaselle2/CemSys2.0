using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace CemSys2.Controllers
{
    public class EmpresasSepeliosController : Controller
    {
        private readonly IRepositoryBusiness<EmpresaFunebre> _empresaSepelioRepositoryBusiness;
        private const int CANTIDAD_POR_PAGINA = 20;

        public EmpresasSepeliosController(IRepositoryBusiness<EmpresaFunebre> empresaSepelioRepositoryBusiness)
        {
            _empresaSepelioRepositoryBusiness = empresaSepelioRepositoryBusiness;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            if (HttpContext.Session.GetInt32("idUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Validar parámetros
            if (pagina < 1) pagina = 1;

            EmpresasSepelioViewModel viewModel = new();

            await ListarEmpresas(viewModel, pagina);

            return View(viewModel);
        }

        private async Task ListarEmpresas(EmpresasSepelioViewModel viewModel, int pagina = 1)
        {
            try
            {
                // Definir filtro una sola vez
                //Expression<Func<Usuario, bool>> filtro = s => s.Visibilidad == true;
                //Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = q => q.OrderByDescending(s => s.Id);

                // Obtener total de registros
                int totalRegistros = await _empresaSepelioRepositoryBusiness.ContarTotalAsync();
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.ListaEmpresasSepelio = await _empresaSepelioRepositoryBusiness.ObtenerPaginadoAsync(pagina, CANTIDAD_POR_PAGINA);
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
        public async Task<IActionResult> Registrar(EmpresasSepelioViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.EsEdicion) // Es una edición
                {
                    // Cargar el usuario existente para obtener la contraseña actual si no se cambió
                    var empresa = await _empresaSepelioRepositoryBusiness.Consultar(model.Id.Value);
                    empresa.Nombre = model.Nombre;

                    int modificacion = await _empresaSepelioRepositoryBusiness.Modificar(empresa);
                    TempData["MensajeExito"] = "Empresa actualizada correctamente";
                    model.EsEdicion = false; // Resetear el estado de edición
                }
                else // Es un registro nuevo
                {
                    EmpresaFunebre empresa = new()
                    {
                        Id = model.Id ?? 0, // Si es nuevo, Id será 0
                        Nombre = model.Nombre,
                    };

                    await _empresaSepelioRepositoryBusiness.Registrar(empresa);
                    TempData["MensajeExito"] = "Empresa registrada correctamente.";
                }
                return RedirectToAction("Index");
            }

            await ListarEmpresas(model); // Vuelve a cargar listas si hay errores de validación
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(EmpresasSepelioViewModel model)
        {
            try
            {
                await _empresaSepelioRepositoryBusiness.Eliminar(model.Id.Value);
                TempData["MensajeExito"] = "Empresa eliminada correctamente";
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
            return RedirectToAction(model.Redirigir);
        }

        [HttpGet]
        public async Task<IActionResult> Modificar(EmpresasSepelioViewModel model)
        {
            EmpresaFunebre modelo = await _empresaSepelioRepositoryBusiness.Consultar(model.Id.Value);
            model.Id = modelo.Id;
            model.Nombre = modelo.Nombre;

            await ListarEmpresas(model);
            model.EsEdicion = true;
            ModelState.Clear();

            return View("Index", model);
        }
    }
}
