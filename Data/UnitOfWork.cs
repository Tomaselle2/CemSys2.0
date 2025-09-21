using CemSys2.Interface;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Personas;
using CemSys2.Models;
using iText.Pdfua.Checkers.Utils.Ua1;

namespace CemSys2.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IConcesionesDB _concesionesBD {  get; }
        public IFacturasBD _facturasBD { get; }
        public IPersonasBD _personasBD { get; }

        public UnitOfWork(AppDbContext context, IConcesionesDB concesionesDB, IFacturasBD facturasBD, IPersonasBD personasBD)
        {
            _context = context;
            _concesionesBD = concesionesDB;
            _facturasBD = facturasBD;
            _personasBD = personasBD;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public void Dispose() => _context.Dispose();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
