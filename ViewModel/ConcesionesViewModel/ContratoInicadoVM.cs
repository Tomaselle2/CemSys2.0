using CemSys2.DTO.Concesiones;
using CemSys2.Models;
using CemSys2.ValidacionAnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class ContratoInicadoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();
        public DTO_Datos_Concesion DatosParcela = new DTO_Datos_Concesion();

        public List<SelectListItem> Categorias { get; set; } = new();  // 👈 lista de opciones


        public Factura Factura { get; set; } = new();
        public List<ConceptosFactura> ListaConceptosFactura { get; set; } = new();
        public List<RecibosFactura> ListaRecibosFactura { get; set; } = new(); //solo recibos
        public List<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new();
        public List<DTO_Archivos_Documentacion> ListaArchivos {  get; set; } = new(); //solo archivos sin recibos

        public int? ParcelaId { get; set; }
        public int? TramiteId { get; set; }
        public int? EstadoTramiteId { get; set; }

        public string? NroConcesion { get; set; }
        public int? PrecioSeleccionado { get; set; }
        public string MensajeError = string.Empty;
        public int? CantidadAniosId { get; set; }
        public DateOnly? Vencimiento { get; set; }
        public int? CantidadCuotaSeleccionada { get; set; }
        public decimal PrecioFinal { get; set; }

        //para recibo
        public bool Decreto { get; set; } = false;

        [Required(ErrorMessage = "El contribuyente es obligatorio")]
        public int? IdContribuyente { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(100, ErrorMessage = "La descripción no puede superar los 100 caracteres")]
        public string? Concepto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        public decimal? Monto { get; set; }

        public IFormFile? ArchivoRecibo { get; set; }
        public int? IdFactura { get; set; }
        public int? IdRecibo { get; set; }

        public Guid? IdArchivo { get; set; } //para editar un archivo

        public bool EsEdicion { get; set; }


        //contribuyente para cargar en la bd
        [Range(0, 99999999, ErrorMessage = "El DNI no debe tener más de 8 dígitos")]
        [Required(ErrorMessage = "El DNI es obligatorio")]
        public int? Dni { get; set; }

        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(60, ErrorMessage = "El apellido no puede superar los 60 caracteres")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría del archivo")]
        public string Categoria { get; set; } = string.Empty;   // 👈 String, no enum
    }
}
