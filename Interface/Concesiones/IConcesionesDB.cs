using CemSys2.DTO.Concesiones;
using CemSys2.Models;

namespace CemSys2.Interface.Concesiones
{
    public interface IConcesionesDB
    {
        Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato();
        Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId);
        Task<Persona> RegistrarTitular(Persona titular);

    }
}
