namespace CemSys2.DTO.Concesiones
{
    public class DTO_Archivos_Documentacion
    {
        public Guid ArchivoId { get; set; }

        public string CategoriaArchivo { get; set; } = null!;

        public int TramiteId { get; set; }

        public string NombreArchivo { get; set; } = null!;

        public string TipoArchivo { get; set; } = null!;

        public long TamanoBytes { get; set; }

        public string? Descripcion { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public bool? Visibilidad { get; set; }
    }
}
