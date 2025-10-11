using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.DTO.Personas;
using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class Persona_Historial_Contribuyente_Titular : IValidatableObject
    {
        public int? Id { get; set; }
        public bool NN { get; set; }

        public List<DTO_Factura> ListaFacturas { get; set; } = new(); //facturas del contribuyente o titular

        [Range(0, 99999999, ErrorMessage = "El DNI no debe tener más de 8 dígitos")]
        public int? Dni { get; set; }

        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(60, ErrorMessage = "El apellido no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Apellido { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

        [StringLength(500, ErrorMessage = "La infomación adicional no puede superar los 500 caracteres")]
        [NoSoloEspacios]
        public string? InformacionAdicional { get; set; }

        public DTO_Persona_Historial PersonaHistorial { get; set; } = new();
        public List<DTO_Persona_Historial_Tramites> ListaHistorialTramites { get; set; } = new();

        public string? MensajeError { get; set; }

        public int CategoriaPersona { get; set; }

        [StringLength(60, ErrorMessage = "El correo electrónico no puede superar los 60 caracteres")]
        public string? CorreoElectronico { get; set; }

        [StringLength(25, ErrorMessage = "El celular no puede superar los 25 caracteres")]
        public string? Celular { get; set; }

        [StringLength(100, ErrorMessage = "El domicilio no puede superar los 100 caracteres")]
        public string? Domicilio { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime hoy = DateTime.Now;
            DateOnly hoysoloFecha = DateOnly.FromDateTime(hoy);

            if (FechaNacimiento.HasValue && FechaNacimiento > hoysoloFecha)
                yield return new ValidationResult("La fecha de nacimiento no puede ser posterior a hoy", new[] { nameof(FechaNacimiento) });

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
