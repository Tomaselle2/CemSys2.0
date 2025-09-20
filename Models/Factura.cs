using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Factura
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public decimal Total { get; set; }

    public bool Visibilidad { get; set; }

    public int? TipoTramiteId { get; set; }

    public int? UsuarioEmiteId { get; set; }

    public int? EstadoId { get; set; }

    public int? ContribuyenteId { get; set; }

    public int? MetodoPagoId { get; set; }

    public int? UsuarioCajeroId { get; set; }

    public virtual ICollection<ConceptosFactura> ConceptosFacturas { get; set; } = new List<ConceptosFactura>();

    public virtual Persona? Contribuyente { get; set; }

    public virtual EstadoTramite? Estado { get; set; }

    public virtual MetodoPago? MetodoPago { get; set; }

    public virtual ICollection<RecibosFactura> RecibosFacturas { get; set; } = new List<RecibosFactura>();

    public virtual TipoTramite? TipoTramite { get; set; }

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario? UsuarioCajero { get; set; }

    public virtual Usuario? UsuarioEmite { get; set; }
}
