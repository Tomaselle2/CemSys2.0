namespace CemSys2.DTO.Factura
{
    public class DTO_DetalleFactura
    {
        public int? Id { get; set; }

        public int FacturaId { get; set; }

        public int ConceptoTarifariaId { get; set; }

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        public int? TipoConceptoFacturaId { get; set; }
    }
}
