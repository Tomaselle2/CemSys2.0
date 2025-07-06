using CemSys2.DTO;
using CemSys2.Models;
namespace CemSys2.Interface
{
    public interface ISeccionesBusiness
    {
        public Task<List<DTO_TipoNumeracionParcela>> ListaNumeracionParcelas();
        public Task<List<DTO_TipoNichos>> ListaTipoNicho();
        public Task<List<DTO_secciones>> ListaSecciones();

        public Task<int> RegistrarSeccion(Seccione seccionesViewModel);
        public Task Eliminar(int id);

    }
}
