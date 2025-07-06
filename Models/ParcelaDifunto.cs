using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class ParcelaDifunto
{
    public int Id { get; set; }

    public int ParcelaId { get; set; }

    public int DifuntoId { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaRetiro { get; set; }

    public bool EstadoActual { get; set; }

    public int? TramiteIngresoId { get; set; }

    public int? TramiteRetiroId { get; set; }

    public virtual Persona Difunto { get; set; } = null!;

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite? TramiteIngreso { get; set; }

    public virtual Tramite? TramiteRetiro { get; set; }
}
