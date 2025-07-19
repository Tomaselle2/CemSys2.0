namespace CemSys2.DTO.Reportes
{
    public class DTO_IntroduccionReporte
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int Cantidad { get; set; }

        // Para gráfico de torta
        public string? TipoParcela { get; set; }
        public int CantidadPorTipo { get; set; }
    }
}
