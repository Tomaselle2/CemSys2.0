using CemSys2.DTO.Introduccion;
using CemSys2.Interface.Introduccion;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class IntroduccionBusiness : IIntroduccionBusiness
    {
        private readonly IIntroduccionBD _introduccionBD;

        public IntroduccionBusiness(IIntroduccionBD introduccionBD)
        {
            _introduccionBD = introduccionBD;
        }

        public Task<List<EstadoDifunto>> ListaEstadoDifunto()
        {
            return _introduccionBD.ListaEstadoDifunto();
        }

        public async Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion)
        {
            return await _introduccionBD.ListaParcelas(idSeccion);
        }

        public async Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela)
        {
            return await _introduccionBD.ListaSecciones(idTipoParcela);
        }

        public Task<List<TipoParcela>> ListaTipoParcela()
        {
            return _introduccionBD.ListaTipoParcela();
        }
    }
}
