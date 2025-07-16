using CemSys2.DTO.Introduccion;
using CemSys2.Models;

namespace CemSys2.Interface.Introduccion
{
    public interface IIntroduccionBusiness
    {
        Task<List<EstadoDifunto>> ListaEstadoDifunto();
        Task<List<TipoParcela>> ListaTipoParcela();
        Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela);
        Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion);




    }
}
