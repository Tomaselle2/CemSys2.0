using CemSys2.DTO.Introduccion;
using CemSys2.Models;

namespace CemSys2.Interface.Introduccion
{
    public interface IIntroduccionBD
    {
        //registrar acta defuncion (Generico)
        //Registrar difunto (Generico)
        //registrarTramite
        //registrar introduccion
        //registrar tabla parcelaDifunto
        //registrar HistorialEstadoTramite

        Task<int> RegistrarActaDefuncion(ActaDefuncion model);
        Task<int> RegistrarDifunto(Persona model);
        Task<int> RegistrarTramite(Tramite model);
        Task<int> RegistrarIntroduccion(Introduccione model);
        Task<int> RegistrarParcelaDifunto(ParcelaDifunto model);
        Task<int> RegistrarHistorialEstadoTramite(HistorialEstadoTramite model);

        Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model);

        Task<Persona?> ConsultarDifunto(string dni);
        Task<List<EstadoDifunto>> ListaEstadoDifunto();
        Task<List<TipoParcela>> ListaTipoParcela();
        Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela);
        Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId);
        Task<List<EmpresaFunebre>> ListaEmpresasFunebres();
        Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados();
    }
}
