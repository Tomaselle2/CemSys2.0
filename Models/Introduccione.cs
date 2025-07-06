using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Introduccione
{
    public int IdTramite { get; set; }

    public bool Visibilidad { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int? Empleado { get; set; }

    public int? EmpresaFunebre { get; set; }

    public int ParcelaId { get; set; }

    public int DifuntoId { get; set; }

    public string? EstadoDifunto { get; set; }

    public bool IntroduccionNueva { get; set; }

    public DateTime? FechaRetiro { get; set; }

    public virtual Persona Difunto { get; set; } = null!;

    public virtual Usuario? EmpleadoNavigation { get; set; }

    public virtual EmpresaFunebre? EmpresaFunebreNavigation { get; set; }

    public virtual Tramite IdTramiteNavigation { get; set; } = null!;

    public virtual Parcela Parcela { get; set; } = null!;
}
