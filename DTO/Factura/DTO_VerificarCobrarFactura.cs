namespace CemSys2.DTO.Factura
{
    public class DTO_VerificarCobrarFactura
    {
        public int FacturaId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal? EfectivoRecibido { get; set; }
        public decimal MontoTotal { get; set; } //precio total de la factura con el fondo incluido
        public int TramiteId { get; set; }
        public int TipoTramiteId { get; set; }
        public int CajeroId { get; set; } //id del usuario que esta cobrando la factura
        public decimal Interes {  get; set; }
    }
}
