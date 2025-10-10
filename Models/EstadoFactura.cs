using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class EstadoFactura
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<HistorialEstadosFactura> HistorialEstadosFacturas { get; set; } = new List<HistorialEstadosFactura>();
}
