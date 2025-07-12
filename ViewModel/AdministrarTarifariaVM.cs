using CemSys2.DTO;
using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class AdministrarTarifariaVM
    {
        public int? IdTarifaria { get; set; }
        public string? NombreTarifaria { get; set; }

        public List<PreciosTarifaria> ListaPreciosTarifaria { get; set; } = new();
        public List<ConceptosTarifaria> ListaConceptoTarifaria { get; set; } = new();
        public List<Seccione> ListaSecciones { get; set; } = new();
        public List<PrecioActualizarDto> ListaPrecioDTO { get; set; } = new();

        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }
        public bool EsEdicion { get; set; } = false;
    }
}
