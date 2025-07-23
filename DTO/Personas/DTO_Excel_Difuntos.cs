using CemSys2.Models;

namespace CemSys2.DTO.Personas
{
    public class DTO_Excel_Difuntos
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;

        public ActaDefuncion? ActaDefuncion { get; set; }

        public string? InformacionAdicional { get; set; }


        public DateTime? FechaIngresoId { get; set; }

        public DateTime? FechaRetiroId { get; set; }

        public DateTime? FechaDefuncion { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public int NroParcela { get; set; }

        public int NroFila { get; set; }

        public string NombreSeccion { get; set; } = string.Empty;

        public int TipoParcela { get; set; }
    }
}
