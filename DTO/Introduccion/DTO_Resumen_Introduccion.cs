namespace CemSys2.DTO.Introduccion
{
    public class DTO_Resumen_Introduccion
    {
        public int Id { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Empresa { get; set; } = string.Empty;
        public string? dni { get; set; }
        public string? Nombre { get; set; }
        public string Apellido { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public DateTime FechaDefuncion { get; set; }
        public string EstadoDifunto { get; set; } = string.Empty;
        public string? InformacionAdicional { get; set; }
        public int? Acta { get; set; }
        public int? Tomo { get; set; }
        public int? Folio { get; set; }
        public string? Serie { get; set; }
        public int? Age { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public string NroParcela { get; set; } = string.Empty;
        public string NroFila { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public int TipoParcela { get; set; }
        public bool DomicilioEnTirolesa { get; set; }
        public bool FallecioEnTirolesa { get; set; }
        public int CantidadDifuntos { get; set; }
        public int estadoTramite { get; set; }
        public string? informacionAdicionalTramite { get; set; }
        public decimal Precio { get; set; }
        public decimal Pendiente { get; set; }
    }
}
