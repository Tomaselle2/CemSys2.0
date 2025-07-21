namespace CemSys2.DTO.Personas
{
    public class DTO_Persona_Historial_Parcelas
    {
        public int? Id { get; set; }

        public DateTime? FechaIngresoId { get; set; }

        public DateTime? FechaRetiroId { get; set; }

        public int? NroParcela { get; set; }

        public int? NroFila { get; set; }

        public string? NombreSeccion { get; set; }

        public int? TipoParcela { get; set; }
    }
}
