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

        //para archivos
        Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo);
    }
}
