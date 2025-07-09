using System.Linq.Expressions;

namespace CemSys2.Interface
{
    public interface IRepositoryDB<T> where T : class
    {
        Task<int> Registrar(T modelo);
        Task<List<T>> EmitirListado();
        Task<T> Consultar(int id);
        Task<int> Modificar(T modelo);
        Task Eliminar(int id);

        Task<List<T>> ObtenerPaginadoAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<int> ContarTotalAsync(Expression<Func<T, bool>> filtro = null);
    }
}
