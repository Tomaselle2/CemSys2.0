using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel
{
    public class ConceptoTarifariaVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, ErrorMessage = "El nombre no puede superar los 30 caracteres")]
        [NoSoloEspacios]
        public string? Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de concepto es obligatorio")]
        public int? TipoConceptoId { get; set; }

        public string? TipoConceptoNombre { get; set; }

        public bool? Visibilidad { get; set; }

        public List<ConceptosTarifaria> ListaConceptos { get; set; } = new();
        public List<TiposConceptoTarifarium> ListaTiposConcepto { get; set; } = new();


        public bool EsEdicion { get; set; } = false;
        public string? MensajeError { get; set; }

        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }


    }
}
