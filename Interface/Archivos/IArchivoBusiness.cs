using CemSys2.DTO.Concesiones;
using CemSys2.Models;

namespace CemSys2.Interface.Archivos
{
    public interface IArchivoBusiness
    {
        Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId); //trae todos los archivos menos recibos
        Task<ArchivosDocumentacion> ObtenerArchivo(Guid archivoGuid);
    }
}
