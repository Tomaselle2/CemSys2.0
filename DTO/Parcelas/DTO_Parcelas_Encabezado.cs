namespace CemSys2.DTO.Parcelas
{
    public class DTO_Parcelas_Encabezado
    {
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public int ParcelaId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int TipoParcela { get; set; }
    }
}
