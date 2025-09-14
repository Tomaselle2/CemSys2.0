using CemSys2.Interface.Facturas;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class FacturasBusiness : IFacturaBusiness
    {
        private readonly IFacturasBD _facturasBD;

        public FacturasBusiness(IFacturasBD facturasBD)
        {
            _facturasBD = facturasBD;
        }
        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _facturasBD.ConsultarFacturaPorTramiteId(idTramite);
        }

        public async Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura)
        {
            return await _facturasBD.ListaConceptosFacturaPorFactura(idFactura);
        }

        public async Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId)
        {
            return await _facturasBD.ListaRecibosFactura(facturaId);
        }

        public async Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo)
        {
            await _facturasBD.RegistrarArchivo(archivo, mimeType, tramiteId, categoriaArchivo);
        }

        public async Task<int> RegistrarConceptoFactura(ConceptosFactura concepto)
        {
            return await _facturasBD.RegistrarConceptoFactura(concepto);
        }

        public async Task<int> RegistrarFactura(Factura factura)
        {
            return await _facturasBD.RegistrarFactura(factura);
        }

        public async Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId)
        {
            await _facturasBD.RegistrarReciboFactura(recibo, archivo, mimeType, tramiteId);
        }
    }
}
