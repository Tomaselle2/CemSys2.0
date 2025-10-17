using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Interface.Archivos;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Data
{
    public class ArchivoBD : IArchivoBD
    {
        readonly AppDbContext _context;

        public ArchivoBD(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo, string descripcion)
        {
                byte[] contenido;
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }

                var archivoo = new ArchivosDocumentacion
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
                _context.ArchivosDocumentacions.Add(archivoo);
           
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
            archivo.CategoriaArchivo = categoriaArchivo;

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

                using (var ms = new MemoryStream())
                {
                    await nuevoArchivo.CopyToAsync(ms);
                    archivo.Contenido = ms.ToArray();
                }

                archivo.NombreArchivo = Path.GetFileName(nuevoArchivo.FileName);
                archivo.TipoArchivo = mimeType;
                archivo.TamanoBytes = nuevoArchivo.Length;
            }

            _context.ArchivosDocumentacions.Update(archivo);
            await _context.SaveChangesAsync();
        }

        public async Task<ArchivosDocumentacion> ObtenerArchivo(Guid archivoGuid)
        {
            return await _context.ArchivosDocumentacions.FirstAsync(a => a.ArchivoId == archivoGuid);
        }
    }
}
