using CemSys2.Interface;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CemSys2.Data
{
    public class ServiceGenericDB<T> : IRepositoryDB<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public ServiceGenericDB(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> Consultar(int id)

        {
            try
            {
                var modelo = await _dbSet.FindAsync(id);
                return modelo!;

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
                var modelo = await Consultar(id);
                if (modelo != null)
                {
                    _dbSet.Remove(modelo);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<List<T>> EmitirListado()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> Modificar(T modelo)
        {
            int resultado = 0;
            try
            {
                if (modelo != null)
                {
                    _dbSet.Update(modelo);
                    resultado = await _context.SaveChangesAsync();
                }
                return resultado;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> Registrar(T modelo)
        {
            try
            {
                // Agrega la entidad al DbSet
                var entry = await _dbSet.AddAsync(modelo);
                await _context.SaveChangesAsync();

                // Intenta encontrar una propiedad llamada "Id"
                var idProp = modelo!.GetType().GetProperty("Id");

                if (idProp != null)
                {
                    var idValue = idProp.GetValue(modelo);
                    if (idValue != null)
                    {
                        return Convert.ToInt32(idValue);
                    }
                }

                // Alternativamente: buscar clave primaria con EF Core
                var keyProperties = entry.Metadata.FindPrimaryKey()?.Properties;

                if (keyProperties != null && keyProperties.Count > 0)
                {
                    var keyValue = keyProperties
                        .Select(p => entry.Property(p.Name).CurrentValue)
                        .FirstOrDefault();

                    if (keyValue != null)
                        return Convert.ToInt32(keyValue);
                }

                throw new InvalidOperationException("No se pudo obtener el ID del registro insertado.");
            }
            catch (Exception)
            {
                throw; // o podés loguear antes de relanzar
            }
        }

        public async Task<List<T>> ObtenerPaginadoAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filtro,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            try
            {
                IQueryable<T> query = _dbSet.Where(filtro);

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                query = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize);

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<int> ContarTotalAsync(Expression<Func<T, bool>> filtro)
        {
            try
            {
                return await _dbSet.CountAsync(filtro);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
