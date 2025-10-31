using CemSys2.ValidacionAnotations;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.Usuario
{
    public class PerfirUsuarioModificarVM
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

        //[StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        //[NoSoloEspacios]
        //public string? Clave { get; set; }

        public string? MensajeError { get; set; }
    }
}
