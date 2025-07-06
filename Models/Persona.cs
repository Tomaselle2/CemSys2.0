using System;
using System.Collections.Generic;

namespace CemSys2.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public DateOnly? FechaDefuncion { get; set; }

    public int? EstadoDifunto { get; set; }

    public int? ActaDefuncion { get; set; }

    public string? InformacionAdicional { get; set; }

    public int CategoriaPersona { get; set; }

    public string Sexo { get; set; } = null!;

    public string? Correo { get; set; }

    public string? Celular { get; set; }

    public string? Domicilio { get; set; }

    public bool? DomicilioEnTirolesa { get; set; }

    public bool? FallecioEnTirolesa { get; set; }

    public virtual ActaDefuncion? ActaDefuncionNavigation { get; set; }

    public virtual ICollection<ArchivosDocumentacion> ArchivosDocumentacions { get; set; } = new List<ArchivosDocumentacion>();

    public virtual CategoriaPersona CategoriaPersonaNavigation { get; set; } = null!;

    public virtual ICollection<ContratoConcesion> ContratoConcesions { get; set; } = new List<ContratoConcesion>();

    public virtual EstadoDifunto? EstadoDifuntoNavigation { get; set; }

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntos { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<TitularesContratoConcesion> TitularesContratoConcesions { get; set; } = new List<TitularesContratoConcesion>();

    public virtual ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
