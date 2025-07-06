using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class HistorialEstadoTramite
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public int EstadoTramiteId { get; set; }

    public DateTime Fecha { get; set; }

    public virtual EstadoTramite EstadoTramite { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
