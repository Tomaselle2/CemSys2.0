namespace CemSys2.DTO.Concesiones
{
    public class DTO_Parcelas_Sin_Contrato
    {
        public int ParcelaId { get; set; }
        public int TipoParcela {  get; set; }
        public string NombreSeccion {  get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public string Difuntos { get; set; } = string.Empty;
        public int EstadoTramiteIntroduccion { get; set; }
    }
}
