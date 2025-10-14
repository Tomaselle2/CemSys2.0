using CemSys2.Models;

namespace CemSys2.ViewModel.Cajero
{
    public class FacturaPDF_VM
    {
        public Factura Factura { get; set; } = new Factura();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new List<ConceptosFactura>();
        public decimal PorcentajeFondo { get; set; }
        public string baseUrl = string.Empty;

    }
}
