using CemSys2.DTO.Concesiones;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class GenerarContratoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();

        public string MensajeError = string.Empty;


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

        [StringLength(60, ErrorMessage = "El celular no puede superar los 25 dígitos")]
        public string? Celular { get; set; }

        [StringLength(60, ErrorMessage = "El correo no puede superar los 25 dígitos")]
        public string? CorreoElectronico { get; set; }

        [StringLength(60, ErrorMessage = "El domicilio no puede superar los 100 caracteres")]
        [Required(ErrorMessage = "El domicilio es obligatorio")]
        public string? Domicilio { get; set; }

    }
}
