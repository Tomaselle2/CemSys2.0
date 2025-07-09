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
        private readonly IRepositoryBusiness<Parcela> _repositoryParcelasBusiness;
        private readonly IRepositoryBusiness<TipoPanteon> _repositoryTipoPanteon;

        public SeccionesBusiness(IRepositoryBusiness<Seccione> repositorySeccionesBusiness,
                                 IRepositoryBusiness<TipoNumeracionParcela> repositoryTipoNumeracionParcela,
                                 IRepositoryBusiness<TipoNicho> repositoryTipoNicho,
                                 IRepositoryBusiness<Parcela> repositoryParcelasBusiness,
                                 IRepositoryBusiness<TipoPanteon> repositoryTipoPanteon)
        {
            _repositorySeccionesBusiness = repositorySeccionesBusiness;
            _repositoryTipoNumeracionParcela = repositoryTipoNumeracionParcela;
            _repositoryTipoNicho = repositoryTipoNicho;
            _repositoryParcelasBusiness = repositoryParcelasBusiness;
            _repositoryTipoPanteon = repositoryTipoPanteon;
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

        public async Task<int> ContarTotalparcelasAsync(Expression<Func<Parcela, bool>> filtro)
        {
            try
            {
                return await _repositoryParcelasBusiness.ContarTotalAsync(filtro);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar las parcelas: {ex.Message}", ex);
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                Seccione seccion = await _repositorySeccionesBusiness.Consultar(id);
                seccion.Visibilidad = false;
                await _repositorySeccionesBusiness.Modificar(seccion);
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

        public async Task<List<DTO_Parcelas>> ListaParcelasPaginada(int pagina, int cantidadPorPagina, Expression<Func<Parcela, bool>> filtro = null, Func<IQueryable<Parcela>, IOrderedQueryable<Parcela>> orderBy = null)
        {
            try
            {
                // Si no se proporciona filtro, usar el filtro por defecto
                if (filtro == null)
                    filtro = s => s.Visibilidad == true;

                // Si no se proporciona orden, usar Id descendente por defecto
                if (orderBy == null)
                    orderBy = q => q.OrderByDescending(s => s.Id);

                // Llamada al repositorio con orden incluido
                var parcelas = await _repositoryParcelasBusiness.ObtenerPaginadoAsync(
                    pagina,
                    cantidadPorPagina,
                    filtro,
                    orderBy
                );

                return parcelas.Select(t => new DTO_Parcelas
                {
                    Id = t.Id,
                    Visibilidad = t.Visibilidad,
                    NroFila = t.NroFila,
                    NroParcela = t.NroParcela,
                    CantidadDifuntos = t.CantidadDifuntos,
                    Seccion = t.Seccion,
                    IdTipoNicho = t.TipoNicho
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista paginada de parcelas: {ex.Message}", ex);
            }
        }

        public async Task<List<DTO_secciones>> ListaSeccionesPaginado(
            int pagina,
            int cantidadPorPagina,
            Expression<Func<Seccione, bool>> filtro = null,
            Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = null)
        {
            try
            {
                // Si no se proporciona filtro, usar el filtro por defecto
                if (filtro == null)
                    filtro = s => s.Visibilidad == true;

                // Si no se proporciona orden, usar Id descendente por defecto
                if (orderBy == null)
                    orderBy = q => q.OrderByDescending(s => s.Id);

                // Llamada al repositorio con orden incluido
                var secciones = await _repositorySeccionesBusiness.ObtenerPaginadoAsync(
                    pagina,
                    cantidadPorPagina,
                    filtro,
                    orderBy
                );

                return secciones.Select(t => new DTO_secciones
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

        public async Task<List<DTO_secciones>> ListaSeccionesPaginado2(
            int pagina,
            int cantidadPorPagina,
            Expression<Func<Seccione, bool>> filtro = null,
            Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = null)
        {
            try
            {
                // Si no se proporciona filtro, usar el filtro por defecto
                if (filtro == null)
                    filtro = s => s.Visibilidad == true;

                // Si no se proporciona orden, usar Id descendente por defecto
                if (orderBy == null)
                    orderBy = q => q.OrderByDescending(s => s.Id);

                // Llamada al repositorio con orden incluido
                var secciones = await _repositorySeccionesBusiness.ObtenerPaginadoAsync(
                    pagina,
                    cantidadPorPagina,
                    filtro,
                    orderBy
                );

                return secciones.Select(t => new DTO_secciones
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Visibilidad = t.Visibilidad,
                    Filas = t.Filas,
                    NroParcelas = t.NroParcelas
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

        public async Task<List<DTO_TipoPanteon>> ListaTipoPanteon()
        {
            List<TipoPanteon> listaTipoPanteon = await _repositoryTipoPanteon.EmitirListado();
            return listaTipoPanteon.Select(t => new DTO_TipoPanteon
            {
                Id = t.Id,
                Tipo = t.Tipo.ToString()
            }).ToList();
        }

        public async Task<int> RegistrarSeccion(DTO_secciones seccionesViewModel)
        {
            Seccione seccion = new Seccione();
            try
            {
                seccion = new Seccione
                {
                    Nombre = seccionesViewModel.Nombre,
                    Visibilidad = true,
                    Filas = seccionesViewModel.Filas,
                    NroParcelas = seccionesViewModel.NroParcelas,
                    TipoNumeracionParcela = seccionesViewModel.IdTipoNumeracionParcela,
                    TipoParcela = seccionesViewModel.IdTipoParcela
                };
                
                return await _repositorySeccionesBusiness.Registrar(seccion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar la sección nicho: {ex.Message}", ex);
            }
        }

    }
}
