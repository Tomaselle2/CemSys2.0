using CemSys2.Models;

namespace CemSys2.DTO.Personas
{
    public class DTO_Persona_Historial
    {
        public int? IdPersona { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? Dni { get; set; }

        public bool? Visibilidad { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public DateOnly? FechaDefuncion { get; set; }

        public string? EstadoDifunto { get; set; }

        public ActaDefuncion? ActaDefuncion { get; set; }

        public string? InformacionAdicional { get; set; }

        public int? CategoriaPersona { get; set; }

        public string? Sexo { get; set; }

        public string? Correo { get; set; }

        public string? Celular { get; set; }

        public string? Domicilio { get; set; }

        public bool? DomicilioEnTirolesa { get; set; }

        public bool? FallecioEnTirolesa { get; set; }
    }
}
