using System.ComponentModel.DataAnnotations;

namespace CemSys2.DTO.Concesiones
{
    public class DTO_Titulares
    {
        public int Id { get; set; }
        public string Dni { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string? Celular { get; set; } = string.Empty;
        public string? CorreoElectronico { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
    }
}
