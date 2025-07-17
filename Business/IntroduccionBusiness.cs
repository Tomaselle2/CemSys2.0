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

        public async Task<Persona?> ConsultarDifunto(string dni)
        {
            return await _introduccionBD.ConsultarDifunto(dni);
        }

        public async Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados()
        {
            return await _introduccionBD.ListaEmpleados();
        }

        public async Task<List<EmpresaFunebre>> ListaEmpresasFunebres()
        {
            return await _introduccionBD.ListaEmpresasFunebres();
        }

        public Task<List<EstadoDifunto>> ListaEstadoDifunto()
        {
            return _introduccionBD.ListaEstadoDifunto();
        }

        public async Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId)
        {
            return await _introduccionBD.ListaParcelas(idSeccion, estadoDifuntoId);
        }

        public async Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela)
        {
            return await _introduccionBD.ListaSecciones(idTipoParcela);
        }

        public Task<List<TipoParcela>> ListaTipoParcela()
        {
            return _introduccionBD.ListaTipoParcela();
        }

        public async Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model)
        {
            return await _introduccionBD.RegistrarEmpresaSepelio(model);
        }
    }
}
