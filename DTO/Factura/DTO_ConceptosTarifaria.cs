namespace CemSys2.DTO.Factura
{
    public class DTO_ConceptosTarifaria
    {
        public int PrecioId { get; set; }
        public int TarifariaId { get; set; }
        public int ConceptoTarifariaId { get; set; }
        public decimal Precio { get; set; }
        public int TipoConceptoTarifariaId { get; set; }
        public string NombreConcepto { get; set; } = string.Empty;
    }
}
