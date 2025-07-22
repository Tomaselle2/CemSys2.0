using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Personas;
using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class PersonasVM
    {
        // Propiedades de búsqueda
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public int? CondicionPersonaId { get; set; }
        public int? SeccionId   { get; set; }
        public int? TipoParcelaId { get; set; }

        // Resultados
        public List<DTO_Difunto_Persona_Index> ListaPersonasIndex { get; set; } = new();
        public List<CategoriaPersona> ListaCondicionPersona { get; set; } = new();

        //combos
        public List<DTO_SeccionIntroduccion> ListaSecciones { get; set; } = new();
        public List<TipoParcela> ListaTipoParcela { get; set; } = new();

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public bool HayResultados => ListaPersonasIndex.Any();

        // Mensajes
        public string? MensajeError { get; set; }
        public string? MensajeExito { get; set; }
        public string? Redirigir { get; set; }
    }
}
