using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class ActaDefuncion
{
    public int Id { get; set; }

    public int? Acta { get; set; }

    public int? Tomo { get; set; }

    public int? Folio { get; set; }

    public string? Serie { get; set; }

    public int? Age { get; set; }

    public virtual ICollection<ArchivosDocumentacion> ArchivosDocumentacions { get; set; } = new List<ArchivosDocumentacion>();

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
