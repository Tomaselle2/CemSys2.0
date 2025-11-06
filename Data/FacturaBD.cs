using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Enumerable;
using CemSys2.Interface.Facturas;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Data
{
    public class FacturaBD : IFacturasBD
    {
        private AppDbContext _context;

        public FacturaBD(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<int> RegistrarConceptoFactura(ConceptosFactura concepto)
        {
            _context.ConceptosFacturas.Add(concepto);
            await _context.SaveChangesAsync();
            return concepto.Id;
        }

        public async Task<int> RegistrarFactura(Factura factura) //devuelve el id de la factura
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();
            return factura.Id;
        }

        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _context.Facturas
                .FirstAsync(f => f.TramiteId == idTramite);
        }

        public async Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura)
        {
            return await _context.ConceptosFacturas
                .Include(c => c.ConceptoTarifaria)
                .Where(c => c.FacturaId == idFactura)
                .ToListAsync();
        }

        public async Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId)
        {
            return await _context.RecibosFacturas.Include(p => p.ContribuyenteNavigation).Include(a => a.Archivo).Where(f => f.FacturaId == facturaId).OrderByDescending(t => t.FechaPago).ToListAsync();
        }

        public async Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️ Insertar ReciboFactura
                var reciboFactura = new RecibosFactura
                {
                    FacturaId = recibo.FacturaId,
                    FechaPago = DateTime.Now,
                    Concepto = recibo.Concepto!,
                    Monto = recibo.Monto,
                    Decreto = recibo.Decreto,
                    Contribuyente = recibo.Contribuyente
                };
                _context.RecibosFacturas.Add(reciboFactura);
                await _context.SaveChangesAsync();

                // 2️ Insertar archivo en ArchivosDocumentacion (FILESTREAM)
                byte[] contenido;
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }

                CategoriaArchivosEnum categoriaArchivo = CategoriaArchivosEnum.Recibo;
                var archivoRecibo = new ArchivosDocumentacion
                {
                    CategoriaArchivo = categoriaArchivo.ToString(),
                    TramiteId = tramiteId,
                    NombreArchivo = Path.GetFileName(archivo.FileName),
                    TipoArchivo = mimeType,
                    TamanoBytes = archivo.Length,
                    Contenido = contenido,
                    Descripcion = $"Recibo {recibo.Id} - Factura {reciboFactura.FacturaId}",
                    FechaCreacion = DateTime.Now,
                    Visibilidad = true,
                };
                _context.ArchivosDocumentacions.Add(archivoRecibo);
                await _context.SaveChangesAsync();

                // 3️⃣ Actualizar FK del Recibo con el archivoID
                reciboFactura.ArchivoId = archivoRecibo.ArchivoId;
                _context.RecibosFacturas.Update(reciboFactura);
                await _context.SaveChangesAsync();


                //busco la factura
                Factura factura = await _context.Facturas.FirstAsync(f => f.Id == reciboFactura.FacturaId);

                //busco el tramite
                Tramite tramite = await _context.Tramites.FirstAsync(t => t.Id == tramiteId);

                //if (factura != null)
                //{
                //    //resto del monto que llega, nunca puede ser mayor que el pendiente
                //    factura.Pendiente = factura.Pendiente - reciboFactura.Monto;

                //    if (factura.Pendiente <= 0) //se abono todo
                //    {
                //        //actualizo la factura
                //        factura.Pendiente = 0;
                //        _context.Facturas.Update(factura);
                //        await _context.SaveChangesAsync();
                //    }
                //    else
                //    {
                //        _context.Facturas.Update(factura);
                //        await _context.SaveChangesAsync();
                //    }
                //}

                // ✅ VERIFICAR SI LA RELACIÓN TRÁMITE-PERSONA YA EXISTE
                bool relacionExistente = await _context.TramitePersonas
                    .AnyAsync(tp => tp.TramiteId == tramite.Id && tp.PersonaId == recibo.Contribuyente.Value);

                // Solo crear la relación si no existe
                if (!relacionExistente)
                {
                    TramitePersona tramitePersona = new TramitePersona
                    {
                        TramiteId = tramite.Id,
                        PersonaId = recibo.Contribuyente.Value
                    };
                    _context.TramitePersonas.Add(tramitePersona);
                    await _context.SaveChangesAsync();
                }


                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<DTO_ConceptosTarifaria>> ListaConceptoTarifariaIntroduccion(int tarifariaId)
        {
            return await(
                from pt in _context.PreciosTarifarias
                join ct in _context.ConceptosTarifarias
                    on pt.ConceptoTarifariaId equals ct.Id
                where pt.TarifarioId == tarifariaId
                      && (ct.TipoConceptoId == 1
                          || ct.TipoConceptoId == 2
                          || ct.TipoConceptoId == 5
                          || ct.TipoConceptoId == 6)
                    select new DTO_ConceptosTarifaria
                    {
                        PrecioId = pt.Id,
                        TarifariaId = pt.TarifarioId,
                        ConceptoTarifariaId = pt.ConceptoTarifariaId,
                        Precio = pt.Precio,
                        TipoConceptoTarifariaId = ct.TipoConceptoId,
                        NombreConcepto = ct.Nombre
                    }
            ).ToListAsync();
        }

        //para resumen introducccion
        public async Task<DTO_FacturaInternaPrecios> ConsultarFacturaInternaPorTramiteId(int idTramite)
        {
            return await _context.FacturasInternasPrecios
                .Where(f => f.TramiteId == idTramite).Select(f => new DTO_FacturaInternaPrecios
                {
                    TramiteId = f.TramiteId,
                    FechaCreacion = f.FechaCreacion,
                    Id = f.Id,
                    Total = f.Total,
                    Visibilidad = f.Visibilidad
                }).FirstAsync();
        }

        //para resumen introducccion
        public async Task<List<ConceptosFacturaInternasPrecio>> ListaConceptosFacturaInternaPorFactura(int idFactura)
        {
            return await _context.ConceptosFacturaInternasPrecios
                .Include(c => c.ConceptoTarifaria)
                .Where(c => c.FacturaId == idFactura)
                .ToListAsync();
        }

        //verifica las facturas emitidas y pendientes por tramite
        public async Task<List<DTO_VerificarMontoFactura>> ListaFacturasEmitidasYPendientesParaVerificarPorTramite(int tramiteId)
        {
            List<DTO_VerificarMontoFactura> dto = new List<DTO_VerificarMontoFactura>();

             dto = await (from f in _context.Facturas
                   where f.TramiteId == tramiteId
                     && (f.EstadoId == (int)EstadosFactura.Emitido || f.EstadoId == (int)EstadosFactura.PendienteDeCobro)
                   select new DTO_VerificarMontoFactura
                   {
                       FacturaId = f.Id,
                       MontoTotal = f.Total,
                       TramiteId = f.TramiteId,
                       EstadoId = f.EstadoId ?? 0
                   }).ToListAsync();

            return dto;
        }

        public async Task<Factura> ConsultarFacturaPorIdd(int facturaId)
        {
            return await _context.Facturas.Include(f => f.Contribuyente)
                .FirstAsync(f => f.Id == facturaId);
        }

        public async Task<DTO_Factura> ConsultarFacturaPorId(int facturaId)
        {
            DTO_Factura dto = new DTO_Factura();

            dto = await (from f in _context.Facturas.Include(c => c.Contribuyente)
                         where f.Id == facturaId
                         select new DTO_Factura
                         {
                             Id = f.Id,
                             TramiteId = f.TramiteId,
                             FechaCreacion = f.FechaCreacion,
                             Total = f.Total,
                             Visibilidad = f.Visibilidad,
                             TipoTramiteId = f.TipoTramiteId,
                             UsuarioEmiteId = f.UsuarioEmiteId,
                             EstadoId = f.EstadoId,
                             ContribuyenteId = f.ContribuyenteId,
                             MetodoPagoId = f.MetodoPagoId != null ? f.MetodoPagoId : 0,
                             UsuarioCajeroId = f.UsuarioCajeroId,
                             Descripcion = f.Descripcion,
                             NombreContribuyente = f.Contribuyente != null ? $"{f.Contribuyente.Apellido}, {f.Contribuyente.Nombre}" : "",
                             ContribuyenteDNI = f.Contribuyente != null ? f.Contribuyente.Dni : "",
                             Vuelto = f.Vuelto != null ? f.Vuelto : 0,
                             Vencimiento = f.FechaVencimiento != null ? f.FechaVencimiento : null,
                             Interes = f.InteresAplicado != null ? f.InteresAplicado : 0,
                         }).FirstAsync();
            return dto;
        }

        public async Task<List<DTO_Factura>> ListaFacturasPorTramiteId(int tramiteId)
        {
            List<DTO_Factura> dto = new List<DTO_Factura>();

             dto = await (from f in _context.Facturas.Include(c => c.Contribuyente)
                   where f.TramiteId == tramiteId
                   select new DTO_Factura
                   {
                       Id = f.Id,
                       TramiteId = f.TramiteId,
                       FechaCreacion = f.FechaCreacion,
                       Total = f.Total,
                       Visibilidad = f.Visibilidad,
                       TipoTramiteId = f.TipoTramiteId,
                       UsuarioEmiteId = f.UsuarioEmiteId,
                       EstadoId = f.EstadoId,
                       ContribuyenteId = f.ContribuyenteId,
                       MetodoPagoId = f.MetodoPagoId,
                       UsuarioCajeroId = f.UsuarioCajeroId,
                       Descripcion = f.Descripcion,
                       NombreContribuyente = f.Contribuyente != null ? $"{f.Contribuyente.Apellido}, {f.Contribuyente.Nombre}" : "",
                       Vencimiento = f.FechaVencimiento != null ? f.FechaVencimiento : null
                   }).ToListAsync();
            return dto;
        }

        public async Task<List<DTO_Factura>> ListaTotalFacturasEmitidasYPendientes()
        {
            List<DTO_Factura> dto = new List<DTO_Factura>();
            dto = await (from f in _context.Facturas.Include(F => F.UsuarioEmite).Where(f => f.EstadoId == (int)EstadosFactura.Emitido || f.EstadoId == (int)EstadosFactura.PendienteDeCobro)
                         select new DTO_Factura
                         {
                             Id = f.Id,
                             TramiteId = f.TramiteId,
                             FechaCreacion = f.FechaCreacion,
                             Total = f.Total,
                             Visibilidad = f.Visibilidad,
                             TipoTramiteId = f.TipoTramiteId,
                             UsuarioEmiteId = f.UsuarioEmiteId,
                             EstadoId = f.EstadoId,
                             ContribuyenteId = f.ContribuyenteId,
                             MetodoPagoId = f.MetodoPagoId,
                             UsuarioCajeroId = f.UsuarioCajeroId,
                             Descripcion = f.Descripcion,
                             NombreUsuarioEmite = f.UsuarioEmite != null ? f.UsuarioEmite.Usuario1 : "",
                             Vencimiento = f.FechaVencimiento != null ? f.FechaVencimiento : null
                         }).OrderByDescending(f=>f.FechaCreacion).ToListAsync();
           
            return dto;
        }

        public async Task<List<DTO_Factura>> ListaFacturasPorPersonaId(int personaId)
        {
            List<DTO_Factura> dto = new List<DTO_Factura>();

            dto = await(from f in _context.Facturas.Include(c => c.Contribuyente)
                        where f.ContribuyenteId == personaId
                        select new DTO_Factura
                        {
                            Id = f.Id,
                            TramiteId = f.TramiteId,
                            FechaCreacion = f.FechaCreacion,
                            Total = f.Total,
                            Visibilidad = f.Visibilidad,
                            TipoTramiteId = f.TipoTramiteId,
                            UsuarioEmiteId = f.UsuarioEmiteId,
                            EstadoId = f.EstadoId,
                            ContribuyenteId = f.ContribuyenteId,
                            MetodoPagoId = f.MetodoPagoId,
                            UsuarioCajeroId = f.UsuarioCajeroId,
                            Descripcion = f.Descripcion,
                            NombreContribuyente = f.Contribuyente != null ? $"{f.Contribuyente.Apellido}, {f.Contribuyente.Nombre}" : ""
                        }).ToListAsync();
            return dto;
        }

        public async Task<List<MetodoPago>> ListaMetodoPago()
        {
            return await _context.MetodoPagos.ToListAsync();
        }

        public async Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasCobradas(
             int paginaActual,
             int registrosPorPagina,
             DateTime? fechaDesde = null,
             DateTime? fechaHasta = null)
        {
            var query = _context.Facturas
                .Include(f => f.UsuarioEmite)
                .Where(f => f.EstadoId == (int)EstadosFactura.Cobrado);

            // Aplicar filtros solo si se pasan las fechas
            if (fechaDesde.HasValue)
                query = query.Where(f => f.FechaCreacion >= fechaDesde.Value);

            if (fechaHasta.HasValue)
            {
                // Sumamos un día completo a la fechaHasta para incluir todas las facturas de ese día
                DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(f => f.FechaCreacion <= hasta);
            }

            int totalRegistros = await query.CountAsync();

            var dto = await query
                .OrderByDescending(f => f.FechaCreacion)
                .Skip((paginaActual - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(f => new DTO_Factura
                {
                    Id = f.Id,
                    TramiteId = f.TramiteId,
                    FechaCreacion = f.FechaCreacion,
                    Total = f.Total,
                    Visibilidad = f.Visibilidad,
                    TipoTramiteId = f.TipoTramiteId,
                    UsuarioEmiteId = f.UsuarioEmiteId,
                    EstadoId = f.EstadoId,
                    ContribuyenteId = f.ContribuyenteId,
                    MetodoPagoId = f.MetodoPagoId,
                    UsuarioCajeroId = f.UsuarioCajeroId,
                    Descripcion = f.Descripcion,
                    NombreUsuarioEmite = f.UsuarioEmite != null ? f.UsuarioEmite.Usuario1 : "",
                    Vencimiento = f.FechaVencimiento != null ? f.FechaVencimiento : null
                })
                .ToListAsync();

            return (dto, totalRegistros);
        }

        public async Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasAnuladas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var query = _context.Facturas
                .Include(f => f.UsuarioEmite)
                .Where(f => f.EstadoId == (int)EstadosFactura.Anulado);

            // Aplicar filtros solo si se pasan las fechas
            if (fechaDesde.HasValue)
                query = query.Where(f => f.FechaCreacion >= fechaDesde.Value);

            if (fechaHasta.HasValue)
            {
                // Sumamos un día completo a la fechaHasta para incluir todas las facturas de ese día
                DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(f => f.FechaCreacion <= hasta);
            }

            int totalRegistros = await query.CountAsync();

            var dto = await query
                .OrderByDescending(f => f.FechaCreacion)
                .Skip((paginaActual - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(f => new DTO_Factura
                {
                    Id = f.Id,
                    TramiteId = f.TramiteId,
                    FechaCreacion = f.FechaCreacion,
                    Total = f.Total,
                    Visibilidad = f.Visibilidad,
                    TipoTramiteId = f.TipoTramiteId,
                    UsuarioEmiteId = f.UsuarioEmiteId,
                    EstadoId = f.EstadoId,
                    ContribuyenteId = f.ContribuyenteId,
                    MetodoPagoId = f.MetodoPagoId,
                    UsuarioCajeroId = f.UsuarioCajeroId,
                    Descripcion = f.Descripcion,
                    NombreUsuarioEmite = f.UsuarioEmite != null ? f.UsuarioEmite.Usuario1 : "",
                    Vencimiento = f.FechaVencimiento != null ? f.FechaVencimiento : null
                })
                .ToListAsync();

            return (dto, totalRegistros);
        }

        public async Task<List<DTO_HistorialEstadoFactura>> HistorialEstadoFacturaPorFacturaId(int facturaId)
        {
            List<DTO_HistorialEstadoFactura> dto = new List<DTO_HistorialEstadoFactura>();

            dto = await (from f in _context.HistorialEstadosFacturas
                         where f.FacturaId == facturaId
                         select new DTO_HistorialEstadoFactura{
                            Id = f.Id,
                            FacturaId = f.FacturaId,
                            EstadoId= f.EstadoId,
                            FechaCambio = f.FechaCambio,
                        }).OrderBy(f=>f.FechaCambio).ToListAsync();

            return dto;
        }

        public async Task<List<DTO_FacturasReporte>> ListaFacturasReportes(DateTime fechaDesde, DateTime fechaHasta)
        {
            List<DTO_FacturasReporte> dto = new List<DTO_FacturasReporte>();
            dto = await (from f in _context.Facturas
                         where f.FechaCreacion >= fechaDesde && f.FechaCreacion <= fechaHasta
                         select new DTO_FacturasReporte
                         {
                             Id = f.Id,
                             FechaGeneracion = f.FechaCreacion,
                             Monto = f.Total
                         }).ToListAsync();

            return dto;
        }
    }
}
