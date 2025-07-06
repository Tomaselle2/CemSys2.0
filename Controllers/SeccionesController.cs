using CemSys2.Interface;
using CemSys2.Models;
using Microsoft.AspNetCore.Mvc;
using CemSys2.DTO;
using CemSys2.ViewModel;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace CemSys2.Controllers
{
    public class SeccionesController : Controller
    {

        private readonly ISeccionesBusiness _seccionesBusiness;
        public SeccionesController(ISeccionesBusiness seccionesBusiness)
        {
            _seccionesBusiness = seccionesBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> InicioSeccionesNichos(int pagina = 1)
        {
            const int cantidadPorPagina = 5;
            SeccionesViewModel viewModel = new();
            try
            {
                viewModel.TipoNumeracionParcelas = await _seccionesBusiness.ListaNumeracionParcelas(); //trae los tipos de numeración de parcelas
                viewModel.TipoNichos = await _seccionesBusiness.ListaTipoNicho(); //trae los tipos de nichos
                Expression<Func<Seccione, bool>> filtro = s => s.Visibilidad == true;

                // Total de registros con el filtro
                int totalRegistros = await _seccionesBusiness.ContarTotalAsync(filtro);
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)cantidadPorPagina);

                viewModel.Secciones = await ObtenerSeccionesPaginadas(pagina, cantidadPorPagina);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;

            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarSeccion(SeccionesViewModel model)
        {

            if (!ModelState.IsValid)
            {
                await CargarCombos(model);
                return View(model.Redirigir, model);
            }

            Seccione seccion = new Seccione
            {
                Nombre = model.Nombre.Trim(),
                Visibilidad = true,
                Filas = model.Filas.Value,
                NroParcelas = model.NroParcelas.Value,
                TipoNumeracionParcela = model.IdTipoNumeracionParcela.Value,
                TipoParcela = 1
            };

            try
            {
                int idSeccion = await _seccionesBusiness.RegistrarSeccion(seccion);
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                await CargarCombos(model);
                return View(model);
            }

            return RedirectToAction(model.Redirigir);
        }


        [HttpPost]
        public async Task<IActionResult> Eliminar(SeccionesViewModel model)
        {
            try
            {
                await _seccionesBusiness.Eliminar(model.Id.Value);
                return RedirectToAction(model.Redirigir);
            }
            catch(Exception ex)
            {
                return RedirectToAction(model.Redirigir);
            }
        }


        private async Task<List<DTO_secciones>> ObtenerSeccionesPaginadas(int pagina, int cantidadPorPagina)
        {
            try
            {
                return await _seccionesBusiness.ListaSeccionesPaginado(pagina, cantidadPorPagina);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las secciones paginadas: {ex.Message}", ex);
            }
        }

        private async Task CargarCombos(SeccionesViewModel model)
        {
            model.TipoNichos = await _seccionesBusiness.ListaTipoNicho();
            model.TipoNumeracionParcelas = await _seccionesBusiness.ListaNumeracionParcelas();
            model.Secciones = await _seccionesBusiness.ListaSeccionesPaginado(1, 10);
        }
    }

   
}
