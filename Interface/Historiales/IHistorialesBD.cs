using CemSys2.Models;

namespace CemSys2.Interface.Historiales
{
    public interface IHistorialesBD
    {
        Task RegistrarHistorialTramite(HistorialEstadoTramite historial);
        Task RegistrarHistorialFactura(HistorialEstadosFactura historial);
        Task<List<HistorialEstadoTramite>> HistorialEstadoTramites(int tramiteId);
    }
}
