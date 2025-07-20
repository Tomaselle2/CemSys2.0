namespace CemSys2.DTO.Personas
{
    public class DTO_Difunto_Persona_Index
    {
        public int IdPersona { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; } 

        public string? Dni { get; set; } 

        public string? Sexo { get; set; } 
        
        public int? CategoriaPersona { get; set; }

        public int TotalRegistros { get; set; }
    }
}
