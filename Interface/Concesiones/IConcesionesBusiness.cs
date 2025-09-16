using CemSys2.DTO.Concesiones;
using CemSys2.Models;

namespace CemSys2.Interface.Concesiones
{
    public interface IConcesionesBusiness
    {
        Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato();
        Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId);
        Task<Persona> RegistrarTitular(Persona titular);
        Task<DTO_Datos_Concesion> DatosParcela(int parcelaId);
        Task<List<DTO_Precios_Concesion>> PreciosConcesion(int conceptoTarifariaId, int seccionId, int nroFila);
        Task<List<CantidadCuota>> CantidadCuotas();

        Task<bool> GenerarContrato(DTO_DatosGenerarContratoConcesion contrato, CemSys2.Models.Tramite tramite); //para generar contrato
        Task<int> VerificarSiExisteContratoConcesion(string nroConcesion, int parcelaId);
        Task<ContratoConcesion> ConsultarContratoConcesion(int tramiteId);
        Task<bool> ModificarContratoConcesion(ContratoConcesion contrato);
        Task<bool> PasoPendienteDocumentacion(ContratoConcesion contrato, List<DTO_Titulares> titulares, int tipoConceptoTarifariaId);
        Task<DTO_Listado_Paginado_Concesiones> ListadoConcesiones(int paginaActual, int tamanoPagina);

        Task<List<DTO_Titulares>> ListaTitularesActualesContrato(int contratoId);
        Task<bool> VerificarArchivoContratoSubido(int tramiteId);
        Task FinalizarPendienteDocumentacion(int tramiteId);



    }
}
