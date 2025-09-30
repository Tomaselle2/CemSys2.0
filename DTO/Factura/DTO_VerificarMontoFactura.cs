namespace CemSys2.DTO.Factura
{
    public class DTO_VerificarMontoFactura
    {
        public int FacturaId { get; set; }
        public decimal MontoTotal { get; set; }
        public int TramiteId { get; set; }
        public int EstadoId { get; set; }
    }
}
