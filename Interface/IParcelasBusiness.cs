using CemSys2.DTO;
using CemSys2.DTO.Parcelas;

namespace CemSys2.Interface
{
    public interface IParcelasBusiness
    {
        Task RegistrarParcelas(DTO_secciones seccion);
        Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosActuales(int parcelaId);
        Task<DTO_Parcelas_Encabezado> EncabezadoParcela(int parcelaId);
        Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosHistoricos(int parcelaId);


    }
}
