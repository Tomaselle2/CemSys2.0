namespace CemSys2.DTO.SeccionesGraficas
{
    public class DatosSeccionDto
    {
        public SeccionDto seccion { get; set; }
        public List<ParcelaDto> parcelas { get; set; }
        public List<DifuntoDto> difuntos { get; set; }
    }
}
