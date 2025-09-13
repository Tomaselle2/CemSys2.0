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
            int id = 0;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    tramite.Id = await ObtenerProximoIdTramite();
                    _context.Tramites.Add(tramite);
                    await _context.SaveChangesAsync();
                    id = tramite.Id;
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Re-throw the exception after rolling back
                }
            }

            return id;
        }

        private async Task<int> ObtenerProximoIdTramite()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }
    }
}
