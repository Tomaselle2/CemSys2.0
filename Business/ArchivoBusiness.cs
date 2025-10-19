using CemSys2.DTO.Concesiones;
using CemSys2.Interface.Archivos;
using CemSys2.Models;
using DocumentFormat.OpenXml.Presentation;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.Business
{
    public class ArchivoBusiness : IArchivoBusiness
    {
        private readonly IArchivoBD _archivoBD;
        public ArchivoBusiness(IArchivoBD archivoBD)
        {
            _archivoBD = archivoBD;
        }

        public async Task EditarArchivo(Guid archivoId, string descripcion, string categoriaArchivo, IFormFile? nuevoArchivo)
        {
            if (string.IsNullOrEmpty(descripcion))
                throw new ValidationException("El concepto es obligatorio.");

            if (nuevoArchivo != null && nuevoArchivo.Length > 0)
            {
                var extension = Path.GetExtension(nuevoArchivo.FileName).ToLower();
                var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };

                if (!permitidas.Contains(extension))
                    throw new ValidationException("Solo se permiten archivos PNG, JPG o PDF.");
            }

            await _archivoBD.EditarArchivo(archivoId, descripcion, categoriaArchivo, nuevoArchivo);
        }

        public async Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId)
        {
            return await _archivoBD.ListaArchivosTramiteId(tramiteId);
        }

        public async Task<ArchivosDocumentacion> ObtenerArchivo(Guid archivoGuid)
        {
            return await _archivoBD.ObtenerArchivo(archivoGuid);
        }

        public async Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo, string descripcion)
        {
            await _archivoBD.RegistrarArchivo(archivo, mimeType, tramiteId, categoriaArchivo, descripcion);
        }
    }
}
