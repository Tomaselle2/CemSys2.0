using CemSys2.Models;

namespace CemSys2.Interface.Facturas
{
    public interface IFacturaBusiness
    {
        Task<int> RegistrarFactura(Factura factura);
        Task<int> RegistrarConceptoFactura(ConceptosFactura concepto);

        Task<Factura> ConsultarFacturaPorTramiteId(int idTramite);
        Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura);
    }
}
