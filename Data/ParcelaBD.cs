using CemSys2.DTO.Parcelas;
using CemSys2.Interface.Parcelas;
using CemSys2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CemSys2.Data
{
    public class ParcelaBD : IParcelaBD
    {
        private readonly AppDbContext _context;

        public ParcelaBD(AppDbContext context)
        {
            _context = context;
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
                                TipoParcela = reader.GetInt32(reader.GetOrdinal("TipoParcela"))
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
                                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? null : reader.GetString(reader.GetOrdinal("Dni"))
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
                                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? null : reader.GetString(reader.GetOrdinal("Dni"))
                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }

            return resultado;
        }
    }
}
