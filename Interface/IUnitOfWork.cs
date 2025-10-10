using CemSys2.Interface;
using CemSys2.Interface.Archivos;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Historiales;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Personas;
using CemSys2.Interface.Tramite;
namespace CemSys2.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        Task ExecuteInTransactionAsync(Func<Task> action);

        public IConcesionesDB _concesionesBD { get; }
        public IFacturasBD _facturasBD { get; }
        public IPersonasBD _personasBD { get; }
        public ITramiteBD _tramiteBD { get; }
        public IArchivoBD _archivoBD { get; }
        public IHistorialesBD _historialesBD { get; }
        public IIntroduccionBD _introduccionBD { get; }
        Task<int> SaveChangesAsync();

    }
}
