using CemSys2.Interface.Historiales;
using CemSys2.Models;

namespace CemSys2.Data
{
    public class HistorialesBD : IHistorialesBD
    {
        private readonly AppDbContext _context;

        public HistorialesBD(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarHistorialFactura(HistorialEstadosFactura historial)
        {
           await _context.HistorialEstadosFacturas.AddAsync(historial);
        }

        public async Task RegistrarHistorialTramite(HistorialEstadoTramite historial)
        {
            await _context.HistorialEstadoTramites.AddAsync(historial);
        }
    }
}
