using System.ComponentModel.DataAnnotations;

namespace CemSys2.Enumerable
{
    public enum MetodoPagoEnum
    {
        [Display(Name = "------")]
        Ninguno = 0,

        [Display(Name = "Efectivo")]
        Efectivo = 1,

        [Display(Name = "Tarjeta")]
        Tarjeta = 2,

        [Display(Name = "QR")]
        QR = 3
    }
}
