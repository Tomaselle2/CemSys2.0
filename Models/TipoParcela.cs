using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class TipoParcela
{
    public int Id { get; set; }

    public string TipoParcela1 { get; set; } = null!;

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();

    public virtual ICollection<Seccione> Secciones { get; set; } = new List<Seccione>();
}
