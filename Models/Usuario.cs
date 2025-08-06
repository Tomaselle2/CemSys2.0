using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int Rol { get; set; }

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();

    public virtual RolesUsuario RolNavigation { get; set; } = null!;

    public virtual ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
