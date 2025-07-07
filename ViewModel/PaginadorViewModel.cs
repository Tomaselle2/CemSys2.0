namespace CemSys2.ViewModel
{
    public class PaginadorViewModel
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public string? Accion { get; set; }
        public string? Controlador { get; set; }
    }
}
