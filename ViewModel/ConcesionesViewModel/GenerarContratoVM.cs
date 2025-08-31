using CemSys2.DTO.Concesiones;
using CemSys2.ValidacionAnotations;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.ViewModel.ConcesionesViewModel
{
    public class GenerarContratoVM
    {
        public List<DTO_Difuntos_Para_Concesion> DifuntosEnParcela = new();
        public List<DTO_Titulares> Titulares { get; set; } = new();
        public DTO_Datos_Concesion DatosParcela = new DTO_Datos_Concesion();

        public string MensajeError = string.Empty;


    }
}
