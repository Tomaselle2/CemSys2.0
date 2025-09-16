using CemSys2.DTO.Concesiones;
using CemSys2.Interface.Concesiones;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class ConcesionesBusiness : IConcesionesBusiness
    {
        public readonly IConcesionesDB _concesionesDB;

        public ConcesionesBusiness(IConcesionesDB concesionesBd)
        {
           _concesionesDB = concesionesBd;
        }

        public async Task<List<CantidadCuota>> CantidadCuotas()
        {
            return await _concesionesDB.CantidadCuotas();
        }

        public async Task<ContratoConcesion> ConsultarContratoConcesion(int tramiteId)
        {
            return await _concesionesDB.ConsultarContratoConcesion(tramiteId);
        }

        public async Task<DTO_Datos_Concesion> DatosParcela(int parcelaId)
        {
            return await _concesionesDB.DatosParcela(parcelaId);
        }

        public async Task FinalizarPendienteDocumentacion(int tramiteId)
        {
            await _concesionesDB.FinalizarPendienteDocumentacion(tramiteId);
        }

        public async Task<bool> GenerarContrato(DTO_DatosGenerarContratoConcesion contrato, Tramite tramite)
        {
            return await _concesionesDB.GenerarContrato(contrato, tramite);
        }

        public async Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId)
        {
            return await _concesionesDB.ListaDifuntosPorParcela(parcelaId);
        }

        public async Task<DTO_Listado_Paginado_Concesiones> ListadoConcesiones(int paginaActual, int tamanoPagina)
        {
            return await _concesionesDB.ListadoConcesiones(paginaActual, tamanoPagina);
        }

        public async Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato()
        {
           return await _concesionesDB.ListaParcelasSinContrato();
        }

        public async Task<List<DTO_Titulares>> ListaTitularesActualesContrato(int contratoId)
        {
            return await _concesionesDB.ListaTitularesActualesContrato(contratoId);
        }

        public async Task<bool> ModificarContratoConcesion(ContratoConcesion contrato)
        {
            return await  _concesionesDB.ModificarContratoConcesion(contrato);
        }

        public async Task<bool> PasoPendienteDocumentacion(ContratoConcesion contrato, List<DTO_Titulares> titulares, int tipoConceptoTarifariaId)
        {
            return await _concesionesDB.PasoPendienteDocumentacion(contrato, titulares, tipoConceptoTarifariaId);
        }

        public async Task<List<DTO_Precios_Concesion>> PreciosConcesion(int conceptoTarifariaId, int seccionId, int nroFila)
        {
            return await _concesionesDB.PreciosConcesion(conceptoTarifariaId, seccionId, nroFila);
        }

        public async Task<Persona> RegistrarTitular(Persona titular)
        {
            return await _concesionesDB.RegistrarTitular(titular);
        }

        public Task<bool> VerificarArchivoContratoSubido(int tramiteId)
        {
            return _concesionesDB.VerificarArchivoContratoSubido(tramiteId);
        }

        public async Task<int> VerificarSiExisteContratoConcesion(string nroConcesion, int parcelaId)
        {
            return await _concesionesDB.VerificarSiExisteContratoConcesion(nroConcesion, parcelaId);
        }
    }
}
