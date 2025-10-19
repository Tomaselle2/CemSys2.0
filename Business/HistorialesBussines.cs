using CemSys2.Interface.Historiales;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class HistorialesBussines : IHistorialesBusiness
    {
        private readonly IHistorialesBD _historialesBD;

        public HistorialesBussines(IHistorialesBD historialesBD)
        {
            _historialesBD = historialesBD;
        }
        public async Task<List<HistorialEstadoTramite>> HistorialEstadoTramites(int tramiteId)
        {
            return await _historialesBD.HistorialEstadoTramites(tramiteId);
        }

        public async Task RegistrarHistorialFactura(HistorialEstadosFactura historial)
        {
            await _historialesBD.RegistrarHistorialFactura(historial);
        }

        public async Task RegistrarHistorialTramite(HistorialEstadoTramite historial)
        {
            await _historialesBD.RegistrarHistorialTramite(historial);
        }
    }
}
