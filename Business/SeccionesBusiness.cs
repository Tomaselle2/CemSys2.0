using CemSys2.DTO;
using CemSys2.Interface;
using CemSys2.Models;
using System.Linq.Expressions;

namespace CemSys2.Business
{
    public class SeccionesBusiness : ISeccionesBusiness
    {
        private readonly IRepositoryBusiness<Seccione> _repositorySeccionesBusiness;
        private readonly IRepositoryBusiness<TipoNumeracionParcela> _repositoryTipoNumeracionParcela;
        private readonly IRepositoryBusiness<TipoNicho> _repositoryTipoNicho;

        public SeccionesBusiness(IRepositoryBusiness<Seccione> repositorySeccionesBusiness,
                                 IRepositoryBusiness<TipoNumeracionParcela> repositoryTipoNumeracionParcela,
                                 IRepositoryBusiness<TipoNicho> repositoryTipoNicho)
        {
            _repositorySeccionesBusiness = repositorySeccionesBusiness;
            _repositoryTipoNumeracionParcela = repositoryTipoNumeracionParcela;
            _repositoryTipoNicho = repositoryTipoNicho;
        }

        public async Task<int> ContarTotalAsync(Expression<Func<Seccione, bool>> filtro)
        {
            try
            {
                return await _repositorySeccionesBusiness.ContarTotalAsync(filtro);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar las secciones: {ex.Message}", ex);
            }
        }

        public Task Eliminar(int id)
        {
            try
            {
                return _repositorySeccionesBusiness.Eliminar(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la sección nicho: {ex.Message}", ex);
            }
        }
        public async Task<List<DTO_TipoNumeracionParcela>> ListaNumeracionParcelas()
        {
            try
            {
                List<TipoNumeracionParcela> listaNumeracionParcelas = await _repositoryTipoNumeracionParcela.EmitirListado();
                return listaNumeracionParcelas.Select(t => new DTO_TipoNumeracionParcela
                {
                    Id = t.Id,
                    Tipo = t.TipoNumeracion.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de numeración de parcelas: {ex.Message}", ex);
            }
        }

        public async Task<List<DTO_secciones>> ListaSeccionesPaginado(int pagina, int cantidadPorPagina, Expression<Func<Seccione, bool>> filtro = null)
        {
            try
            {
                // Si no se proporciona filtro, usar el filtro por defecto
                if (filtro == null)
                    filtro = s => s.Visibilidad == true;

                var secciones = await _repositorySeccionesBusiness.ObtenerPaginadoAsync(pagina, cantidadPorPagina, filtro);
                return secciones.OrderByDescending(t => t.Id).Select(t => new DTO_secciones
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Visibilidad = t.Visibilidad,
                    Filas = t.Filas,
                    NroParcelas = t.NroParcelas,
                    TipoNumeracion = t.TipoNumeracionParcelaNavigation.TipoNumeracion.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista paginada de secciones: {ex.Message}", ex);
            }
        }


        public async Task<List<DTO_TipoNichos>> ListaTipoNicho()
        {
            try
            {
                List<TipoNicho> listaTipoNicho = await _repositoryTipoNicho.EmitirListado();
                return listaTipoNicho.Select(t => new DTO_TipoNichos
                {
                    Id = t.Id,
                    Tipo = t.Tipo.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de tipos de nichos: {ex.Message}", ex);
            }
        }

        public async Task<int> RegistrarSeccion(Seccione seccionesViewModel)
        {
            try
            {
                return await _repositorySeccionesBusiness.Registrar(seccionesViewModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar la sección nicho: {ex.Message}", ex);
            }
        }

    }
}
