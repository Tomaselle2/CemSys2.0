using CemSys2.DTO.Concesiones;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class GenerarContratoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public string MensajeError = string.Empty;

        [Required(ErrorMessage = "El contribuyente es obligatorio")]
        public int? IdContribuyente { get; set; }
    }
}
