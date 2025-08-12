using CemSys2.DTO.Introduccion;
using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class ResumenIntroduccionVM
    {
        public List<DTO_Resumen_Introduccion> ResumenIntroduccion { get; set; } = new();
        public Factura Factura { get; set; } = new();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new();
    }
}
