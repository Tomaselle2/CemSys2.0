using CemSys2.Interface;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Personas;
namespace CemSys2.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        Task ExecuteInTransactionAsync(Func<Task> action);

        //public FacturaRepository Facturas { get; }
        //public PagoRepository Pagos { get; }
        //public HistorialRepository Historiales { get; }
        public IConcesionesDB _concesionesBD { get; }
        public IFacturasBD _facturasBD { get; }
        public IPersonasBD _personasBD { get; }
        Task<int> SaveChangesAsync();

    }
}
