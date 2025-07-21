using CemSys2.DTO.Personas;
using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class Persona_Historial_VM : IValidatableObject
    {

        public int? Id { get; set; }
        public bool NN { get; set; }

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

        public string? EstadoDifunto { get; set; }

        public ActaDefuncion ActaDefuncion { get; set; } = new();

        [StringLength(500, ErrorMessage = "La infomación adicional no puede superar los 500 caracteres")]
        [NoSoloEspacios]
        public string? InformacionAdicional { get; set; }

        public bool? DomicilioEnTirolesa { get; set; }

        public bool? FallecioEnTirolesa { get; set; }

        public DTO_Persona_Historial PersonaHistorial { get; set; } = new();
        public List<DTO_Persona_Historial_Parcelas> ListaHistorialParcelas { get; set; } = new();
        public List<DTO_Persona_Historial_Tramites> ListaHistorialTramites { get; set; } = new();

        public string? MensajeError { get; set; }


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

            // Solo validar Nombre y DNI si NN es false
            if (!NN)
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                    yield return new ValidationResult("El nombre es obligatorio", new[] { nameof(Nombre) });

                if (Dni == null)
                    yield return new ValidationResult("El DNI es obligatorio", new[] { nameof(Dni) });
            }

        }
    }
}
