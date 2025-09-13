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


    }
}
