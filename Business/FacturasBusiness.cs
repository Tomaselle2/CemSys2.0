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

        public async Task<int> RegistrarConceptoFactura(ConceptosFactura concepto)
        {
            return await _facturasBD.RegistrarConceptoFactura(concepto);
        }

        public async Task<int> RegistrarFactura(Factura factura)
        {
            return await _facturasBD.RegistrarFactura(factura);
        }
    }
}
