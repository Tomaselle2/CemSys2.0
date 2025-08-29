using CemSys2.DTO.Concesiones;

namespace CemSys2.ViewModel.ContratoViewModel
{
    public class IndexConcesionesVM
    {
        public List<DTO_Parcelas_Sin_Contrato> ListaParcelasSinContrato = new();
        public string MensajeError { get; set; } = string.Empty;
    }
}
