using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace CemSys2.Controllers
{
    public class CementeriosController : Controller
    {
        private readonly IRepositoryBusiness<Cementerio> _cementerioRepositoryBusiness;
        private const int CANTIDAD_POR_PAGINA = 20;

        public CementeriosController(IRepositoryBusiness<Cementerio> cementerioRepositoryBusiness)
        {
            _cementerioRepositoryBusiness = cementerioRepositoryBusiness;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            if (HttpContext.Session.GetInt32("idUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Validar parámetros
            if (pagina < 1) pagina = 1;

            CementerioViewModel viewModel = new();

            await ListarCementerios(viewModel, pagina);

            return View(viewModel);
        }

        private async Task ListarCementerios(CementerioViewModel viewModel, int pagina = 1)
        {
            try
            {
                // Definir filtro una sola vez
                Expression<Func<Cementerio, bool>> filtro = s => s.Visibilidad == true;
                Func<IQueryable<Cementerio>, IOrderedQueryable<Cementerio>> orderBy = q => q.OrderByDescending(s => s.Id);

                // Obtener total de registros
                int totalRegistros = await _cementerioRepositoryBusiness.ContarTotalAsync();
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.ListaCementerios = await _cementerioRepositoryBusiness.ObtenerPaginadoAsync(pagina, CANTIDAD_POR_PAGINA, filtro, orderBy);
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
        public async Task<IActionResult> Registrar(CementerioViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.EsEdicion) // Es una edición
                {
                    // Cargar el usuario existente para obtener la contraseña actual si no se cambió
                    var cementerio = await _cementerioRepositoryBusiness.Consultar(model.Id.Value);
                    cementerio.Nombre = model.Nombre;

                    int modificacion = await _cementerioRepositoryBusiness.Modificar(cementerio);
                    TempData["MensajeExito"] = "Cementerio actualizado correctamente";
                    model.EsEdicion = false; // Resetear el estado de edición
                }
                else // Es un registro nuevo
                {
                    Cementerio cementerio = new()
                    {
                        Id = model.Id ?? 0, // Si es nuevo, Id será 0
                        Nombre = model.Nombre,
                        Visibilidad = true // Por defecto, al registrar un nuevo cementerio, se establece como visible
                    };

                    await _cementerioRepositoryBusiness.Registrar(cementerio);
                    TempData["MensajeExito"] = "Cementerio registrado correctamente.";
                }
                return RedirectToAction("Index");
            }

            await ListarCementerios(model); // Vuelve a cargar listas si hay errores de validación
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(CementerioViewModel model)
        {
            try
            {
                Cementerio cementerio = await _cementerioRepositoryBusiness.Consultar(model.Id.Value);
                cementerio.Visibilidad = false; // Cambiar visibilidad a false en lugar de eliminar
                await _cementerioRepositoryBusiness.Modificar(cementerio);
                TempData["MensajeExito"] = "Cementerio eliminado correctamente";
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
            return RedirectToAction(model.Redirigir);
        }

        [HttpGet]
        public async Task<IActionResult> Modificar(CementerioViewModel model)
        {
            Cementerio modelo = await _cementerioRepositoryBusiness.Consultar(model.Id.Value);
            model.Id = modelo.Id;
            model.Nombre = modelo.Nombre;

            await ListarCementerios(model);
            model.EsEdicion = true;
            ModelState.Clear();

            return View("Index", model);
        }
    }

}

