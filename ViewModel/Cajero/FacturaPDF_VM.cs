using CemSys2.DTO.Factura;
using CemSys2.Models;

namespace CemSys2.ViewModel.Cajero
{
    public class FacturaPDF_VM
    {
        public DTO_Factura Factura { get; set; } = new DTO_Factura();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new List<ConceptosFactura>();
        public decimal PorcentajeFondo { get; set; }
        public string baseUrl = string.Empty;
    }
}
