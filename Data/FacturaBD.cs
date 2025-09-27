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

        public async Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo, string descripcion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2️ Insertar archivo en ArchivosDocumentacion (FILESTREAM)
                byte[] contenido;
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }

                var archivoRecibo = new ArchivosDocumentacion
                {
                    CategoriaArchivo = categoriaArchivo,
                    TramiteId = tramiteId,
                    NombreArchivo = Path.GetFileName(archivo.FileName),
                    TipoArchivo = mimeType,
                    TamanoBytes = archivo.Length,
                    Contenido = contenido,
                    Descripcion = descripcion,
                    FechaCreacion = DateTime.Now,
                    Visibilidad = true,
                };
                _context.ArchivosDocumentacions.Add(archivoRecibo);
                await _context.SaveChangesAsync();


                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //me devuelve todos los archivos menos los recibos
        public async Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId)
        {
            return await _context.ArchivosDocumentacions
                    .Where(ar => ar.TramiteId == tramiteId && ar.CategoriaArchivo != CategoriaArchivosEnum.Recibo.ToString())
                    .Select(ar => new DTO_Archivos_Documentacion
                    {
                        TramiteId = ar.TramiteId.Value,
                        CategoriaArchivo = ar.CategoriaArchivo,
                        NombreArchivo = ar.NombreArchivo,
                        TipoArchivo = ar.TipoArchivo,
                        TamanoBytes = ar.TamanoBytes,
                        Descripcion = ar.Descripcion,
                        FechaCreacion = ar.FechaCreacion,
                        Visibilidad = ar.Visibilidad,
                        ArchivoId = ar.ArchivoId,
                    }).ToListAsync();
        }

        //edita un archivo
        public async Task EditarArchivo(Guid archivoId, string descripcion, string categoriaArchivo, IFormFile? nuevoArchivo)
        {
            var archivo = await _context.ArchivosDocumentacions
                    .FirstAsync(a => a.ArchivoId == archivoId);

            archivo.Descripcion = descripcion;
            _context.ArchivosDocumentacions.Update(archivo);

            if (nuevoArchivo != null && nuevoArchivo.Length > 0)
            {
                var extension = Path.GetExtension(nuevoArchivo.FileName).ToLower();
                string mimeType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".pdf" => "application/pdf",
                    _ => "application/octet-stream"
                };

                byte[] contenido;
                using (var ms = new MemoryStream())
                {
                    await nuevoArchivo.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }

                if (archivo != null)
                {
                    archivo.NombreArchivo = Path.GetFileName(nuevoArchivo.FileName);
                    archivo.TipoArchivo = mimeType;
                    archivo.TamanoBytes = nuevoArchivo.Length;
                    archivo.Contenido = contenido;
                    archivo.CategoriaArchivo = categoriaArchivo;

                    _context.ArchivosDocumentacions.Update(archivo);
                }
            }

            await _context.SaveChangesAsync();
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
    }
}
