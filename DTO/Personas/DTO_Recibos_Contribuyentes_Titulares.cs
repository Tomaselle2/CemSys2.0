namespace CemSys2.DTO.Personas
{
    public class DTO_Recibos_Contribuyentes_Titulares
    {
        public int TramiteId { get; set; }
        public int PersonaId { get; set; }
        public DateTime FechaPago { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public Guid ReciboId { get; set; }
    }
}
