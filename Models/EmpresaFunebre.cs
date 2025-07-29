using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class EmpresaFunebre
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();
}
