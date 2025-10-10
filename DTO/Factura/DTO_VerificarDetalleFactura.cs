using CemSys2.DTO.Concesiones;

namespace CemSys2.DTO.Factura
{
    public class DTO_VerificarDetalleFactura
    {
        public List<DTO_DetalleFactura> DetallesFactura { get; set; } = new();
        public int? Contribuyente { get; set; }
        public decimal Pendiente { get; set; }
        public int TramiteId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int EstadoFacturaId { get; set; }
        public int? UsuarioEmiteId { get; set; }

        public IFormFile? Archivo { get; set; }
        public bool Decreto { get; set; } = false;
        public decimal? MontoDecreto { get; set; }
    }
}
