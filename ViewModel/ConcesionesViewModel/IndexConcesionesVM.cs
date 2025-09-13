using CemSys2.DTO.Concesiones;

namespace CemSys2.ViewModel.ContratoViewModel
{
    public class IndexConcesionesVM
    {
        public List<DTO_Parcelas_Sin_Contrato> ListaParcelasSinContrato = new();
        public List<DTO_Listado_Tabla_General_Concesiones> ListaConcesiones { get; set; } = new();

        public string MensajeError { get; set; } = string.Empty;

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamanoPagina { get; set; } = 10; // valor por defecto

    }
}
