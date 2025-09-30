namespace CemSys2.DTO.Factura
{
    public class DTO_FacturaInternaPrecios
    {
        public int Id { get; set; }

        public int TramiteId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public decimal Total { get; set; }

        public bool Visibilidad { get; set; }
    }
}
