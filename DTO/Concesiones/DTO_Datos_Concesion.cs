namespace CemSys2.DTO.Concesiones
{
    public class DTO_Datos_Concesion
    {
        public int ParcelaId { get; set; }
        public int TipoParcela { get; set; }
        public int SeccionId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public int Pendiente { get; set; }
    }

    public class DTO_Precios_Concesion
    {
        public int precioId { get; set; }
        public int conceptoTarifariaId { get; set; }
        public decimal Precio { get; set; }
        public int seccionId { get; set; }
        public int fila { get; set; }
        public int aniosConcesion { get; set; }
        public int cantidadAniosId { get; set; }  // Nueva propiedad
    }
}
