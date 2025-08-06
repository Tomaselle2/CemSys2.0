using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class TiposConceptoTarifarium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<ConceptosFactura> ConceptosFacturas { get; set; } = new List<ConceptosFactura>();

    public virtual ICollection<ConceptosTarifaria> ConceptosTarifaria { get; set; } = new List<ConceptosTarifaria>();
}
