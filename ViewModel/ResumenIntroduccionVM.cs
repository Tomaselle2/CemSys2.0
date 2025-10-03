using CemSys2.DTO.Factura;
using CemSys2.DTO.Introduccion;
using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class ResumenIntroduccionVM : IValidatableObject
    {
        public List<DTO_Resumen_Introduccion> ResumenIntroduccion { get; set; } = new();
        public DTO_FacturaInternaPrecios FacturaInterna { get; set; } = new();
        public List<ConceptosFacturaInternasPrecio> ListaConceptosFactura { get; set; } = new();
        public List<RecibosFactura> ListaRecibosFactura { get; set; } = new();
        public List<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new();
        public List<DTO_ConceptosTarifaria> ListaConceptosTarifaria { get; set; } = new();
        public List<DTO_DetalleFactura> ListaDetalleFactura { get; set; } = new(); //lista de conceptos de la factura (detalle)

        public int? IdTramite { get; set; }
        public int? IdFactura { get; set; }
        public int? IdRecibo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(100, ErrorMessage = "La descripción no puede superar los 100 caracteres")]
        public string? Descripcion { get; set; } = string.Empty;

        public decimal? Monto { get; set; }

        public decimal? MontoMaximo { get; set; }

        public string? infoAdicional { get; set; }

        public bool EsEdicion { get; set; }

        public IFormFile? ArchivoRecibo { get; set; }

        public bool Decreto { get; set; } = false;

        public string? MensajeError { get; set; }

        public decimal? MontoMinimoFondo { get; set; }
        public decimal? PorcentajeFondo { get; set; }

        //contribuyente
        [Range(0, 99999999, ErrorMessage = "El DNI no debe tener más de 8 dígitos")]
        [Required(ErrorMessage = "El DNI es obligatorio")]
        public int? Dni { get; set; }

        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(60, ErrorMessage = "El apellido no puede superar los 60 caracteres")]
        public string? Apellido { get; set; }


        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

      

        [Required(ErrorMessage = "El contribuyente es obligatorio")]
        public int? IdContribuyente { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Decreto == true && !EsEdicion && ArchivoRecibo == null)
            {
                yield return new ValidationResult(
                    "El archivo es obligatorio al cargar un nuevo recibo.",
                    new[] { nameof(ArchivoRecibo) });
            }

            if (Decreto == true && Monto.HasValue)
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
