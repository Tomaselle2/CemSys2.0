namespace CemSys2.DTO
{
    public class DTO_Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Usuario1 { get; set; } = string.Empty;

        public string Clave { get; set; } = string.Empty;

        public bool Visibilidad { get; set; }

        public int Rol { get; set; }
    }
}
