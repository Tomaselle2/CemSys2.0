using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActaDefuncion> ActaDefuncions { get; set; }

    public virtual DbSet<AniosConcesion> AniosConcesions { get; set; }

    public virtual DbSet<ArchivosDocumentacion> ArchivosDocumentacions { get; set; }

    public virtual DbSet<CantidadCuota> CantidadCuotas { get; set; }

    public virtual DbSet<CategoriaPersona> CategoriaPersonas { get; set; }

    public virtual DbSet<Cementerio> Cementerios { get; set; }

    public virtual DbSet<ConceptosFactura> ConceptosFacturas { get; set; }

    public virtual DbSet<ConceptosTarifaria> ConceptosTarifarias { get; set; }

    public virtual DbSet<ContratoConcesion> ContratoConcesions { get; set; }

    public virtual DbSet<EmpresaFunebre> EmpresaFunebres { get; set; }

    public virtual DbSet<EstadoDifunto> EstadoDifuntos { get; set; }

    public virtual DbSet<EstadoTramite> EstadoTramites { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<HistorialEstadoTramite> HistorialEstadoTramites { get; set; }

    public virtual DbSet<Introduccione> Introducciones { get; set; }

    public virtual DbSet<Parcela> Parcelas { get; set; }

    public virtual DbSet<ParcelaDifunto> ParcelaDifuntos { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PreciosTarifaria> PreciosTarifarias { get; set; }

    public virtual DbSet<RecibosFactura> RecibosFacturas { get; set; }

    public virtual DbSet<RolesUsuario> RolesUsuarios { get; set; }

    public virtual DbSet<Seccione> Secciones { get; set; }

    public virtual DbSet<Tarifaria> Tarifarias { get; set; }

    public virtual DbSet<TipoNicho> TipoNichos { get; set; }

    public virtual DbSet<TipoNumeracionParcela> TipoNumeracionParcelas { get; set; }

    public virtual DbSet<TipoPanteon> TipoPanteons { get; set; }

    public virtual DbSet<TipoParcela> TipoParcelas { get; set; }

    public virtual DbSet<TipoTramite> TipoTramites { get; set; }

    public virtual DbSet<TiposConceptoTarifarium> TiposConceptoTarifaria { get; set; }

    public virtual DbSet<TitularesContratoConcesion> TitularesContratoConcesions { get; set; }

    public virtual DbSet<Tramite> Tramites { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActaDefuncion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActaDefu__3213E83F97D8FECF");

            entity.ToTable("ActaDefuncion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Acta).HasColumnName("acta");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.Serie)
                .HasMaxLength(10)
                .HasColumnName("serie");
            entity.Property(e => e.Tomo).HasColumnName("tomo");
        });

        modelBuilder.Entity<AniosConcesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AniosCon__3213E83FC21622A3");

            entity.ToTable("AniosConcesion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anios).HasColumnName("anios");
        });

        modelBuilder.Entity<ArchivosDocumentacion>(entity =>
        {
            entity.HasKey(e => e.ArchivoId).HasName("PK__Archivos__3D24276A8CC79C2D");

            entity.ToTable("ArchivosDocumentacion");

            entity.Property(e => e.ArchivoId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ArchivoID");
            entity.Property(e => e.ActaDefuncionId).HasColumnName("ActaDefuncionID");
            entity.Property(e => e.CategoriaArchivo).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NombreArchivo).HasMaxLength(255);
            entity.Property(e => e.PersonaId).HasColumnName("PersonaID");
            entity.Property(e => e.ReciboId).HasColumnName("ReciboID");
            entity.Property(e => e.TipoArchivo).HasMaxLength(50);
            entity.Property(e => e.TramiteId).HasColumnName("TramiteID");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.ActaDefuncion).WithMany(p => p.ArchivosDocumentacions)
                .HasForeignKey(d => d.ActaDefuncionId)
                .HasConstraintName("FK__ArchivosD__ActaD__282DF8C2");

            entity.HasOne(d => d.Persona).WithMany(p => p.ArchivosDocumentacions)
                .HasForeignKey(d => d.PersonaId)
                .HasConstraintName("FK__ArchivosD__Perso__29221CFB");

            entity.HasOne(d => d.Recibo).WithMany(p => p.ArchivosDocumentacions)
                .HasForeignKey(d => d.ReciboId)
                .HasConstraintName("FK__ArchivosD__Recib__2739D489");

            entity.HasOne(d => d.Tramite).WithMany(p => p.ArchivosDocumentacions)
                .HasForeignKey(d => d.TramiteId)
                .HasConstraintName("FK__ArchivosD__Trami__2645B050");
        });

        modelBuilder.Entity<CantidadCuota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cantidad__3213E83F59DCCEE2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cuota).HasColumnName("cuota");
        });

        modelBuilder.Entity<CategoriaPersona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3213E83F094192A2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .HasColumnName("categoria");
        });

        modelBuilder.Entity<Cementerio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cementer__3213E83F09BA4D64");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<ConceptosFactura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Concepto__3213E83F110A5E62");

            entity.ToTable("ConceptosFactura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(1)
                .HasColumnName("cantidad");
            entity.Property(e => e.ConceptoTarifariaId).HasColumnName("conceptoTarifariaId");
            entity.Property(e => e.FacturaId).HasColumnName("facturaId");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioUnitario");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([precioUnitario]*[cantidad])", true)
                .HasColumnType("decimal(21, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.ConceptoTarifaria).WithMany(p => p.ConceptosFacturas)
                .HasForeignKey(d => d.ConceptoTarifariaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conceptos__conce__1DB06A4F");

            entity.HasOne(d => d.Factura).WithMany(p => p.ConceptosFacturas)
                .HasForeignKey(d => d.FacturaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conceptos__factu__1CBC4616");
        });

        modelBuilder.Entity<ConceptosTarifaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Concepto__3213E83F0585AA83");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoConceptoId).HasColumnName("tipoConceptoId");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.TipoConcepto).WithMany(p => p.ConceptosTarifaria)
                .HasForeignKey(d => d.TipoConceptoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conceptos__tipoC__619B8048");
        });

        modelBuilder.Entity<ContratoConcesion>(entity =>
        {
            entity.HasKey(e => e.IdTramite).HasName("PK__Contrato__5B86469854ED1385");

            entity.ToTable("ContratoConcesion");

            entity.Property(e => e.IdTramite)
                .ValueGeneratedNever()
                .HasColumnName("idTramite");
            entity.Property(e => e.CantidadAnios).HasColumnName("cantidadAnios");
            entity.Property(e => e.Concesion)
                .HasMaxLength(5)
                .HasColumnName("concesion");
            entity.Property(e => e.CuotaId).HasColumnName("cuotaId");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.Empleado).HasColumnName("empleado");
            entity.Property(e => e.FechaGeneracion)
                .HasColumnType("datetime")
                .HasColumnName("fechaGeneracion");
            entity.Property(e => e.PagoDescripcion)
                .HasMaxLength(150)
                .HasColumnName("pagoDescripcion");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.PrecioTarifariaId).HasColumnName("precioTarifariaID");
            entity.Property(e => e.TipoParcela).HasColumnName("tipoParcela");
            entity.Property(e => e.Vencimiento)
                .HasColumnType("datetime")
                .HasColumnName("vencimiento");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.CantidadAniosNavigation).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.CantidadAnios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__canti__02084FDA");

            entity.HasOne(d => d.Cuota).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.CuotaId)
                .HasConstraintName("FK__ContratoC__cuota__03F0984C");

            entity.HasOne(d => d.Difunto).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__difun__00200768");

            entity.HasOne(d => d.EmpleadoNavigation).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.Empleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__emple__04E4BC85");

            entity.HasOne(d => d.IdTramiteNavigation).WithOne(p => p.ContratoConcesion)
                .HasForeignKey<ContratoConcesion>(d => d.IdTramite)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__idTra__7F2BE32F");

            entity.HasOne(d => d.Parcela).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__parce__01142BA1");

            entity.HasOne(d => d.PrecioTarifaria).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.PrecioTarifariaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__preci__02FC7413");

            entity.HasOne(d => d.TipoParcelaNavigation).WithMany(p => p.ContratoConcesions)
                .HasForeignKey(d => d.TipoParcela)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContratoC__tipoP__05D8E0BE");
        });

        modelBuilder.Entity<EmpresaFunebre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmpresaF__3213E83F42DA4D51");

            entity.ToTable("EmpresaFunebre");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoDifunto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadoDi__3213E83FF71F0FA7");

            entity.ToTable("EstadoDifunto");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasColumnName("estado");
        });

        modelBuilder.Entity<EstadoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadoTr__3213E83F7C0C7EDE");

            entity.ToTable("EstadoTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteId");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.EstadoTramites)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EstadoTra__tipoT__6477ECF3");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Facturas__3213E83F1CD526C9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.Pendiente)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("pendiente");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Tramite).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Facturas__tramit__18EBB532");
        });

        modelBuilder.Entity<HistorialEstadoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3213E83F39DEF72B");

            entity.ToTable("HistorialEstadoTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstadoTramiteId).HasColumnName("estadoTramiteID");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteID");

            entity.HasOne(d => d.EstadoTramite).WithMany(p => p.HistorialEstadoTramites)
                .HasForeignKey(d => d.EstadoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__estad__75A278F5");

            entity.HasOne(d => d.Tramite).WithMany(p => p.HistorialEstadoTramites)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__trami__74AE54BC");
        });

        modelBuilder.Entity<Introduccione>(entity =>
        {
            entity.HasKey(e => e.IdTramite).HasName("PK__Introduc__5B8646980117CC08");

            entity.Property(e => e.IdTramite)
                .ValueGeneratedNever()
                .HasColumnName("idTramite");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoID");
            entity.Property(e => e.Empleado).HasColumnName("empleado");
            entity.Property(e => e.EmpresaFunebre).HasColumnName("empresaFunebre");
            entity.Property(e => e.EstadoDifunto)
                .HasMaxLength(30)
                .HasColumnName("estadoDifunto");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngreso");
            entity.Property(e => e.FechaRetiro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRetiro");
            entity.Property(e => e.IntroduccionNueva).HasColumnName("introduccionNueva");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaID");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.Difunto).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Introducc__difun__7C4F7684");

            entity.HasOne(d => d.EmpleadoNavigation).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.Empleado)
                .HasConstraintName("FK__Introducc__emple__797309D9");

            entity.HasOne(d => d.EmpresaFunebreNavigation).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.EmpresaFunebre)
                .HasConstraintName("FK__Introducc__empre__7A672E12");

            entity.HasOne(d => d.IdTramiteNavigation).WithOne(p => p.Introduccione)
                .HasForeignKey<Introduccione>(d => d.IdTramite)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Introducc__idTra__787EE5A0");

            entity.HasOne(d => d.Parcela).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Introducc__parce__7B5B524B");
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Parcela__3213E83F162E714D");

            entity.ToTable("Parcela");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadDifuntos).HasColumnName("cantidadDifuntos");
            entity.Property(e => e.Seccion).HasColumnName("seccion");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.SeccionNavigation).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.Seccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Parcela__seccion__6754599E");

            entity.HasOne(d => d.TipoNichoNavigation).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.TipoNicho)
                .HasConstraintName("FK__Parcela__TipoNic__68487DD7");

            entity.HasOne(d => d.TipoPanteon).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.TipoPanteonId)
                .HasConstraintName("FK_Parcela_TipoPanteon");
        });

        modelBuilder.Entity<ParcelaDifunto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ParcelaD__3213E83F28E54CB7");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.EstadoActual)
                .HasDefaultValue(true)
                .HasColumnName("estadoActual");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngreso");
            entity.Property(e => e.FechaRetiro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRetiro");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.TramiteIngresoId).HasColumnName("tramiteIngresoId");
            entity.Property(e => e.TramiteRetiroId).HasColumnName("tramiteRetiroId");

            entity.HasOne(d => d.Difunto).WithMany(p => p.ParcelaDifuntos)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParcelaDi__difun__123EB7A3");

            entity.HasOne(d => d.Parcela).WithMany(p => p.ParcelaDifuntos)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParcelaDi__parce__114A936A");

            entity.HasOne(d => d.TramiteIngreso).WithMany(p => p.ParcelaDifuntoTramiteIngresos)
                .HasForeignKey(d => d.TramiteIngresoId)
                .HasConstraintName("FK__ParcelaDi__trami__1332DBDC");

            entity.HasOne(d => d.TramiteRetiro).WithMany(p => p.ParcelaDifuntoTramiteRetiros)
                .HasForeignKey(d => d.TramiteRetiroId)
                .HasConstraintName("FK__ParcelaDi__trami__14270015");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__Personas__A47881419E2A7781");

            entity.Property(e => e.IdPersona).HasColumnName("idPersona");
            entity.Property(e => e.ActaDefuncion).HasColumnName("actaDefuncion");
            entity.Property(e => e.Apellido)
                .HasMaxLength(60)
                .HasColumnName("apellido");
            entity.Property(e => e.CategoriaPersona).HasColumnName("categoriaPersona");
            entity.Property(e => e.Celular)
                .HasMaxLength(25)
                .HasColumnName("celular");
            entity.Property(e => e.Correo)
                .HasMaxLength(60)
                .HasColumnName("correo");
            entity.Property(e => e.Dni)
                .HasMaxLength(15)
                .HasColumnName("dni");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(100)
                .HasColumnName("domicilio");
            entity.Property(e => e.DomicilioEnTirolesa).HasColumnName("domicilioEnTirolesa");
            entity.Property(e => e.EstadoDifunto).HasColumnName("estadoDifunto");
            entity.Property(e => e.FallecioEnTirolesa).HasColumnName("fallecioEnTirolesa");
            entity.Property(e => e.FechaDefuncion).HasColumnName("fechaDefuncion");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fechaNacimiento");
            entity.Property(e => e.InformacionAdicional).HasColumnName("informacionAdicional");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.Sexo)
                .HasMaxLength(15)
                .HasColumnName("sexo");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.ActaDefuncionNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.ActaDefuncion)
                .HasConstraintName("FK__Personas__actaDe__5CD6CB2B");

            entity.HasOne(d => d.CategoriaPersonaNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.CategoriaPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Personas__catego__5DCAEF64");

            entity.HasOne(d => d.EstadoDifuntoNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.EstadoDifunto)
                .HasConstraintName("FK__Personas__estado__5BE2A6F2");
        });

        modelBuilder.Entity<PreciosTarifaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PreciosT__3213E83FD2ECCA6F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AniosConcesion).HasColumnName("aniosConcesion");
            entity.Property(e => e.ConceptoTarifariaId).HasColumnName("conceptoTarifariaId");
            entity.Property(e => e.NroFila).HasColumnName("nroFila");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.SeccionId).HasColumnName("seccionId");
            entity.Property(e => e.TarifarioId).HasColumnName("tarifarioId");

            entity.HasOne(d => d.ConceptoTarifaria).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.ConceptoTarifariaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PreciosTa__conce__6C190EBB");

            entity.HasOne(d => d.Seccion).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.SeccionId)
                .HasConstraintName("FK__PreciosTa__secci__6D0D32F4");

            entity.HasOne(d => d.Tarifario).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.TarifarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PreciosTa__tarif__6B24EA82");
        });

        modelBuilder.Entity<RecibosFactura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RecibosF__3213E83F93B5C739");

            entity.ToTable("RecibosFactura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArchivoId).HasColumnName("archivoID");
            entity.Property(e => e.Concepto)
                .HasMaxLength(100)
                .HasColumnName("concepto");
            entity.Property(e => e.FacturaId).HasColumnName("facturaId");
            entity.Property(e => e.FechaPago)
                .HasColumnType("datetime")
                .HasColumnName("fechaPago");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            entity.HasOne(d => d.Archivo).WithMany(p => p.RecibosFacturas)
                .HasForeignKey(d => d.ArchivoId)
                .HasConstraintName("FK_RecibosFactura_ArchivosDocumentacion");

            entity.HasOne(d => d.Factura).WithMany(p => p.RecibosFacturas)
                .HasForeignKey(d => d.FacturaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecibosFa__factu__208CD6FA");
        });

        modelBuilder.Entity<RolesUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolesUsu__3213E83FB4329151");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<Seccione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seccione__3213E83F97DA2CEF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Filas).HasColumnName("filas");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NroParcelas).HasColumnName("nroParcelas");
            entity.Property(e => e.TipoNumeracionParcela).HasColumnName("tipoNumeracionParcela");
            entity.Property(e => e.TipoParcela).HasColumnName("tipoParcela");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.TipoNumeracionParcelaNavigation).WithMany(p => p.Secciones)
                .HasForeignKey(d => d.TipoNumeracionParcela)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Secciones__tipoN__5812160E");

            entity.HasOne(d => d.TipoParcelaNavigation).WithMany(p => p.Secciones)
                .HasForeignKey(d => d.TipoParcela)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Secciones__tipoP__59063A47");
        });

        modelBuilder.Entity<Tarifaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tarifari__3213E83FAA9C14C6");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaCreacionTarifaria)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");
        });

        modelBuilder.Entity<TipoNicho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNich__3213E83F5FEFD0F5");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoNumeracionParcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNume__3213E83F5AD662BB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TipoNumeracion)
                .HasMaxLength(30)
                .HasColumnName("tipoNumeracion");
        });

        modelBuilder.Entity<TipoPanteon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoPant__3213E83F367559C8");

            entity.ToTable("TipoPanteon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoParcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoParc__3213E83FB9B7483C");

            entity.ToTable("TipoParcela");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TipoParcela1)
                .HasMaxLength(30)
                .HasColumnName("tipoParcela");
        });

        modelBuilder.Entity<TipoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoTram__3213E83F6AE28B78");

            entity.ToTable("TipoTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(30)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TiposConceptoTarifarium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TiposCon__3213E83FF89E108A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TitularesContratoConcesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Titulare__3213E83F4D44A19A");

            entity.ToTable("TitularesContratoConcesion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContratoId).HasColumnName("contratoId");
            entity.Property(e => e.PersonaId).HasColumnName("personaId");

            entity.HasOne(d => d.Contrato).WithMany(p => p.TitularesContratoConcesions)
                .HasForeignKey(d => d.ContratoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Titulares__contr__08B54D69");

            entity.HasOne(d => d.Persona).WithMany(p => p.TitularesContratoConcesions)
                .HasForeignKey(d => d.PersonaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Titulares__perso__09A971A2");
        });

        modelBuilder.Entity<Tramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tramite__3213E83F8D28456E");

            entity.ToTable("Tramite");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EstadoActualId).HasColumnName("estadoActualID");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteID");
            entity.Property(e => e.Usuario).HasColumnName("usuario");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.EstadoActual).WithMany(p => p.Tramites)
                .HasForeignKey(d => d.EstadoActualId)
                .HasConstraintName("FK__Tramite__estadoA__71D1E811");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.Tramites)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tramite__tipoTra__6FE99F9F");

            entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.Tramites)
                .HasForeignKey(d => d.Usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tramite__usuario__70DDC3D8");

            entity.HasMany(d => d.Personas).WithMany(p => p.Tramites)
                .UsingEntity<Dictionary<string, object>>(
                    "TramitePersona",
                    r => r.HasOne<Persona>().WithMany()
                        .HasForeignKey("PersonaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TramitePe__perso__0D7A0286"),
                    l => l.HasOne<Tramite>().WithMany()
                        .HasForeignKey("TramiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TramitePe__trami__0C85DE4D"),
                    j =>
                    {
                        j.HasKey("TramiteId", "PersonaId").HasName("PK__TramiteP__770E40CABA4F0FA1");
                        j.ToTable("TramitePersonas");
                        j.IndexerProperty<int>("TramiteId").HasColumnName("tramiteId");
                        j.IndexerProperty<int>("PersonaId").HasColumnName("personaId");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3213E83F66A933ED");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Clave)
                .HasMaxLength(300)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(60)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol).HasColumnName("rol");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(30)
                .HasColumnName("usuario");
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Rol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__rol__5535A963");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
