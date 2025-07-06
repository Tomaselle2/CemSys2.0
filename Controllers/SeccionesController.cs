using CemSys2.Interface;
using CemSys2.Models;
using Microsoft.AspNetCore.Mvc;
using CemSys2.DTO;
using CemSys2.ViewModel;
using System.Threading.Tasks;

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

        public async Task<IActionResult> InicioSeccionesNichos()
        {
            SeccionesViewModel viewModel = new();
            try
            {
                viewModel.TipoNumeracionParcelas = await _seccionesBusiness.ListaNumeracionParcelas(); //trae los tipos de numeración de parcelas
                viewModel.TipoNichos = await _seccionesBusiness.ListaTipoNicho(); //trae los tipos de nichos
                viewModel.Secciones = await _seccionesBusiness.ListaSecciones(); //trae las secciones
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
                model.TipoNichos = await _seccionesBusiness.ListaTipoNicho();
                model.TipoNumeracionParcelas = await _seccionesBusiness.ListaNumeracionParcelas();
                model.Secciones = await _seccionesBusiness.ListaSecciones();
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
                model.TipoNichos = await _seccionesBusiness.ListaTipoNicho();
                model.TipoNumeracionParcelas = await _seccionesBusiness.ListaNumeracionParcelas();
                model.Secciones = await _seccionesBusiness.ListaSecciones();
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
    }

   
}
