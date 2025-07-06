using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class RecibosFactura
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public DateTime FechaPago { get; set; }

    public string Concepto { get; set; } = null!;

    public decimal Monto { get; set; }

    public Guid? ArchivoId { get; set; }

    public virtual ArchivosDocumentacion? Archivo { get; set; }

    public virtual ICollection<ArchivosDocumentacion> ArchivosDocumentacions { get; set; } = new List<ArchivosDocumentacion>();

    public virtual Factura Factura { get; set; } = null!;
}
