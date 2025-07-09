using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class UsuariosViewModel : IValidatableObject
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [StringLength(60, ErrorMessage = "El correo no puede superar los 60 caracteres")]
        [NoSoloEspacios]
        public string? Correo { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(30, ErrorMessage = "El usuario no puede superar los 30 caracteres")]
        [NoSoloEspacios]
        public string? NombreUsuario { get; set; } = null!;

        [StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        [NoSoloEspacios]
        public string? Clave { get; set; }

        public bool? Visibilidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public int? Rol { get; set; }

        public List<Usuario> ListaUsuarios { get; set; } = new();
        public List<RolesUsuario> ListaRolesUsuarios { get; set; } = new();

        public bool EsEdicion { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // La contraseña es obligatoria SOLO si NO estamos en modo edición (es decir, es un nuevo registro)
            if (!EsEdicion && string.IsNullOrEmpty(Clave))
            {
                yield return new ValidationResult(
                    "La contraseña es obligatoria para nuevos registros.",
                    new[] { nameof(Clave) }
                );
            }
        }

        public string? MensajeError { get; set; }

        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}
