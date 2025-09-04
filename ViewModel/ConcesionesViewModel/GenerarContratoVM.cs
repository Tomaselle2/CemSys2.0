using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Models;
using System.ComponentModel.DataAnnotations;


namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class GenerarContratoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();
        public DTO_Datos_Concesion DatosParcela = new DTO_Datos_Concesion();
        public List<DTO_Precios_Concesion> PreciosConcesion { get; set; } = new();
        public List<DTO_Cuotas> CantidadCuotas { get; set; } = new();

        public int? ParcelaId { get; set; }

        public string MensajeError = string.Empty;

        [Range(0, 99999, ErrorMessage = "El número de concesión no puede superar los 5 dígitos")]
        [Required(ErrorMessage = "El número de concesión es obligatorio")]
        public int? NroConcesion { get; set; }

        [Required(ErrorMessage = "La cantidad de años es obligatoria")]
        public int? PrecioSeleccionado { get; set; }

        public int? CantidadAnios { get; set; }

        [Required(ErrorMessage = "El vencimiento es obligatorio")]
        public DateOnly? Vencimiento { get; set; }

        [Required(ErrorMessage = "La forma de pago es obligatoria")]
        public string? FormaDePago { get; set; }

        public int? CantidadCuotaSeleccionada { get; set; }


        public Dictionary<int, string> CantidadAniosNicho { get; set; } =
            new Dictionary<int, string>
        {
            { 1, "1 año" },
            { 2, "5 años" },
            { 3, "10 años" },
            { 4, "15 años" },
            { 5, "25 años" }
        };

        public Dictionary<int, string> CantidadAniosFosa { get; set; } =
            new Dictionary<int, string>
        {
            { 4, "15 años" },
            { 5, "25 años" }
        };

        //Propiedad calculada que devuelve solo los precios que corresponden
        public IEnumerable<DTO_Precios_Concesion> PreciosFiltrados
        {
            get
            {
                if (DatosParcela.TipoParcela == (int)TipoParcelaEnum.Nicho)
                {
                    // Nicho -> mostrar todos los precios
                    decimal precio1anio = 0;
                    if (PreciosConcesion[1].Precio != 0) //si precio por 5 años es distinto de 0
                    {
                        precio1anio = (PreciosConcesion[1].Precio / 5) * 2; //calculo el precio por 1 año
                        PreciosConcesion.Insert(0, new DTO_Precios_Concesion //inserto en la posicion 0 el precio por 1 año
                        {
                            precioId = PreciosConcesion[0].precioId,
                            conceptoTarifariaId = PreciosConcesion[0].conceptoTarifariaId,
                            Precio = precio1anio,
                            seccionId = PreciosConcesion[0].seccionId,
                            fila = PreciosConcesion[0].fila,
                            aniosConcesion = 1
                        });
                        PreciosConcesion.Remove(PreciosConcesion[1]); //elimino el precio de 1 de $0
                    }

                    return PreciosConcesion;
                }
                else if (DatosParcela.TipoParcela == (int)TipoParcelaEnum.Fosa)
                {
                    // Fosa -> solo 15 y 25 años
                    return PreciosConcesion.Where(p => p.aniosConcesion == 15 || p.aniosConcesion == 25);
                }

                // Si no es ninguno, devolver vacío
                return new List<DTO_Precios_Concesion>();
            }
        }


    }
}
