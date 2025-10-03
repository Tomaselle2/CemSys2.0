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

        public int? PrecioId { get; set; } //este precio es el id del precio que se selecciona de la tarifaria, solo para comparar
    }
}
