using CemSys2.DTO.Concesiones;

namespace CemSys2.Interface.Archivos
{
    public interface IArchivoBD
    {
        Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo, string descripcion);
        Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId); //trae todos los archivos menos recibos
        Task EditarArchivo(Guid archivoId, string descripcion, string categoriaArchivo, IFormFile? nuevoArchivo);
    }
}
