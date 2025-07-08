namespace CemSys2.DTO
{
    public class DTO_Parcelas
    {
        public int Id { get; set; }

        public bool Visibilidad { get; set; }

        public int NroParcela { get; set; }

        public int NroFila { get; set; }

        public int CantidadDifuntos { get; set; }

        public int Seccion { get; set; }

        public int? IdTipoNicho { get; set; }

        public string? TipoNicho { get; set; }
    }
}
