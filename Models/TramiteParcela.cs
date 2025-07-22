using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class TramiteParcela
{
    public int TramiteId { get; set; }

    public int ParcelaId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
