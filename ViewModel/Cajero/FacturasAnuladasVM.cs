using CemSys2.DTO.Factura;

namespace CemSys2.ViewModel.Cajero
{
    public class FacturasAnuladasVM
    {
        public List<DTO_Factura> ListaFacturasAnuladas = new List<DTO_Factura>();
        public string? MensajeError { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}
