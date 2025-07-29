namespace CemSys2.DTO.Introduccion
{
    public class DTO_Resumen_Introduccion
    {
        public int Id { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Empresa { get; set; }
        public string? dni { get; set; }
        public string? Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime FechaDefuncion { get; set; }
        public string EstadoDifunto { get; set; }
        public string? InformacionAdicional { get; set; }
        public int? Acta { get; set; }
        public int? Tomo { get; set; }
        public int? Folio { get; set; }
        public string? Serie { get; set; }
        public int? Age { get; set; }
        public string Empleado { get; set; }
        public string NroParcela { get; set; }
        public string NroFila { get; set; }
        public string Seccion { get; set; }
        public int TipoParcela { get; set; }
        public bool DomicilioEnTirolesa { get; set; }
        public bool FallecioEnTirolesa { get; set; }
    }
}
