using CemSys2.Interface;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Data
{
    public class TarifariaBD : ITarifariaBD
    {
        private readonly IRepositoryDB<Tarifaria> _tarifariaRepository;
        private readonly AppDbContext _context;

        public TarifariaBD(IRepositoryDB<Tarifaria> tarifariaRepository, AppDbContext context)
        {
            _tarifariaRepository = tarifariaRepository;
            _context = context;
        }


        //tarifaria------------------------
        public async Task RegistrarTarifaria(Tarifaria modelo)
        {
            try
            {
                var nombreParam = new SqlParameter("@NombreTarifaria", modelo.Nombre);

                await _context.Database.ExecuteSqlRawAsync("EXEC CrearTarifariaCompleta @NombreTarifaria", nombreParam);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la tarifaria", ex);
            }
        }

        public async Task<int> ModificarTarifaria(Tarifaria modelo)
        {
            try
            {
                return await _tarifariaRepository.Modificar(modelo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la tarifaria", ex);
            }
        }

        public async Task<List<Tarifaria>> EmitirListadoTarifaria()
        {
            try
            {
                return await _context.Tarifarias.FromSqlRaw("EXEC sp_EmitirListadoTarifaria").ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al emitir listado de las tarifarias", ex);
            }
        }

        public async Task<Tarifaria> ConsultarTarifaria(int id)
        {
            try
            {
                return await _tarifariaRepository.Consultar(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la tarifaria", ex);
            }
        }
        //tarifaria------------------------





        //precios tarifaria------------------------
        public async Task<List<PreciosTarifaria>> ConsultarPrecioTarifaria(int id)
        {
            try
            {
                return await _context.PreciosTarifarias
                        .Where(p => p.TarifarioId == id)
                        .Include(p => p.AniosConcesionNavigation)
                        .Include(p => p.ConceptoTarifaria)
                        .Include(p => p.Seccion)
                        .Include(p => p.Tarifario)
                        .OrderBy(p => p.ConceptoTarifaria.Nombre)
                        .ThenBy(p => p.Seccion.Nombre)
                        .ThenBy(p => p.NroFila)
                        .ThenByDescending(p => p.AniosConcesionNavigation.Anios)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar precios tarifaria", ex);
            }
        }
        //precios tarifaria------------------------


        public Task<ConceptosTarifaria> ConsultarConceptoTarifaria(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AniosConcesion>> EmitirListadoAniosConcesion()
        {
            throw new NotImplementedException();
        }

        public Task<List<ConceptosTarifaria>> EmitirListadoConceptoTarifaria()
        {
            throw new NotImplementedException();
        }

        public Task<List<PreciosTarifaria>> EmitirListadoPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

        public Task<int> ModificarConceptoTarifaria(ConceptosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

        public Task<int> ModificarPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

      

        public Task<int> RegistrarConceptoTarifaria(ConceptosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

      

        public Task<int> RegistrarTipoConceptoTarifaria(TiposConceptoTarifarium modelo)
        {
            throw new NotImplementedException();
        }
    }
}
