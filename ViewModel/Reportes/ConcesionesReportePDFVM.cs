namespace CemSys2.ViewModel.Reportes
{
    public class ConcesionesReportePDFVM
    {
        public string BaseUrl { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string ChartType { get; set; } = "";
        public string Frecuencia { get; set; } = "";
        public string TipoParcela { get; set; } = "";
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime RealDesde { get; set; }
        public DateTime RealHasta { get; set; }
        public string ImageBase64 { get; set; } = "";
        public string TituloReporte { get; set; } = "";
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }
}
