using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Parcela
{
    public int Id { get; set; }

    public bool Visibilidad { get; set; }

    public int NroParcela { get; set; }

    public int NroFila { get; set; }

    public int CantidadDifuntos { get; set; }

    public int Seccion { get; set; }

    public int? TipoNicho { get; set; }

    public int? TipoPanteonId { get; set; }

    public string? NombrePanteon { get; set; }

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntos { get; set; } = new List<ParcelaDifunto>();

    public virtual Seccione SeccionNavigation { get; set; } = null!;

    public virtual TipoNicho? TipoNichoNavigation { get; set; }

    public virtual TipoPanteon? TipoPanteon { get; set; }

    public virtual ICollection<TramiteParcela> TramiteParcelas { get; set; } = new List<TramiteParcela>();
}
