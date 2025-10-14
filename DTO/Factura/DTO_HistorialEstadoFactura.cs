namespace CemSys2.DTO.Factura
{
    public class DTO_HistorialEstadoFactura
    {
        public int Id { get; set; }

        public int FacturaId { get; set; }

        public int EstadoId { get; set; }

        public DateTime FechaCambio { get; set; }
    }
}
