using CemSys2.DTO.Parcelas;
using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class ParcelaHistorialVM
    {
        public string? MensajeError { get; set; }
        public List<DTO_Historial_Parcelas> ListaDifuntosActuales { get; set; } = new();
        public DTO_Parcelas_Encabezado EncabezadoParcela { get; set; } = new();
        public List<DTO_Historial_Parcelas> ListaDifuntosHistoricos { get; set; } = new();
        public List<DTO_Parcela_Tramites> ListaTramites { get; set; } = new();
        public List<TipoNicho> ListaTiposNicho { get; set; } = new();
        public List<TipoPanteon> ListaTiposPanteon { get; set; } = new();
        public bool esEdicion { get; set; } = false;
    }
}
