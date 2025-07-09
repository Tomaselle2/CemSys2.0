using CemSys2.DTO;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class SeccionesPanteonesViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; }

        public bool Visibilidad { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de filas debe ser mayor a 0")]
        public int? Filas { get; set; }

        [Required(ErrorMessage = "Debe ingresar el número de parcelas")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de parcelas debe ser mayor a 0")]
        public int? NroParcelas { get; set; }


        public List<DTO_secciones> Secciones { get; set; } = new();
        public int? IdTipoNumeracionParcela { get; set; }

        [Required(ErrorMessage = "Debe ingresar el tipo de panteón")]
        public int? IdTipoPanteon { get; set; }

        public string? TipoPanteon { get; set; }

        public List<DTO_TipoNumeracionParcela> TipoNumeracionParcelas { get; set; } = new();
        public List<DTO_TipoPanteon> ListaTipoPanteones { get; set; } = new();

        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}
