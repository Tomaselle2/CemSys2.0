using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.Login
{
    public class CambiarClaveLoginVM
    {
        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
         ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string? ClaveNueva { get; set; }

        public string? Correo { get; set; }
    }
}
