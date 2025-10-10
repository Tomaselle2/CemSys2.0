using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class HistorialEstadosFactura
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int EstadoId { get; set; }

    public DateTime FechaCambio { get; set; }

    public virtual EstadoFactura Estado { get; set; } = null!;

    public virtual Factura Factura { get; set; } = null!;
}
