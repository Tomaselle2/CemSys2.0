using CemSys2.DTO;
using CemSys2.Models;
using System.Linq.Expressions;
namespace CemSys2.Interface
{
    public interface ISeccionesBusiness
    {
        public Task<List<DTO_TipoNumeracionParcela>> ListaNumeracionParcelas();
        public Task<List<DTO_TipoNichos>> ListaTipoNicho();
        //public Task<List<DTO_secciones>> ListaSecciones();

        public Task<int> RegistrarSeccion(DTO_secciones seccionesViewModel);
        public Task Eliminar(int id);

        public Task<List<DTO_secciones>> ListaSeccionesPaginado(int pagina, int cantidadPorPagina, Expression<Func<Seccione, bool>> filtro = null, Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = null);
        public Task<int> ContarTotalAsync(Expression<Func<Seccione, bool>> filtro);


    }
}
