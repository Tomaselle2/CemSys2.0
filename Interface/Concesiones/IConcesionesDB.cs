using CemSys2.DTO.Concesiones;
using CemSys2.Models;

namespace CemSys2.Interface.Concesiones
{
    public interface IConcesionesDB
    {
        Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato(); //en el index de concesiones
        Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId); //para generar contrato
        Task<DTO_Datos_Concesion> DatosParcela(int parcelaId); //para generar contrato
        Task<Persona> RegistrarTitular(Persona titular); //para generar contrato
        Task<List<DTO_Precios_Concesion>> PreciosConcesion(int conceptoTarifariaId, int seccionId, int nroFila); //para generar contrato
        Task<List<CantidadCuota>> CantidadCuotas(); //para generar contrato

        Task<bool> GenerarContrato(DTO_DatosGenerarContratoConcesion contrato, CemSys2.Models.Tramite tramite); //para generar contrato


    }
}
