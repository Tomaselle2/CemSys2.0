using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class ArchivosDocumentacion
{
    public Guid ArchivoId { get; set; }

    public string CategoriaArchivo { get; set; } = null!;

    public int? TramiteId { get; set; }

    public int? ReciboId { get; set; }

    public int? ActaDefuncionId { get; set; }

    public int? PersonaId { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string TipoArchivo { get; set; } = null!;

    public long TamanoBytes { get; set; }

    public byte[] Contenido { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Visibilidad { get; set; }

    public virtual ActaDefuncion? ActaDefuncion { get; set; }

    public virtual Persona? Persona { get; set; }

    public virtual RecibosFactura? Recibo { get; set; }

    public virtual ICollection<RecibosFactura> RecibosFacturas { get; set; } = new List<RecibosFactura>();

    public virtual Tramite? Tramite { get; set; }
}
