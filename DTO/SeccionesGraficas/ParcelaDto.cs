namespace CemSys2.DTO.SeccionesGraficas
{
    public class ParcelaDto
    {
        public int id { get; set; }
        public bool visibilidad { get; set; }
        public int nroParcela { get; set; }
        public int nroFila { get; set; }
        public int cantidadDifuntos { get; set; }
        public int seccionId { get; set; }
        public int? tipoNichoId { get; set; }
        public int? tipoPanteonId { get; set; }
        public string nombrePanteon { get; set; }
    }
}
