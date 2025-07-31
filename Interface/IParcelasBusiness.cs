using CemSys2.DTO;
using CemSys2.DTO.Parcelas;
using CemSys2.Models;

namespace CemSys2.Interface
{
    public interface IParcelasBusiness
    {
        Task RegistrarParcelas(DTO_secciones seccion);
        Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosActuales(int parcelaId);
        Task<DTO_Parcelas_Encabezado> EncabezadoParcela(int parcelaId);
        Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosHistoricos(int parcelaId);
        Task<List<DTO_Parcela_Tramites>> ListaParcelasTramites(int parcelaId);
        Task<List<TipoNicho>> ListaTiposNicho();
        Task<List<TipoPanteon>> ListaTipoPanteon();
        Task<Parcela> BuscarParcelaPorId(int parcelaId);
        Task<int> ModificarParcela(Parcela parcela);

    }
}
