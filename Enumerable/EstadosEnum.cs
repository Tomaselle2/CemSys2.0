using CemSys2.Models;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.Enumerable
{
    public enum EstadosIntroduccion
    {
        Registrado = 1,
        Cobrado = 2,
        Finalizado = 3
    }

    public enum TipotamiteEmun
    {
        [Display(Name = "Introducción")]
        Introduccion = 1,

        [Display(Name = "Autorización para cremación")]
        AutorizacionParaCremacion = 2,

        [Display(Name = "Autorización para reducción")]
        AutorizacionParaReduccion = 3,

        [Display(Name = "Contrato de concesión")]
        ContratoDeConcesion = 4,

        [Display(Name = "Autorización para traslado")]
        AutorizacionParaTraslado = 5,

        [Display(Name = "Cambio de titularidad")]
        CambioDeTitularidad = 6
    }


    public static class EstadosEnum
    {
        private static readonly Dictionary<TipotamiteEmun, Dictionary<int, string>> _estadosPorTramite =
        new Dictionary<TipotamiteEmun, Dictionary<int, string>>
        {
            {
                TipotamiteEmun.Introduccion,
                new Dictionary<int, string>
                {
                    { 1, "Registrado" },
                    { 2, "Cobrado" },
                    { 3, "Finalizado" }
                }
            },
            {
                TipotamiteEmun.ContratoDeConcesion,
                new Dictionary<int, string>
                {
                    { 4, "Iniciado" },
                    { 5, "Pendiente de documentación" },
                    { 6, "Activa" },
                    { 7, "Vencida" },
                    { 8, "Inactiva" },
                    { 9, "Renovación" }
                }
            }
            //{
            //    TipoTramite.AutorizacionParaReduccion,
            //    new Dictionary<int, string>
            //    {
            //        { 1, "Iniciado" },
            //        { 2, "Documentación completa" },
            //        { 3, "Autorizado" }
            //    }
            //},
            // Agregar más trámites según necesites
        };

        public static string ObtenerNombreEstado(TipotamiteEmun tipoTramite, int estadoId)
        {
            if (_estadosPorTramite.ContainsKey(tipoTramite) &&
                _estadosPorTramite[tipoTramite].ContainsKey(estadoId))
            {
                return _estadosPorTramite[tipoTramite][estadoId];
            }
            return $"Estado {estadoId}"; // Fallback
        }

        public static string ObtenerDisplayTipoTramite(TipotamiteEmun tipo)
        {
            var field = tipo.GetType().GetField(tipo.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute?.Name ?? tipo.ToString();
        }


    }
}


