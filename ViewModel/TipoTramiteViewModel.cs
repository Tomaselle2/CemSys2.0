using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class TipoTramiteViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, ErrorMessage = "El nombre no puede superar los 30 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; } = null!;

        public List<TipoTramite> ListaTipoTramite { get; set; } = new();


        public bool EsEdicion { get; set; } = false;
        public string? MensajeError { get; set; }

        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}
