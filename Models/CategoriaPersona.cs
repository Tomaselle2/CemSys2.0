using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class CategoriaPersona
{
    public int Id { get; set; }

    public string Categoria { get; set; } = null!;

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
