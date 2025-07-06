using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Tramite
{
    public int Id { get; set; }

    public int TipoTramiteId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int Usuario { get; set; }

    public bool Visibilidad { get; set; }

    public int? EstadoActualId { get; set; }

    public virtual ICollection<ArchivosDocumentacion> ArchivosDocumentacions { get; set; } = new List<ArchivosDocumentacion>();

    public virtual ContratoConcesion? ContratoConcesion { get; set; }

    public virtual EstadoTramite? EstadoActual { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new List<HistorialEstadoTramite>();

    public virtual Introduccione? Introduccione { get; set; }

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntoTramiteIngresos { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntoTramiteRetiros { get; set; } = new List<ParcelaDifunto>();

    public virtual TipoTramite TipoTramite { get; set; } = null!;

    public virtual Usuario UsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
