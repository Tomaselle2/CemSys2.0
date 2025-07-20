using CemSys2.DTO.Personas;
using CemSys2.Interface.Personas;
using CemSys2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CemSys2.Data
{
    public class PersonasBD : IPersonasBD
    {
        private readonly AppDbContext _context;

        public PersonasBD(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaPersona>> ListaCategoriaPersonas()
        {
            return await _context.CategoriaPersonas.ToListAsync();
        }

        public async Task<(List<DTO_Difunto_Persona_Index> personas, int totalRegistros)> ListaPersonasIndex(
        string? dni = null,
        string? nombre = null,
        string? apellido = null,
        int? categoriaId = null,
        int registrosPorPagina = 10,
        int pagina = 1)
        {
            // Consulta base
            var query = from p in _context.Personas
                        join cp in _context.CategoriaPersonas on p.CategoriaPersona equals cp.Id
                        select new DTO_Difunto_Persona_Index
                        {
                            IdPersona = p.IdPersona,
                            Nombre = p.Nombre,
                            Apellido = p.Apellido,
                            Dni = p.Dni,
                            Sexo = p.Sexo,
                            CategoriaPersona = p.CategoriaPersona
                        };

            // Aplicar filtros
            if (!string.IsNullOrEmpty(dni))
            {
                query = query.Where(p => p.Dni.Contains(dni));
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.Nombre.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(apellido))
            {
                query = query.Where(p => p.Apellido.Contains(apellido));
            }

            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaPersona == categoriaId.Value);
            }

            // Obtener el total de registros antes de paginar
            var totalRegistros = await query.CountAsync();

            // Aplicar paginación
            var personas = await query
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            return (personas, totalRegistros);
        }
    }
}
