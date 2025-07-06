using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Factura
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public decimal Total { get; set; }

    public decimal Pendiente { get; set; }

    public bool Visibilidad { get; set; }

    public virtual ICollection<ConceptosFactura> ConceptosFacturas { get; set; } = new List<ConceptosFactura>();

    public virtual ICollection<RecibosFactura> RecibosFacturas { get; set; } = new List<RecibosFactura>();

    public virtual Tramite Tramite { get; set; } = null!;
}
