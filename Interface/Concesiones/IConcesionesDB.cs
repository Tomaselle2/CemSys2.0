using CemSys2.DTO.Concesiones;

namespace CemSys2.Interface.Concesiones
{
    public interface IConcesionesDB
    {
        Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato();
    }
}
