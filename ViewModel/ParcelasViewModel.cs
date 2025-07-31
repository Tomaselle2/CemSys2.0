using CemSys2.DTO;
using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class ParcelasViewModel
    {
        public List<DTO_Parcelas> Parcelas { get; set; } = new();

        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public string? NombreSeccion { get; set; }
        public int IdSeccion { get; set; }

        public List<DTO_TipoNichos> ListaTipoNicho { get; set; } = new();
        public List<DTO_TipoPanteon> ListaTipoPanteon { get; set; } = new();


    }
}
