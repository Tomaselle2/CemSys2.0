using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.Reportes
{
    public class FacturasReporteVM
    {
        public List<DTO.Factura.DTO_FacturasReporte> ListaFacturas { get; set; } = new();

        [Required(ErrorMessage = "El campo fecha desde es obligatorio.")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "El campo fecha hasta es obligatorio.")]
        public DateTime FechaHasta { get; set; }

        public string? MensajeError { get; set; }
    }
}
