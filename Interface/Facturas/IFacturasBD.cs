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

        //lista de conceptos tarifaria para la introduccion
        Task<List<DTO_ConceptosTarifaria>> ListaConceptoTarifariaIntroduccion(int tarifariaId);

        //para resumen introduccion
        Task<DTO_FacturaInternaPrecios> ConsultarFacturaInternaPorTramiteId(int idTramite);
        //para resumen introduccion
        Task<List<ConceptosFacturaInternasPrecio>> ListaConceptosFacturaInternaPorFactura(int idFactura);

        //para archivos
        Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo, string descripcion);
        Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId); //trae todos los archivos menos recibos
        Task EditarArchivo(Guid archivoId, string descripcion, string categoriaArchivo, IFormFile? nuevoArchivo);
    }
}
