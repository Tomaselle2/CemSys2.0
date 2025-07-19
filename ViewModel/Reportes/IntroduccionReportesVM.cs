using CemSys2.DTO.Reportes;
using CemSys2.Models;

namespace CemSys2.ViewModel.Reportes
{
    public class IntroduccionReportesVM
    {
        public string? FechaDesde { get; set; }
        public string? FechaHasta { get; set; }

        public List<Introduccione> ListaIntroducciones { get; set; } = new();

        public string? MensajeError { get; set; }

    }
}
