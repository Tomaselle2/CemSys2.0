using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class TipoTramite
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<EstadoTramite> EstadoTramites { get; set; } = new List<EstadoTramite>();

    public virtual ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
