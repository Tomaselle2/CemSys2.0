using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class CantidadCuota
{
    public int Id { get; set; }

    public int Cuota { get; set; }

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();
}
