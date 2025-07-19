using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Reportes;
using CemSys2.Models;

namespace CemSys2.Interface.Introduccion
{
    public interface IIntroduccionBusiness
    {

        Task<int> RegistrarActaDefuncion(ActaDefuncion model);
        Task<int> RegistrarDifunto(Persona model);


        Task<List<EstadoDifunto>> ListaEstadoDifunto();
        Task<List<TipoParcela>> ListaTipoParcela();
        Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela);
        Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId);
        Task<List<EmpresaFunebre>> ListaEmpresasFunebres();
        Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados();

        Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model);
        Task<Persona?> ConsultarDifunto(string dni);

        Task<int> RegistrarIntroduccionCompleta(ActaDefuncion actaDefuncion, Persona difunto, int empleadoId, int empresaSepelioId, int ParcelaId, DateTime fechaIngreso);

        Task<(List<Introduccione> introducciones, int totalRegistros)> ListadoIntroducciones(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int registrosPorPagina = 10,  int pagina = 1);


        //reportes
        //cantidad de introducciones por mes
        Task<List<DTO_IntroduccionReporte>> ReporteIntroduccionesPorFecha(DateTime fechaDesde, DateTime fechaHasta); //fecha
        Task<List<DTO_IntroduccionReporte>> ReporteTodasIntroducciones(); //todas las introducciones
  











    }
}
