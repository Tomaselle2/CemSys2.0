using CemSys2.Models;

namespace CemSys2.ViewModel
{
    public class IntroduccionIndexVM
    {
        public List<Introduccione> ListaIntroducciones { get; set; } = new();
        // lista estado difunto 
        //acta defuncion
        //lista tipo parcela
        //Lista de secciones
        //lista de parcelas
        //lista de empresas de sepelio

        public string? MensajeError { get; set; }
        public string? Redirigir { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }


    }
}
