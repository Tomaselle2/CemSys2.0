using CemSys2.DTO;
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

      

        public async Task RegistrarConceptoTarifaria(ConceptosTarifaria nuevoConcepto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Agregar el nuevo concepto
                _context.ConceptosTarifarias.Add(nuevoConcepto);
                await _context.SaveChangesAsync(); // EF carga nuevoConcepto.Id aquí

                // 2. Obtener tarifarias existentes
                var tarifarias = await _context.Tarifarias.ToListAsync();

                // 3. Si existen tarifarias, crear precios por defecto
                if (tarifarias.Any())
                {
                    var preciosPorDefecto = tarifarias.Select(t => new PreciosTarifaria
                    {
                        TarifarioId = t.Id,
                        ConceptoTarifariaId = nuevoConcepto.Id,
                        Precio = 0m // o cualquier valor por defecto
                    }).ToList();

                    _context.PreciosTarifarias.AddRange(preciosPorDefecto);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al crear el concepto tarifario.", ex);
            }
        }

        public Task<int> RegistrarPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

      

        public Task<int> RegistrarTipoConceptoTarifaria(TiposConceptoTarifarium modelo)
        {
            throw new NotImplementedException();
        }





        public async Task ActualizarPreciosTarifaria(List<PrecioActualizarDto> preciosActualizar)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Obtener los IDs de los precios a actualizar
                var idsPrecios = preciosActualizar.Select(p => p.Id).ToList();

                // Verificar que todos los precios existan
                var preciosExistentes = await _context.PreciosTarifarias
                    .Where(p => idsPrecios.Contains(p.Id))
                    .ToListAsync();

                if (preciosExistentes.Count != preciosActualizar.Count)
                {
                    var idsEncontrados = preciosExistentes.Select(p => p.Id).ToList();
                    var idsNoEncontrados = idsPrecios.Except(idsEncontrados).ToList();

                    throw new ArgumentException($"Los siguientes precios no existen: {string.Join(", ", idsNoEncontrados)}");
                }

                // Actualizar cada precio
                foreach (var precioDto in preciosActualizar)
                {
                    var precioExistente = preciosExistentes.First(p => p.Id == precioDto.Id);

                    // Verificar que el ConceptoTarifariaId coincida (seguridad adicional)
                    if (precioExistente.ConceptoTarifariaId != precioDto.ConceptoTarifariaId)
                    {
                        throw new ArgumentException($"El ConceptoTarifariaId no coincide para el precio {precioDto.Id}");
                    }

                    // Actualizar el precio
                    precioExistente.Precio = precioDto.Precio;

                }

                // Guardar todos los cambios
                var filasAfectadas = await _context.SaveChangesAsync();

                if (filasAfectadas == 0)
                {
                    throw new InvalidOperationException("No se pudieron guardar los cambios.");
                }

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al actualizar precios de tarifaria: {ex.Message}", ex);
            }
        }
    }
}
