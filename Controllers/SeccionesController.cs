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
        private const int CANTIDAD_POR_PAGINA = 20;

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
            // Validar parámetros
            if (pagina < 1) pagina = 1;
            SeccionesNichosViewModel viewModel = new();

            try
            {
                await CargarCombos(viewModel);

                // Definir filtro una sola vez
                Expression<Func<Seccione, bool>> filtro = s => s.Visibilidad == true && s.TipoParcela == 1;
                Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = q => q.OrderByDescending(s => s.Id);
                // Obtener total de registros y secciones paginadas con el mismo filtro
                int totalRegistros = await _seccionesBusiness.ContarTotalAsync(filtro);
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.Secciones = await ObtenerSeccionesPaginadas(pagina, CANTIDAD_POR_PAGINA, filtro, orderBy);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
            return View(viewModel);
        }

        public async Task<IActionResult> InicioSeccionesFosas(int pagina = 1)
        {
            // Validar parámetros
            if (pagina < 1) pagina = 1;
            SeccionesFosasViewModel viewModel = new();

            try
            {
                // Definir filtro una sola vez
                Expression<Func<Seccione, bool>> filtro = s => s.Visibilidad == true && s.TipoParcela == 2;
                Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = q => q.OrderByDescending(s => s.Id);
                // Obtener total de registros y secciones paginadas con el mismo filtro
                int totalRegistros = await _seccionesBusiness.ContarTotalAsync(filtro);
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.Secciones = await ObtenerSeccionesPaginadas2(pagina, CANTIDAD_POR_PAGINA, filtro, orderBy);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarSeccion(SeccionesNichosViewModel model)
        {
            int idSeccion = 0;

            if (model.Filas == null)
            {
                model.Filas = 1; // Asignar un valor por defecto si es nulo
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(model);
                return View(model.Redirigir, model);
            }


            DTO_secciones seccion = new DTO_secciones()
            {
                Nombre = model.Nombre.ToLower().Trim(),
                Visibilidad = true,
                Filas = model.Filas.Value,
                NroParcelas = model.NroParcelas.Value,
                IdTipoParcela = ObtenerTipoParcela(model.Redirigir),
                Redirigir = model.Redirigir
            };

            if (model.IdTipoNumeracionParcela != null)
            {
                seccion.IdTipoNumeracionParcela = model.IdTipoNumeracionParcela.Value;
            }

            if (model.IdTipoNicho != null)
            {
                seccion.IdTipoNicho = model.IdTipoNicho.Value;
            }

            try
            {
                idSeccion = await _seccionesBusiness.RegistrarSeccion(seccion); //registrar sección
                seccion.Id = idSeccion;
                return RedirectToAction("RegistrarParcelas", "Parcelas", seccion);
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                await CargarCombos(model);
                return View(model.Redirigir, model);
            }

        }

        [HttpPost]
        public async Task<IActionResult> RegistrarSeccionFosa(SeccionesFosasViewModel model)
        {
            int idSeccion = 0;

            if (!ModelState.IsValid)
            {
                return View(model.Redirigir, model);
            }


            DTO_secciones seccion = new DTO_secciones()
            {
                Nombre = model.Nombre.ToLower().Trim(),
                Visibilidad = true,
                Filas = 1,
                NroParcelas = model.NroParcelas.Value,
                IdTipoParcela = ObtenerTipoParcela(model.Redirigir),
                IdTipoNumeracionParcela = 2, // Asignar un tipo de numeración por defecto
                Redirigir = model.Redirigir
            };

            try
            {
                idSeccion = await _seccionesBusiness.RegistrarSeccion(seccion); //registrar sección
                seccion.Id = idSeccion;
                return RedirectToAction("RegistrarParcelas", "Parcelas", seccion);
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return View(model.Redirigir, model);
            }

        }

        private int ObtenerTipoParcela(string tipo)
        {
            switch (tipo)
            {
                case "InicioSeccionesNichos":
                    return 1; // Nicho
                case "InicioSeccionesFosas":
                    return 2; // Fosa
                case "InicioSeccionesPanteones":
                    return 3; // Panteón
                default:
                    return 0;
            }
        }


        [HttpPost]
        public async Task<IActionResult> Eliminar(SeccionesNichosViewModel model)
        {
            try
            {
                await _seccionesBusiness.Eliminar(model.Id.Value);
                return RedirectToAction(model.Redirigir);
            }
            catch(Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
        }


        [HttpPost]
        public async Task<IActionResult> EliminarFosa(SeccionesFosasViewModel model)
        {
            try
            {
                await _seccionesBusiness.Eliminar(model.Id.Value);
                return RedirectToAction(model.Redirigir);
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
        }


        public async Task<IActionResult> AdministrarNichos(string Nombre, string Redirigir, string IdSeccion, int pagina = 1)
        {
            if (pagina < 1) pagina = 1;
            ParcelasViewModel viewModel = new();
            viewModel.Redirigir = Redirigir;
            viewModel.NombreSeccion = Nombre;
            viewModel.IdSeccion = int.Parse(IdSeccion);

            try
            {
                // Definir filtro una sola vez
                Expression<Func<Parcela, bool>> filtro = s => s.Visibilidad == true && s.Seccion == int.Parse(IdSeccion);
                Func<IQueryable<Parcela>, IOrderedQueryable<Parcela>> orderBy = q => q.OrderBy(s => s.Id);
                // Obtener total de registros y secciones paginadas con el mismo filtro
                int totalRegistros = await _seccionesBusiness.ContarTotalparcelasAsync(filtro);
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.Parcelas = await ObtenerparcelasPaginadas(pagina, CANTIDAD_POR_PAGINA, filtro, orderBy);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
                viewModel.ListaTipoNicho = await _seccionesBusiness.ListaTipoNicho();
                return View(viewModel);

            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                return View(viewModel);
            }
        }

        public async Task<IActionResult> AdministrarFosas(string Nombre, string Redirigir, string IdSeccion, int pagina = 1)
        {
            if (pagina < 1) pagina = 1;
            ParcelasViewModel viewModel = new();
            viewModel.Redirigir = Redirigir;
            viewModel.NombreSeccion = Nombre;
            viewModel.IdSeccion = int.Parse(IdSeccion);

            try
            {
                // Definir filtro una sola vez
                Expression<Func<Parcela, bool>> filtro = s => s.Visibilidad == true && s.Seccion == int.Parse(IdSeccion);
                Func<IQueryable<Parcela>, IOrderedQueryable<Parcela>> orderBy = q => q.OrderBy(s => s.Id);
                // Obtener total de registros y secciones paginadas con el mismo filtro
                int totalRegistros = await _seccionesBusiness.ContarTotalparcelasAsync(filtro);
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.Parcelas = await ObtenerparcelasPaginadas(pagina, CANTIDAD_POR_PAGINA, filtro, orderBy);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
                return View(viewModel);

            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                return View(viewModel);
            }
        }

        private async Task<List<DTO_secciones>> ObtenerSeccionesPaginadas(int pagina, int cantidadPorPagina, Expression<Func<Seccione, bool>> filtro = null, Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = null)
        {
            try
            {
                return await _seccionesBusiness.ListaSeccionesPaginado(pagina, cantidadPorPagina, filtro, orderBy);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las secciones paginadas: {ex.Message}", ex);
            }
        }

        private async Task<List<DTO_secciones>> ObtenerSeccionesPaginadas2(int pagina, int cantidadPorPagina, Expression<Func<Seccione, bool>> filtro = null, Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = null)
        {
            try
            {
                return await _seccionesBusiness.ListaSeccionesPaginado2(pagina, cantidadPorPagina, filtro, orderBy);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las secciones paginadas: {ex.Message}", ex);
            }
        }

        private async Task<List<DTO_Parcelas>> ObtenerparcelasPaginadas(int pagina, int cantidadPorPagina, Expression<Func<Parcela, bool>> filtro = null, Func<IQueryable<Parcela>, IOrderedQueryable<Parcela>> orderBy = null)
        {
            try
            {
                return await _seccionesBusiness.ListaParcelasPaginada(pagina, cantidadPorPagina, filtro, orderBy);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las parcelas paginadas: {ex.Message}", ex);
            }
        }

        private async Task CargarCombos(SeccionesNichosViewModel model)
        {
            try
            {
                model.TipoNichos = await _seccionesBusiness.ListaTipoNicho();
                model.TipoNumeracionParcelas = await _seccionesBusiness.ListaNumeracionParcelas();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar los combos: {ex.Message}", ex);
            }
        }
    }

   
}
