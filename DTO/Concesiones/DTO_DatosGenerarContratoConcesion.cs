namespace CemSys2.DTO.Concesiones
{
    public class DTO_DatosGenerarContratoConcesion
    {
        public List<DTO_Difuntos_Para_Concesion> Difuntos { get; set; } = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();
        public int ParcelaId { get; set; }
        public int CantidadAnios { get; set; }
        public DateOnly Vencimiento { get; set; }
        public int NroConcesion { get; set; }
        public int PrecioId { get; set; }
        public int? CuotaId { get; set; }
        public string PagoDescripcion { get; set; } = string.Empty;
        public int EmpleadoId { get; set; }
        public int? ContratoAnteriorId { get; set; }
        public decimal Precio { get; set; }
        public int TipoParcela { get; set; }
        public DateTime fechaGeneracion { get; set; }

        public string SeccionNombre { get; set; }
        public string ParcelaString { get; set; }
        public string formaPago { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }

    }
}
