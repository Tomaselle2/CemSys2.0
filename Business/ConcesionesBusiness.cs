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

        public async Task<DTO_Datos_Concesion> DatosParcela(int parcelaId)
        {
            return await _concesionesDB.DatosParcela(parcelaId);
        }

        public async Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId)
        {
            return await _concesionesDB.ListaDifuntosPorParcela(parcelaId);
        }

        public async Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato()
        {
           return await _concesionesDB.ListaParcelasSinContrato();
        }

        public async Task<List<DTO_Precios_Concesion>> PreciosConcesion(int conceptoTarifariaId, int seccionId, int nroFila)
        {
            return await _concesionesDB.PreciosConcesion(conceptoTarifariaId, seccionId, nroFila);
        }

        public async Task<Persona> RegistrarTitular(Persona titular)
        {
            return await _concesionesDB.RegistrarTitular(titular);
        }
    }
}
