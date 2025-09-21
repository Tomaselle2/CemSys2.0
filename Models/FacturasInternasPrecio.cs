using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class FacturasInternasPrecio
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public decimal Total { get; set; }

    public bool Visibilidad { get; set; }

    public virtual ICollection<ConceptosFacturaInternasPrecio> ConceptosFacturaInternasPrecios { get; set; } = new List<ConceptosFacturaInternasPrecio>();

    public virtual Tramite Tramite { get; set; } = null!;
}
