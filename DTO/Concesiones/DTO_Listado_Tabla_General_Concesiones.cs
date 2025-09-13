using Org.BouncyCastle.Asn1.Crmf;

namespace CemSys2.DTO.Concesiones
{
    public class DTO_Listado_Tabla_General_Concesiones
    {
        public int TramiteId { get; set; }
        public string NroConcesion { get; set; } = string.Empty;
        public string NombreSeccion {  get; set; } = string.Empty;
        public int TipoParcelaId { get; set; }
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public DateOnly Vencimiento { get; set; }
        public int EstadoActualId { get; set; }
        public string Difuntos { get; set; } = string.Empty;
        public string Titulares { get; set; } = string.Empty;
    }

    // DTO para manejar la paginación
    public class DTO_Listado_Paginado_Concesiones
    {
        public List<DTO_Listado_Tabla_General_Concesiones> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; }
    }
}
