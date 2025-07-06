using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class PreciosTarifaria
{
    public int Id { get; set; }

    public int TarifarioId { get; set; }

    public int ConceptoTarifariaId { get; set; }

    public decimal Precio { get; set; }

    public int? SeccionId { get; set; }

    public int? NroFila { get; set; }

    public int? AniosConcesion { get; set; }

    public virtual ConceptosTarifaria ConceptoTarifaria { get; set; } = null!;

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();

    public virtual Seccione? Seccion { get; set; }

    public virtual Tarifaria Tarifario { get; set; } = null!;
}
