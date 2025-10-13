using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Models;

namespace CemSys2.Interface.Facturas
{
    public interface IFacturaBusiness
    {
        Task<int> RegistrarFactura(Factura factura);
        Task<int> RegistrarConceptoFactura(ConceptosFactura concepto);

        Task<Factura> ConsultarFacturaPorTramiteId(int idTramite);
        Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId);
        Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura);
        Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId);
        Task<List<DTO_ConceptosTarifaria>> ListaConceptoTarifariaIntroduccion(int tarifariaId);

        //duplica el precio de los conceptos si "fallecido en tirolesa(false)"
        List<DTO_ConceptosTarifaria> ListaConceptoTarifariaConPreciosConLogicaNegocio(List<DTO_ConceptosTarifaria> conceptosTarifaria, bool fallecidoEnTirolesa);

        Task<int> VerificarDetalleFactura(DTO_VerificarDetalleFactura DTO_verificarDetalleFactura);
        Task VerificarCobrarFactura(DTO_VerificarCobrarFactura dto);

        //para resumen introduccion
        Task<DTO_FacturaInternaPrecios> ConsultarFacturaInternaPorTramiteId(int idTramite);
        //para resumen introduccion
        Task<List<ConceptosFacturaInternasPrecio>> ListaConceptosFacturaInternaPorFactura(int idFactura);

        //crea la factura y los detalles de la factura en una sola transaccion
        Task<int> CrearFactura(DTO_Factura dtoFactura, List<DTO_DetalleFactura> dtoDetalleFactura);

        Task PasarFacturaEstadoEmitir(int idfactura);
        Task PasarFacturaEstadoPendienteCobro(int idFactura);
        Task PasarFacturaEstadoAnulado(int idfactura, string descripcion);
        Task PasarFacturaEstadoCobrado(int idFactura);


        Task<List<DTO_Factura>> ListaFacturasPorTramiteId(int tramiteId);
        Task<List<DTO_Factura>> ListaFacturasPorPersonaId(int personaId);

        Task<List<DTO_Factura>> ListaTotalFacturasEmitidasYPendientes();

        Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasCobradas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null,
             DateTime? fechaHasta = null);

        Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasAnuladas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null,
            DateTime? fechaHasta = null);

        Task<Factura> ConsultarFacturaPorId(int facturaId);

        Task<List<MetodoPago>> ListaMetodoPago();

    }
}
