using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class MetodoPago
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
