using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class HistorialTitularesContrato
{
    public int Id { get; set; }

    public int ContratoId { get; set; }

    public int PersonaId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public virtual ContratoConcesion Contrato { get; set; } = null!;

    public virtual Persona Persona { get; set; } = null!;
}
