using CemSys2.DTO.Concesiones;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class ContratoPDF_VM
    {
        public DTO_DatosGenerarContratoConcesion datosContrato = new();
        public string baseUrl = string.Empty;
        public string PrecioEnLetras = string.Empty;
    }
}
