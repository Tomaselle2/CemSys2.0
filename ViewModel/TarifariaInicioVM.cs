using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class TarifariaInicioVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(20, ErrorMessage = "El nombre no puede superar los 20 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; } = null!;

        public bool? Visibilidad { get; set; }

        public List<Tarifaria> ListaTarifarias { get; set; } = new();

        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }
        public bool EsEdicion { get; set; } = false;

    }
}
