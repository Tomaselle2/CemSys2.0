using CemSys2.DTO.Personas;
using CemSys2.Models;

namespace CemSys2.Interface.Personas
{
    public interface IPersonasBD
    {
        Task<List<CategoriaPersona>> ListaCategoriaPersonas();

        Task<(List<DTO_Difunto_Persona_Index> personas, int totalRegistros)> ListaPersonasIndex(
             string? dni = null,
             string? nombre = null,
             string? apellido = null,
             int? categoriaId = null,
             int registrosPorPagina = 10,
             int pagina = 1);

        Task<DTO_Persona_Historial> DatosPersonalesPersona (int idPersona);
        Task<int> ModificarPersona(Persona model);
        Task<Persona> ConsultarPersona (int idPersona);
        Task<List<DTO_Persona_Historial_Parcelas>> ListaHistorialParcelas (int idPersona);
    }
}
