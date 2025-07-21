using CemSys2.Data;
using CemSys2.DTO.Personas;
using CemSys2.Interface.Personas;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class PersonasController : Controller
    {
        private readonly IPersonasBusiness _personasBusiness;
        private int registrosPorPagina = 15; 

        public PersonasController(IPersonasBusiness personasBusiness)
        {
            _personasBusiness = personasBusiness;
        }

        public async Task<IActionResult> Index()
        {
            PersonasVM viewModel = new PersonasVM();
            await CargarCombo(viewModel);

            return View(viewModel);
        }

        private async Task CargarCombo(PersonasVM viewModel)
        {
            try
            {
                viewModel.ListaCondicionPersona = await _personasBusiness.ListaCategoriaPersonas();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al cargar las categorías de personas: {ex.Message}";
            }

        }

        //funcion que reciba el DNI, nombre, apellido o condicion, puede ser null todos los campos
        [HttpPost]
        public async Task<IActionResult> BuscarPersona(PersonasVM viewModel, int pagina = 1)
        {
            try
            {
                // Realizar la búsqueda con paginación
                var (personas, totalRegistros) = await _personasBusiness.ListaPersonasIndex(
                    viewModel.Dni,
                    viewModel.Nombre,
                    viewModel.Apellido,
                    viewModel.CondicionPersonaId,
                    registrosPorPagina,
                    pagina);

                // Actualizar el ViewModel con los resultados
                viewModel.ListaPersonasIndex = personas;
                viewModel.TotalRegistros = totalRegistros;
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                // Cargar el combo de categorías
                await CargarCombo(viewModel);
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al buscar la persona: {ex.Message}";
            }

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> HistorialPersona(int personaId, Persona_Historial_VM model = null)
        {
            if (model != null && model.Id.HasValue)
            {
                // Si viene del POST con errores, mantener el modelo existente
                return View(model);
            }

            Persona_Historial_VM viewModel = new Persona_Historial_VM();
            ModelState.Clear(); // Limpia errores de validación

            DTO_Persona_Historial personaHistorial = new DTO_Persona_Historial();
            List<DTO_Persona_Historial_Parcelas> historialParcelas = new List<DTO_Persona_Historial_Parcelas>();
            try
            {
                personaHistorial = await _personasBusiness.DatosPersonalesPersona(personaId);
                historialParcelas = await _personasBusiness.ListaHistorialParcelas(personaId);

            }
            catch(Exception ex)
            {
                viewModel.MensajeError = $"Error al obtener los datos de la persona: {ex.Message}";
            }

            viewModel.Id = personaHistorial.IdPersona;
            viewModel.Dni = (personaHistorial.Dni == "nn") ? null : int.Parse(personaHistorial.Dni);
            viewModel.Nombre = (personaHistorial.Nombre == "nn") ? null : personaHistorial.Nombre;
            viewModel.Apellido = personaHistorial.Apellido;
            viewModel.FechaDefuncion = personaHistorial.FechaDefuncion;
            viewModel.FechaNacimiento = personaHistorial.FechaNacimiento;
            viewModel.Sexo = personaHistorial.Sexo;
            viewModel.EstadoDifunto = personaHistorial.EstadoDifunto;
            viewModel.ActaDefuncion.Id = personaHistorial.ActaDefuncion.Id;
            viewModel.ActaDefuncion.Acta = personaHistorial.ActaDefuncion?.Acta;
            viewModel.ActaDefuncion.Tomo = personaHistorial.ActaDefuncion?.Tomo;
            viewModel.ActaDefuncion.Folio = personaHistorial.ActaDefuncion?.Folio;
            viewModel.ActaDefuncion.Serie = personaHistorial.ActaDefuncion?.Serie;
            viewModel.ActaDefuncion.Age = personaHistorial.ActaDefuncion?.Age;
            viewModel.InformacionAdicional = (personaHistorial.InformacionAdicional == null) ? null : personaHistorial.InformacionAdicional;
            viewModel.DomicilioEnTirolesa = personaHistorial.DomicilioEnTirolesa;
            viewModel.FallecioEnTirolesa = personaHistorial.FallecioEnTirolesa;
            viewModel.NN = (personaHistorial.Dni == "nn") ? true : false;


            viewModel.ListaHistorialParcelas = historialParcelas;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarPersona(Persona_Historial_VM viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, retornar a la vista con los errores
                viewModel.MensajeError = "Por favor, corrija los errores en el formulario.";
                return View("HistorialPersona", viewModel);
            }

            try
            {
                Persona model = await _personasBusiness.ConsultarPersona(viewModel.Id.Value);
                model.Dni = (viewModel.NN) ? "nn" : viewModel.Dni?.ToString() ?? "nn";
                model.Nombre = viewModel.Nombre?.Trim() ?? "nn";
                model.Apellido = viewModel.Apellido.Trim();
                model.FechaDefuncion = viewModel.FechaDefuncion;
                model.FechaNacimiento = viewModel.FechaNacimiento;
                model.Sexo = viewModel.Sexo;

                model.ActaDefuncionNavigation.Acta = viewModel.ActaDefuncion.Acta;
                model.ActaDefuncionNavigation.Tomo = viewModel.ActaDefuncion.Tomo;
                model.ActaDefuncionNavigation.Folio = viewModel.ActaDefuncion.Folio;
                model.ActaDefuncionNavigation.Serie = viewModel.ActaDefuncion.Serie;
                model.ActaDefuncionNavigation.Age = viewModel.ActaDefuncion.Age;

                model.InformacionAdicional = viewModel.InformacionAdicional;

                int resultado = await _personasBusiness.ModificarPersona(model);

                if (resultado == 0)
                {
                    viewModel.MensajeError = "No se pudo modificar. Por favor, inténtelo de nuevo.";
                    return View("HistorialPersona", viewModel);
                }

                TempData["MensajeExito"] = "Modificación exitosa";
                return RedirectToAction("HistorialPersona", new { personaId = viewModel.Id });

            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al modificar la persona: {ex.Message}";
                return View("HistorialPersona", viewModel);
            }
        }
    }
}
