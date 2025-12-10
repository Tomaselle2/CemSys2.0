namespace CemSys2.DTO.SeccionesGraficas
{
    public class DifuntoDto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public bool visibilidad { get; set; }
        public string fechaNacimeinto { get; set; } // Mantiene el typo del JSON
        public string fechaDefuncion { get; set; }
        public int? estadoDifuntoId { get; set; }
        public string sexo { get; set; }
        public int parcelaId { get; set; }
    }
}
