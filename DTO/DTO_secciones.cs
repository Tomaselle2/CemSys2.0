using CemSys2.Models;

namespace CemSys2.DTO
{
    public class DTO_secciones
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Visibilidad { get; set; }
        public int Filas { get; set; }
        public int NroParcelas { get; set; }
        public string TipoNumeracion { get; set; } = null!;
    }
}
