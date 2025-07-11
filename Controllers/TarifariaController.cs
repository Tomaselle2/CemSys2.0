using Microsoft.AspNetCore.Mvc;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using CemSys2.ViewModel;

namespace CemSys2.Controllers
{
    public class TarifariaController : Controller
    {
        private readonly ITarifariaBusiness _tarifariaBusiness;

        public TarifariaController(ITarifariaBusiness tarifariaBusiness)
        {
            _tarifariaBusiness = tarifariaBusiness;
        }


        public async Task<IActionResult> Index()
        {
            TarifariaInicioVM model = new();

            await ListarTarifarias(model);
            return View(model);
        }

        private async Task ListarTarifarias(TarifariaInicioVM model)
        {
            try
            {
                model.ListaTarifarias = await _tarifariaBusiness.EmitirListadoTarifaria();
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(TarifariaInicioVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.EsEdicion) // Es una edición
                {
                    var tarifaria = await _tarifariaBusiness.ConsultarTarifaria(model.Id.Value);
                    tarifaria.Nombre = model.Nombre.ToLower();

                    int modificacion = await _tarifariaBusiness.ModificarTarifaria(tarifaria);
                    TempData["MensajeExito"] = "Tarifaria actualizada correctamente";
                    model.EsEdicion = false; // Resetear el estado de edición
                }
                else // Es un registro nuevo
                {
                    Tarifaria tarifaria = new()
                    {
                        Nombre = model.Nombre.ToLower(),
                        Visibilidad = true,
                    };

                    await _tarifariaBusiness.RegistrarTarifaria(tarifaria);
                    TempData["MensajeExito"] = "Tarifaria registrada correctamente.";
                }
                return RedirectToAction("Index");
            }

            await ListarTarifarias(model); // Vuelve a cargar listas si hay errores de validación
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> Modificar(TarifariaInicioVM model)
        {
            Tarifaria tarifaria = await _tarifariaBusiness.ConsultarTarifaria(model.Id.Value);
            model.Id = tarifaria.Id;
            model.Nombre = tarifaria.Nombre;

            await ListarTarifarias(model);

            model.EsEdicion = true;
            ModelState.Clear();

            return View("Index", model);
        }

    }
}
