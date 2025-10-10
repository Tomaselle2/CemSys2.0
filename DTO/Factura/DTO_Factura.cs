namespace CemSys2.DTO.Factura
{
    public class DTO_Factura
    {
        public int? Id { get; set; }

        public int TramiteId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public decimal Total { get; set; }

        public bool Visibilidad { get; set; }

        public int? TipoTramiteId { get; set; }

        public int? UsuarioEmiteId { get; set; }

        public int? EstadoId { get; set; }

        public int? ContribuyenteId { get; set; }

        public int? MetodoPagoId { get; set; }

        public int? UsuarioCajeroId { get; set; }

        public string? Descripcion { get; set; }

        public string? NombreContribuyente { get; set; }

        public string? NombreUsuarioEmite { get; set; }
    }
}
