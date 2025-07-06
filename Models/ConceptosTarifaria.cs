using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class ConceptosTarifaria
{
    public int Id { get; set; }

    public int TipoConceptoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<ConceptosFactura> ConceptosFacturas { get; set; } = new List<ConceptosFactura>();

    public virtual ICollection<PreciosTarifaria> PreciosTarifaria { get; set; } = new List<PreciosTarifaria>();

    public virtual TiposConceptoTarifarium TipoConcepto { get; set; } = null!;
}
