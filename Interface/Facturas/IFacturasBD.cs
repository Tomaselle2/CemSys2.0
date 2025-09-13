using CemSys2.Models;

namespace CemSys2.Interface.Facturas
{
    public interface IFacturasBD
    {
        Task<int> RegistrarFactura(Factura factura);
        Task<int> RegistrarConceptoFactura(ConceptosFactura concepto);
    }
}
