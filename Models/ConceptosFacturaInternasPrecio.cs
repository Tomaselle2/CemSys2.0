using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class ConceptosFacturaInternasPrecio
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int ConceptoTarifariaId { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int Cantidad { get; set; }

    public int? TipoConceptoFacturaId { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual ConceptosTarifaria ConceptoTarifaria { get; set; } = null!;

    public virtual FacturasInternasPrecio Factura { get; set; } = null!;

    public virtual TiposConceptoTarifarium? TipoConceptoFactura { get; set; }
}
