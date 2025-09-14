using CemSys2.DTO.Concesiones;
using CemSys2.Models;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class ContratoInicadoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();
        public DTO_Datos_Concesion DatosParcela = new DTO_Datos_Concesion();

        public Factura Factura { get; set; } = new();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new();
        public List<RecibosFactura> ListaRecibosFactura { get; set; } = new();
        public List<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new();

        public int? ParcelaId { get; set; }
        public int? TramiteId { get; set; }
        public int? EstadoTramiteId { get; set; }

        public string? NroConcesion { get; set; }
        public int? PrecioSeleccionado { get; set; }
        public string MensajeError = string.Empty;
        public int? CantidadAniosId { get; set; }
        public DateOnly? Vencimiento { get; set; }
        public int? CantidadCuotaSeleccionada { get; set; }
        public decimal PrecioFinal { get; set; }
    }
}
