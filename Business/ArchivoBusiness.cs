using CemSys2.DTO.Concesiones;
using CemSys2.Interface.Archivos;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class ArchivoBusiness : IArchivoBusiness
    {
        private readonly IArchivoBD _archivoBD;
        public ArchivoBusiness(IArchivoBD archivoBD)
        {
            _archivoBD = archivoBD;
        }

        public async Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId)
        {
            return await _archivoBD.ListaArchivosTramiteId(tramiteId);
        }

        public async Task<ArchivosDocumentacion> ObtenerArchivo(Guid archivoGuid)
        {
            return await _archivoBD.ObtenerArchivo(archivoGuid);
        }

    }
}
