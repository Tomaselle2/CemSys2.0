using CemSys2.DTO;
using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class ParcelasViewModel
    {
        //public int Id { get; set; }

        //public bool Visibilidad { get; set; }

        //public int NroParcela { get; set; }

        //public int NroFila { get; set; }

        //public int CantidadDifuntos { get; set; }

        //public int Seccion { get; set; }

        //public int? IdTipoNicho { get; set; }

        //public string? TipoNicho { get; set; }

        public List<DTO_Parcelas> Parcelas { get; set; } = new();

        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public string? NombreSeccion { get; set; }
        public int IdSeccion { get; set; }

        public List<DTO_TipoNichos> ListaTipoNicho { get; set; } = new();

    }
}
