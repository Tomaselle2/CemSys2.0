using CemSys2.DTO.Concesiones;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class GenerarContratoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public string MensajeError = string.Empty;
    }
}
