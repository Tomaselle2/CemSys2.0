using CemSys2.Interface;
using System.Linq.Expressions;

namespace CemSys2.Business
{
    public class ServiceGenericBusiness<T> : IRepositoryBusiness<T> where T : class
    {
        private readonly IRepositoryDB<T> _contextDB;
        public ServiceGenericBusiness(IRepositoryDB<T> context)
        {
            _contextDB = context;
        }

        public async Task<T> Consultar(int id)
        {
            try
            {
                return await _contextDB.Consultar(id);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> ContarTotalAsync(Expression<Func<T, bool>> filtro)
        {
            try
            {
                return await _contextDB.ContarTotalAsync(filtro);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                await _contextDB.Eliminar(id);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<T>> EmitirListado()
        {
            try
            {
                return await _contextDB.EmitirListado();
            }
            catch (Exception) { throw; }
        }

        public async Task<int> Modificar(T modelo)
        {
            try
            {
                return await _contextDB.Modificar(modelo);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<T>> ObtenerPaginadoAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filtro, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            try
            {
                return await _contextDB.ObtenerPaginadoAsync(pageNumber, pageSize, filtro);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> Registrar(T modelo)
        {
            try
            {
                return await _contextDB.Registrar(modelo);
            }
            catch (Exception) { throw; }
        }
    }
}
