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

        public async Task<Persona> ConsultarPersona(int idPersona)
        {
            return await _context.Personas
                .Include(p => p.ActaDefuncionNavigation)
                .Include(p => p.EstadoDifuntoNavigation)
                .FirstOrDefaultAsync(p => p.IdPersona == idPersona);
        }

        public async Task<DTO_Persona_Historial> DatosPersonalesPersona(int idPersona)
        {
            return await _context.Personas.Where(p => p.IdPersona == idPersona)
                .Include(p => p.ActaDefuncionNavigation)
                .Include(p => p.EstadoDifuntoNavigation)
                .Select(p => new DTO_Persona_Historial
                {
                    IdPersona = p.IdPersona,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Dni = p.Dni,
                    Visibilidad = p.Visibilidad,
                    FechaNacimiento = p.FechaNacimiento,
                    FechaDefuncion = p.FechaDefuncion,
                    EstadoDifunto = p.EstadoDifuntoNavigation.Estado,
                    ActaDefuncion = p.ActaDefuncionNavigation,
                    InformacionAdicional = p.InformacionAdicional,
                    CategoriaPersona = p.CategoriaPersona,
                    Sexo = p.Sexo,
                    Correo = p.Correo,
                    Celular = p.Celular,
                    Domicilio = p.Domicilio,
                    DomicilioEnTirolesa = p.DomicilioEnTirolesa,
                    FallecioEnTirolesa = p.FallecioEnTirolesa
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<CategoriaPersona>> ListaCategoriaPersonas()
        {
            return await _context.CategoriaPersonas.ToListAsync();
        }

        public async Task<List<DTO_Persona_Historial_Parcelas>> ListaHistorialParcelas(int idPersona)
        {
            var resultado = new List<DTO_Persona_Historial_Parcelas>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PersonasHistorialParcelas";
                    command.CommandType = CommandType.StoredProcedure;

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@idPersona";
                    parametro.Value = idPersona;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new DTO_Persona_Historial_Parcelas
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("id")) ? null : reader.GetInt32(reader.GetOrdinal("id")),
                                FechaIngresoId = reader.IsDBNull(reader.GetOrdinal("fechaIngreso")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fechaIngreso")),
                                FechaRetiroId = reader.IsDBNull(reader.GetOrdinal("fechaRetiro")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fechaRetiro")),
                                NroParcela = reader.IsDBNull(reader.GetOrdinal("NroParcela")) ? null : reader.GetInt32(reader.GetOrdinal("NroParcela")),
                                NroFila = reader.IsDBNull(reader.GetOrdinal("NroFila")) ? null : reader.GetInt32(reader.GetOrdinal("NroFila")),
                                NombreSeccion = reader.IsDBNull(reader.GetOrdinal("Seccion")) ? null : reader.GetString(reader.GetOrdinal("Seccion")),
                                TipoParcela = reader.IsDBNull(reader.GetOrdinal("tipoParcela")) ? null : reader.GetInt32(reader.GetOrdinal("tipoParcela"))
                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<List<DTO_Persona_Historial_Tramites>> ListaHistorialTramites(int idPersona)
        {
            var resultado = new List<DTO_Persona_Historial_Tramites>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PersonasHistorialTramites";
                    command.CommandType = CommandType.StoredProcedure;

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@idPersona";
                    parametro.Value = idPersona;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new DTO_Persona_Historial_Tramites
                            {
                                TramiteId = reader.GetInt32(reader.GetOrdinal("TramiteId")),
                                PersonaId = reader.GetInt32(reader.GetOrdinal("PersonaId")),
                                FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                                TipoTramite = reader.IsDBNull(reader.GetOrdinal("TipoTramite")) ? null : reader.GetString(reader.GetOrdinal("TipoTramite"))
                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }

            return resultado;
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

        public async Task<int> ModificarPersona(Persona model)
        {
            if (model == null) return 0;

            // Verificar si la entidad existe
            var existe = await _context.Personas.AnyAsync(p => p.IdPersona == model.IdPersona);
            if (!existe) return 0;

            _context.Update(model);
            return await _context.SaveChangesAsync();
        }
    }
}
