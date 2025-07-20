using CemSys2.DTO.Personas;
using CemSys2.Interface.Personas;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class PersonasBusiness : IPersonasBusiness
    {
        private readonly IPersonasBD _personasBD;

        public PersonasBusiness(IPersonasBD personasBD)
        {
            _personasBD = personasBD;
        }

        public async Task<Persona> ConsultarPersona(int idPersona)
        {
            return await _personasBD.ConsultarPersona(idPersona);
        }

        public async Task<DTO_Persona_Historial> DatosPersonalesPersona(int idPersona)
        {
            return await _personasBD.DatosPersonalesPersona(idPersona);
        }

        public async Task<List<CategoriaPersona>> ListaCategoriaPersonas()
        {
            return await _personasBD.ListaCategoriaPersonas();
        }

        public async Task<(List<DTO_Difunto_Persona_Index> personas, int totalRegistros)> ListaPersonasIndex(string? dni = null, string? nombre = null, string? apellido = null, int? categoriaId = null, int registrosPorPagina = 10, int pagina = 1)
        {
            return await _personasBD.ListaPersonasIndex(dni, nombre, apellido, categoriaId, registrosPorPagina, pagina);
        }

        public async Task<int> ModificarPersona(Persona model)
        {
            return await _personasBD.ModificarPersona(model);
        }
    }
}
