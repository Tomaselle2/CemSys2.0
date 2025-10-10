using CemSys2.DTO.Factura;

namespace CemSys2.ViewModel.Cajero
{
    public class FacturasEmitidasVM
    {
        public List<DTO_Factura> ListaFacturasEmitidas = new List<DTO_Factura>();
        public string? MensajeError { get; set; }
    }
}
