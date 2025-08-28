namespace CemSys2.DTO.Personas
{
    public class DTO_Persona_Historial_Tramites
    {
        public int TramiteId { get; set; }
        public int PersonaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public int TipoTramite { get; set; }
        public int EstadoTramite { get; set; }
        
    }
}
