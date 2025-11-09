using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.Login
{
    public class CorreoRecuperacionVM
    {

        [Required(ErrorMessage = "El correo es obligatorio")]
        public string? correo { get; set; }

        public string? MensajeError { get; set; }
    }
}
