using CemSys2.Interface.Personas;
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
    }
}
