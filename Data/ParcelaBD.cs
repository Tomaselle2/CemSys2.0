using CemSys2.DTO.Parcelas;
using CemSys2.Interface;
using CemSys2.Interface.Parcelas;
using CemSys2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;

namespace CemSys2.Data
{
    public class ParcelaBD : IParcelaBD
    {
        private readonly AppDbContext _context;
        private readonly IRepositoryDB<Parcela> _parcelaGeneric;

        public ParcelaBD(AppDbContext context, IRepositoryDB<Parcela> parcelaGeneric)
        {
            _context = context;
            _parcelaGeneric = parcelaGeneric;
        }

        public async Task<Parcela> BuscarParcelaPorId(int parcelaId)
        {
            return await _parcelaGeneric.Consultar(parcelaId);
        }

        public async Task<DTO_Parcelas_Encabezado> EncabezadoParcela(int parcelaId)
        {
            var resultado = new DTO_Parcelas_Encabezado();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerEncabezadoParcela"; // Nombre del SP
                    command.CommandType = CommandType.StoredProcedure;

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@parcelaId";
                    parametro.Value = parcelaId;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) // Usamos if en lugar de while ya que esperamos un solo registro
                        {
                            resultado = new DTO_Parcelas_Encabezado
                            {
                                ParcelaId = reader.GetInt32(reader.GetOrdinal("ParcelaId")),
                                NroParcela = reader.GetInt32(reader.GetOrdinal("NroParcela")),
                                NroFila = reader.GetInt32(reader.GetOrdinal("NroFila")),
                                NombreSeccion = reader.IsDBNull(reader.GetOrdinal("NombreSeccion"))
                                                ? string.Empty
                                                : reader.GetString(reader.GetOrdinal("NombreSeccion")),
                                TipoParcela = reader.GetInt32(reader.GetOrdinal("TipoParcela")),
                                TipoNicho = reader.IsDBNull(reader.GetOrdinal("TipoNicho"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("TipoNicho")),
                                TipoPanteon = reader.IsDBNull(reader.GetOrdinal("TipoPanteonId"))
                                              ? (int?)null
                                              : reader.GetInt32(reader.GetOrdinal("TipoPanteonId")),
                                NombrePanteon = reader.IsDBNull(reader.GetOrdinal("nombrePanteon"))
                                              ? ""
                                              : reader.GetString(reader.GetOrdinal("nombrePanteon"))
                            };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosActuales(int parcelaId)
        {
            var resultado = new List<DTO_Historial_Parcelas>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerDifuntosEnParcela";
                    command.CommandType = CommandType.StoredProcedure;

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@parcelaId";
                    parametro.Value = parcelaId;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new DTO_Historial_Parcelas
                            {
                                DifuntoId = reader.GetInt32(reader.GetOrdinal("DifuntoId")),
                                FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fechaIngreso")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("apellido")) ? null : reader.GetString(reader.GetOrdinal("apellido")),
                                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? null : reader.GetString(reader.GetOrdinal("Dni")),
                                EstadoDifunto = reader.GetInt32(reader.GetOrdinal("estadoDifunto"))

                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosHistoricos(int parcelaId)
        {
            var resultado = new List<DTO_Historial_Parcelas>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerDifuntosHistoricosEnParcela";
                    command.CommandType = CommandType.StoredProcedure;

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@parcelaId";
                    parametro.Value = parcelaId;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new DTO_Historial_Parcelas
                            {
                                DifuntoId = reader.GetInt32(reader.GetOrdinal("DifuntoId")),
                                FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fechaIngreso")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("apellido")) ? null : reader.GetString(reader.GetOrdinal("apellido")),
                                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? null : reader.GetString(reader.GetOrdinal("Dni")),
                                FechaRetiro = reader.IsDBNull(reader.GetOrdinal("fechaRetiro"))
                                              ? (DateTime?)null
                                              : reader.GetDateTime(reader.GetOrdinal("fechaRetiro")),
                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<List<DTO_Parcela_Tramites>> ListaParcelasTramites(int parcelaId)
        {
            var resultado = new List<DTO_Parcela_Tramites>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerTramitesPorParcela";
                    command.CommandType = CommandType.StoredProcedure;

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@parcelaId";
                    parametro.Value = parcelaId;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new DTO_Parcela_Tramites
                            {
                                TramiteId = reader.GetInt32(reader.GetOrdinal("TramiteId")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                TipoTramite = reader.IsDBNull(reader.GetOrdinal("TipoTramite"))
                                            ? string.Empty
                                            : reader.GetString(reader.GetOrdinal("TipoTramite")),
                                ParcelaId = reader.GetInt32(reader.GetOrdinal("ParcelaId"))
                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }

            return resultado;
        }

        public Task<List<TipoPanteon>> ListaTipoPanteon()
        {
            return _context.TipoPanteons.ToListAsync();
        }

        public Task<List<TipoNicho>> ListaTiposNicho()
        {
            return _context.TipoNichos.ToListAsync();
        }

        public async Task<int> ModificarParcela(Parcela parcela)
        {
           return await _parcelaGeneric.Modificar(parcela);
        }


    }
}
