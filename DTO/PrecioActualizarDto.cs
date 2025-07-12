using System.ComponentModel.DataAnnotations;

namespace CemSys2.DTO
{
    public class PrecioActualizarDto
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ConceptoTarifariaId es requerido")]
        public int ConceptoTarifariaId { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Precio { get; set; }
    }
}
