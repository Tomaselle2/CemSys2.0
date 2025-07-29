namespace CemSys2.DTO.Parcelas
{
    public class DTO_Historial_Parcelas
    {
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string? Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int DifuntoId { get; set; }
        public int EstadoDifunto { get; set; }
        
    }
}
