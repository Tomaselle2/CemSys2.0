using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class AniosConcesion
{
    public int Id { get; set; }

    public int Anios { get; set; }

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();

    public virtual ICollection<PreciosTarifaria> PreciosTarifaria { get; set; } = new List<PreciosTarifaria>();
}
