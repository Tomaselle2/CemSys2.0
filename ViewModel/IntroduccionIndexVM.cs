using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class IntroduccionIndexVM
    {
        public List<Introduccione> ListaIntroducciones { get; set; } = new();


        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }


    }
}
