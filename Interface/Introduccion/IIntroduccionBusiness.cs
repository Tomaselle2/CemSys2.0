using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Reportes;
using CemSys2.Models;

namespace CemSys2.Interface.Introduccion
{
    public interface IIntroduccionBusiness
    {

        Task<List<EstadoDifunto>> ListaEstadoDifunto();
        Task<List<TipoParcela>> ListaTipoParcela();
        Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela);
        Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId);
        Task<List<EmpresaFunebre>> ListaEmpresasFunebres();
        Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados();
        Task<Parcela> ConsultarParcela(int idParcela);


        Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model);
        Task<Persona?> ConsultarDifunto(string dni);

        Task<int> RegistrarIntroduccionCompleta(ActaDefuncion actaDefuncion, Persona difunto, int empleadoId, int empresaSepelioId, int ParcelaId, DateTime fechaIngreso, bool? placa = null);

        Task<(List<Introduccione> introducciones, int totalRegistros)> ListadoIntroducciones(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int registrosPorPagina = 10,  int pagina = 1);
        Task<List<DTO_Resumen_Introduccion>> ObtenerResumenIntroduccion(int idTramite);

        //reportes
        Task<List<Introduccione>> ReporteIntroducciones(DateTime? desde = null, DateTime? hasta = null);


        //facturacion
        Task<List<ConceptosFactura>> ListaConceptoFactura(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int parcelaId, int estadoDifuntoId, bool? placa = null);
        Task<Factura> ConsultarFacturaPorTramiteId(int idTramite);
        Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura);
        Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId);
        Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId);

    }
}
