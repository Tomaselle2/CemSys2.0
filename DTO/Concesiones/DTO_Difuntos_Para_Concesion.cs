namespace CemSys2.DTO.Concesiones
{
    public class DTO_Difuntos_Para_Concesion
    {
        public int DifuntoId { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public string EstadoDifunto { get; set; } = string.Empty;

    }
}
