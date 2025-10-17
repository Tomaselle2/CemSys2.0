using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Models;

namespace CemSys2.Interface.Facturas
{
    public interface IFacturasBD
    {
        Task<int> RegistrarFactura(Factura factura);
        Task<int> RegistrarConceptoFactura(ConceptosFactura concepto);

        Task<Factura> ConsultarFacturaPorTramiteId(int idTramite);
        Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId);
        Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura);
        Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId);

        //verifica las facturas emitidas y pendientes por tramite
        Task<List<DTO_VerificarMontoFactura>> ListaFacturasEmitidasYPendientesParaVerificarPorTramite(int tramiteId);

        //lista de conceptos tarifaria para la introduccion
        Task<List<DTO_ConceptosTarifaria>> ListaConceptoTarifariaIntroduccion(int tarifariaId);

        //para resumen introduccion
        Task<DTO_FacturaInternaPrecios> ConsultarFacturaInternaPorTramiteId(int idTramite);
        //para resumen introduccion
        Task<List<ConceptosFacturaInternasPrecio>> ListaConceptosFacturaInternaPorFactura(int idFactura);        

        Task<DTO_Factura> ConsultarFacturaPorId(int facturaId);
        Task<Factura> ConsultarFacturaPorIdd(int facturaId);

        Task<List<DTO_Factura>> ListaFacturasPorTramiteId(int tramiteId);
        Task<List<DTO_Factura>> ListaFacturasPorPersonaId(int personaId);

        Task<List<DTO_Factura>> ListaTotalFacturasEmitidasYPendientes();
        Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasCobradas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null,
             DateTime? fechaHasta = null);

        Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasAnuladas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null,
             DateTime? fechaHasta = null);

        Task<List<MetodoPago>> ListaMetodoPago();

        Task<List<DTO_HistorialEstadoFactura>> HistorialEstadoFacturaPorFacturaId(int facturaId); 
    }
}
