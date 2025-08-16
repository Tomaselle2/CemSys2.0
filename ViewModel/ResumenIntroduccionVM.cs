using CemSys2.DTO.Introduccion;
using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class ResumenIntroduccionVM : IValidatableObject
    {
        public List<DTO_Resumen_Introduccion> ResumenIntroduccion { get; set; } = new();
        public Factura Factura { get; set; } = new();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new();
        public List<RecibosFactura> ListaRecibosFactura { get; set; } = new();

        public int? IdTramite { get; set; }
        public int? IdFactura { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [NoSoloEspacios]
        [StringLength(100, ErrorMessage = "El concepto no puede superar los 100 caracteres")]
        public string? Concepto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        public decimal? Monto { get; set; }

        public decimal? MontoMaximo { get; set; }

        public string? infoAdicional { get; set; }

        [Required(ErrorMessage = "El archivo es obligatorio")]
        public IFormFile? ArchivoRecibo { get; set; }

        public bool Decreto { get; set; } = false;

        public string? MensajeError { get; set; }

        //contribuyente
        [Range(0, 99999999, ErrorMessage = "El DNI no debe tener más de 8 dígitos")]
        [Required(ErrorMessage = "El DNI es obligatorio")]
        public int? Dni { get; set; }

        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; }

        [StringLength(60, ErrorMessage = "El apellido no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "El contribuyente es obligatorio")]
        public int? IdContribuyente { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            MontoMaximo = Factura.Pendiente;
            if (Monto.HasValue)
            {
                if (Monto < 1)
                {
                    yield return new ValidationResult(
                        $"El monto debe ser mayor o igual a 1",
                        new[] { nameof(Monto) });
                }

                if (Monto > MontoMaximo)
                {
                    yield return new ValidationResult(
                        $"El monto no puede ser mayor que {MontoMaximo}",
                        new[] { nameof(Monto) });
                }
            }
        }
    }
}
