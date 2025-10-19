using CemSys2.DTO.Concesiones;
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
             int? tipoParcelaId = null,
             int? seccionId = null,
             int registrosPorPagina = 10,
             int pagina = 1);

        Task<DTO_Persona_Historial> DatosPersonalesPersona (int idPersona);
        Task<int> ModificarPersona(Persona model);
        Task<Persona> ConsultarPersona (int idPersona);
        Task<List<DTO_Persona_Historial_Parcelas>> ListaHistorialParcelas (int idPersona);
        Task<List<DTO_Persona_Historial_Tramites>> ListaHistorialTramites(int idPersona);
        Task<List<DTO_Recibos_Contribuyentes_Titulares>> ListaRecibosContribuyentesTitulares(int idPersona);

        Task<List<int>> ListaIdsPersonasFiltradasParaExcel(
            string? dni = null,
            string? nombre = null,
            string? apellido = null,
            int? categoriaId = null,
            int? tipoParcelaId = null,
            int? seccionId = null);

        Task<List<DTO_Excel_Difuntos>> ListaDifuntosExcel(List<int> idsDifuntos);

        Task<List<DTO_Titulares>> ListaTitularesActualesContrato(List<int> idTitulares);

        Task<Persona> BuscarContribuyente(string DniContribuyente, string sexo);
        Task<Persona> RegistrarContribuyente(Persona contribuyente);

        Task<bool> VerificarRelacioPersonaTramiteExiste(int tramiteId, int personaId);
        Task AgregarTramitePersona(TramitePersona tramitePersona);
        Task<Persona> RegistrarTitular(Persona titular);
    }
}
