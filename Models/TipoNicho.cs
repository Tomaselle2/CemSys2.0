using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class TipoNicho
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
}
