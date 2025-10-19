using CemSys2.Models;

namespace CemSys2.Interface.Historiales
{
    public interface IHistorialesBusiness
    {
        Task<List<HistorialEstadoTramite>> HistorialEstadoTramites(int tramiteId);
    }
}
