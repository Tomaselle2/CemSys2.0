namespace CemSys2.DTO.Parcelas
{
    public class DTO_Parcela_Tramites
    {
        public int TramiteId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoTramite { get; set; } = string.Empty;
        public int ParcelaId { get; set; }
    }
}
