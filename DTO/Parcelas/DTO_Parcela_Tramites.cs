namespace CemSys2.DTO.Parcelas
{
    public class DTO_Parcela_Tramites
    {
        public int TramiteId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int TipoTramite { get; set; }
        public int ParcelaId { get; set; }
        public int EstadoTramite { get; set; }
    }
}
