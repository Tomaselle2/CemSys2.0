using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.Usuario
{
    public class CambiarContraseniaUsuarioVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
         ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string? ClaveNueva { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        public string? ClaveAnterior { get; set; }

        public string? MensajeError { get; set; }

    }
}
