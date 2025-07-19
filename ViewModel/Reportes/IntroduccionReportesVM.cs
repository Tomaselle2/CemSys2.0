using CemSys2.DTO.Reportes;

namespace CemSys2.ViewModel.Reportes
{
    public class IntroduccionReportesVM
    {
        public string? FechaDesde { get; set; }
        public string? FechaHasta { get; set; }

        public List<DTO_IntroduccionReporte> ListaIntroducciones { get; set; } = new();

        public string? MensajeError { get; set; }

    }
}
