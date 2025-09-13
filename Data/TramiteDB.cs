using CemSys2.Interface.Tramite;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Data
{
    public class TramiteDB : ITramiteBD
    {
        private readonly AppDbContext _context;

        public TramiteDB(AppDbContext context)
        {
            _context = context;
        }
        public Task<Tramite> ConsultarTramite(int idTramite)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RegistrarTramite(Tramite tramite) 
        {
            tramite.Id = await ObtenerProximoIdTramite();
            _context.Tramites.Add(tramite);
            await _context.SaveChangesAsync();
            return tramite.Id;
        }

        private async Task<int> ObtenerProximoIdTramite()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }
    }
}
