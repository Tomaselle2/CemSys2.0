using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Seccione
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int Filas { get; set; }

    public int NroParcelas { get; set; }

    public int TipoNumeracionParcela { get; set; }

    public int TipoParcela { get; set; }

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();

    public virtual ICollection<PreciosTarifaria> PreciosTarifaria { get; set; } = new List<PreciosTarifaria>();

    public virtual TipoNumeracionParcela TipoNumeracionParcelaNavigation { get; set; } = null!;

    public virtual TipoParcela TipoParcelaNavigation { get; set; } = null!;
}
