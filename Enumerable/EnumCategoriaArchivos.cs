using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CemSys2.Enumerable
{
    public enum CategoriaArchivosEnum
    {
        [Display(Name = "Contrato de Concesión")]
        Contrato_Concesion,

        [Display(Name = "Recibo")]
        Recibo,

        [Display(Name = "Documento de Identidad")]
        DNI,

        [Display(Name = "Acta")]
        Acta,

        [Display(Name = "Libreta de Familia")]
        Libreta_Familia,

        [Display(Name = "Otro tipo de archivo")]
        Otro
    }

    public class EnumCategoriaArchivos
    {
    }

    public static class EnumHelper
    {
        public static List<SelectListItem> ToSelectList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           Value = e.ToString(), // lo que llega al controlador
                           Text = e.GetType()
                                   .GetMember(e.ToString())
                                   .First()
                                   .GetCustomAttribute<DisplayAttribute>()?
                                   .GetName() ?? e.ToString()
                       }).ToList();
        }
    }
}
