using CemSys2.DTO.Concesiones;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class ContratoInicadoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();
        public DTO_Datos_Concesion DatosParcela = new DTO_Datos_Concesion();

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
