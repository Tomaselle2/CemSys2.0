using CemSys2.DTO.Factura;
using CemSys2.Models;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.Cajero
{
    public class ProcesarFacturasVM
    {
        public Factura Factura { get; set; } = new Factura();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new List<ConceptosFactura>();
        public List<MetodoPago> ListaMetodoPago { get; set; } = new List<MetodoPago>();

        public string? MensajeError { get; set; }
        public decimal PorcentajeFondo { get; set; }


        [Required(ErrorMessage = "El medio de cobro es obligatorio")]
        public int? MetodoPagoId { get; set; }

        public decimal? EfectivoRecibido { get; set; }
        public decimal? MontoTotal { get; set; } //precio total de la factura con el fondo incluido
        public int? FacturaId { get; set; }
    }
}
