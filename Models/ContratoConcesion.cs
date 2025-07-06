using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class ContratoConcesion
{
    public int IdTramite { get; set; }

    public int DifuntoId { get; set; }

    public int ParcelaId { get; set; }

    public int CantidadAnios { get; set; }

    public DateTime Vencimiento { get; set; }

    public string Concesion { get; set; } = null!;

    public int PrecioTarifariaId { get; set; }

    public int? CuotaId { get; set; }

    public string? PagoDescripcion { get; set; }

    public bool Visibilidad { get; set; }

    public DateTime FechaGeneracion { get; set; }

    public int Empleado { get; set; }

    public int TipoParcela { get; set; }

    public virtual AniosConcesion CantidadAniosNavigation { get; set; } = null!;

    public virtual CantidadCuota? Cuota { get; set; }

    public virtual Persona Difunto { get; set; } = null!;

    public virtual Usuario EmpleadoNavigation { get; set; } = null!;

    public virtual Tramite IdTramiteNavigation { get; set; } = null!;

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual PreciosTarifaria PrecioTarifaria { get; set; } = null!;

    public virtual TipoParcela TipoParcelaNavigation { get; set; } = null!;

    public virtual ICollection<TitularesContratoConcesion> TitularesContratoConcesions { get; set; } = new List<TitularesContratoConcesion>();
}
