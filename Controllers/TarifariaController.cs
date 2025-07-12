using Microsoft.AspNetCore.Mvc;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using CemSys2.ViewModel;
using System.Threading.Tasks;
using CemSys2.DTO;

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

        [HttpGet]
        public IActionResult AdministrarTarifaria(TarifariaInicioVM model)
        {
            return RedirectToAction("Administrar", new { Id = model.Id });
        }

        public async Task<IActionResult> Administrar(int Id)
        {
            AdministrarTarifariaVM model = new()
            {
                IdTarifaria = Id,
                Redirigir = "Administrar",
                EsEdicion = false,
                NombreTarifaria = string.Empty
            };
            await ListarPreciosTarifaria(model);
            return View(model);
        }

        private async Task ListarPreciosTarifaria(AdministrarTarifariaVM model)
        {
            try
            {
                model.ListaPreciosTarifaria = await _tarifariaBusiness.ConsultarPrecioTarifaria(model.IdTarifaria.Value);
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
            }
        }

        // Método actualizado para AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPreciosTarifaria([FromBody] List<PrecioActualizarDto> precios)
        {
            try
            {
                // Validar que se recibieron datos
                if (precios == null || !precios.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se recibieron precios para actualizar."
                    });
                }

                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = $"Datos inválidos: {string.Join(", ", errores)}"
                    });
                }

                // Validaciones adicionales
                foreach (var precio in precios)
                {
                    if (precio.Id <= 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "ID de precio inválido."
                        });
                    }

                    if (precio.ConceptoTarifariaId <= 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "ID de concepto tarifario inválido."
                        });
                    }

                    if (precio.Precio < 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "El precio no puede ser negativo."
                        });
                    }
                }

                // Actualizar los precios usando el business logic
                await _tarifariaBusiness.ActualizarPreciosTarifaria(precios);

                return Json(new
                {
                    success = true,
                    message = "Precios actualizados correctamente."
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Log del error (agregar tu sistema de logging aquí)
                // _logger.LogError(ex, "Error al actualizar precios de tarifaria");

                return Json(new
                {
                    success = false,
                    message = "No se actualizo ningun precio"
                });
            }

        }
    }
}
