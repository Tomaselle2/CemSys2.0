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

        public async Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato()
        {
           return await _concesionesDB.ListaParcelasSinContrato();
        }
    }
}
