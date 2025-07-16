using CemSys2.DTO.Introduccion;
using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class IntroduccionDifuntoVM :IValidatableObject
    {
        public int? Id { get; set; }

        [Range(0, 99999999, ErrorMessage = "El DNI no debe tener más de 8 dígitos")]
        public int? Dni { get; set; }

        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(60, ErrorMessage = "El apellido no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "La fecha de defunción es obligatoria")]
        public DateOnly? FechaDefuncion { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateOnly? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "El estado del difunto es obligatorio")]
        public int? EstadoDifuntoId { get; set; }

        public ActaDefuncion ActaDefuncion { get; set; } = new();

        public bool NN { get; set; } = false;

        [StringLength(500, ErrorMessage = "La infomación adicional no puede superar los 500 caracteres")]
        [NoSoloEspacios]
        public string? InformacionAdicional { get; set; }

        [Required(ErrorMessage = "La opción de domicilio del difunto es obligatoria")]
        public bool? DomicilioEnTirolesa { get; set; }

        [Required(ErrorMessage = "La opción del lugar del fallecimiento del difunto es obligatoria")]
        public bool? FallecioEnTirolesa { get; set; }

        [Required(ErrorMessage = "El tipo de parcela es obligatorio")]
        public int? TipoParcelaID { get; set; }

        [Required(ErrorMessage = "La sección es obligatoria")]
        public int? SeccionID { get; set; }

        [Required(ErrorMessage = "La parcela es obligatoria")]
        public int? ParcelaID { get; set; }

        [Required(ErrorMessage = "El empleado es obligatorio")]
        public int? EmpleadoID { get; set; }

        [Required(ErrorMessage = "La empresa de sepelio es obligatoria")]
        public int? EmpresaFunebreID { get; set; }

        [Required(ErrorMessage = "La fecha y hora de ingreso es obligatoria")]
        public DateTime? FechaHoraIngreso { get; set; }

        public List<EstadoDifunto> ListaEstadoDifunto { get; set; } = new();
        public List<TipoParcela> ListaTipoParcela { get; set; } = new();
        public List<DTO_SeccionIntroduccion> ListaSecciones { get; set; } = new();
        public List<DTO_parcelaIntroduccion> ListaParcelas { get; set; } = new();
        public List<EmpresaFunebre> ListaEmpresasSepelio { get; set; } = new();


        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime hoy = DateTime.Now;
            DateOnly hoysoloFecha = DateOnly.FromDateTime(hoy);

            if (FechaDefuncion.HasValue && FechaDefuncion > hoysoloFecha)
                yield return new ValidationResult("La fecha de defunción no puede ser posterior a hoy", new[] { nameof(FechaDefuncion) });

            if (FechaNacimiento.HasValue && FechaNacimiento > hoysoloFecha)
                yield return new ValidationResult("La fecha de nacimiento no puede ser posterior a hoy", new[] { nameof(FechaNacimiento) });

            if (FechaDefuncion.HasValue && FechaNacimiento.HasValue && FechaNacimiento > FechaDefuncion)
                yield return new ValidationResult("La fecha de nacimiento no puede ser posterior a la fecha de defunción", new[] { nameof(FechaNacimiento), nameof(FechaDefuncion) });

            if (FechaHoraIngreso.HasValue && FechaHoraIngreso > hoy)
                yield return new ValidationResult("La fecha y hora de ingreso no puede ser posterior a la fecha y hora actual", new[] { nameof(FechaHoraIngreso) });

            if (!NN && string.IsNullOrWhiteSpace(Nombre))
                yield return new ValidationResult("El nombre es obligatorio", new[] { nameof(Nombre) });

            if (!NN && Dni == null)
                yield return new ValidationResult("El DNI es obligatorio", new[] { nameof(Dni) });

        }
    }
}
