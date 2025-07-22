using CemSys2.Business;
using CemSys2.Data;
using CemSys2.DTO.Personas;
using CemSys2.Interface.Introduccion;
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
        private readonly IIntroduccionBusiness _introduccionBusiness;
        private int registrosPorPagina = 15; 

        public PersonasController(IPersonasBusiness personasBusiness, IIntroduccionBusiness introduccionBusiness)
        {
            _personasBusiness = personasBusiness;
            _introduccionBusiness = introduccionBusiness;
        }

        public async Task<IActionResult> Index(PersonasVM? viewModel = null)
        {
            if(viewModel == null)
            {
                viewModel = new PersonasVM();
                await CargarCombo(viewModel);
            }
            
            await CargarCombo(viewModel);

            return View(viewModel);
        }

        private async Task CargarCombo(PersonasVM viewModel)
        {
            try
            {
                viewModel.ListaCondicionPersona = await _personasBusiness.ListaCategoriaPersonas();
                viewModel.ListaTipoParcela = await _introduccionBusiness.ListaTipoParcela();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al cargar las categorías de personas: {ex.Message}";
            }

        }

        //funcion que reciba el DNI, nombre, apellido o condicion, puede ser null todos los campos
        [HttpGet]
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
                    viewModel.TipoParcelaId,
                    viewModel.SeccionId,
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
            // Inicializa el viewModel correctamente
            Persona_Historial_VM viewModel = model ?? new Persona_Historial_VM();
            ModelState.Clear(); // Limpia errores de validación

            // Recuperar mensajes de TempData
            if (TempData.TryGetValue("MensajeError", out object mensajeError))
            {
                viewModel.MensajeError = mensajeError.ToString();
                TempData.Remove("MensajeError");
            }

            DTO_Persona_Historial personaHistorial = new DTO_Persona_Historial();
            List<DTO_Persona_Historial_Parcelas> historialParcelas = new List<DTO_Persona_Historial_Parcelas>();
            List<DTO_Persona_Historial_Tramites> historialTramites = new List<DTO_Persona_Historial_Tramites>();
            try
            {
                personaHistorial = await _personasBusiness.DatosPersonalesPersona(personaId);
                historialParcelas = await _personasBusiness.ListaHistorialParcelas(personaId);
                historialTramites = await _personasBusiness.ListaHistorialTramites(personaId);

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
            viewModel.ListaHistorialTramites = historialTramites;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarPersona(Persona_Historial_VM viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, retornar a la vista con los errores
                TempData["MensajeError"] = "Por favor, corrija los errores en el formulario.";
                return View("HistorialPersona", viewModel);
            }

            try
            {
                Persona? difunto = new Persona();

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
                    TempData["MensajeError"]  = "No se pudo modificar. Por favor, inténtelo de nuevo.";
                    return View("HistorialPersona", viewModel);
                }

                TempData["MensajeExito"] = "Modificación exitosa";
                return RedirectToAction("HistorialPersona", new { personaId = viewModel.Id });

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al modificar la persona: {ex.Message}";
                return View("HistorialPersona", viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            var secciones = await _introduccionBusiness.ListaSecciones(tipoParcelaId);
            return Json(secciones);
        }
    }
}
