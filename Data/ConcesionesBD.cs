using CemSys2.DTO.Concesiones;
using CemSys2.Interface.Concesiones;
using CemSys2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CemSys2.Data
{
    public class ConcesionesBD : IConcesionesDB
    {
        public readonly AppDbContext _context;

        public ConcesionesBD(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato()
        {
            var resultados = new List<DTO_Parcelas_Sin_Contrato>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ParcelasSinContrato"; // Nombre del SP
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var parcela = new DTO_Parcelas_Sin_Contrato
                            {
                                ParcelaId = reader.GetInt32(reader.GetOrdinal("parcelaId")),
                                TipoParcela = reader.GetInt32(reader.GetOrdinal("tipoParcela")),
                                NombreSeccion = reader.IsDBNull(reader.GetOrdinal("NombreSeccion"))
                                                ? string.Empty
                                                : reader.GetString(reader.GetOrdinal("NombreSeccion")),
                                NroParcela = reader.GetInt32(reader.GetOrdinal("NroParcela")),
                                NroFila = reader.GetInt32(reader.GetOrdinal("NroFila")),
                                Difuntos = reader.IsDBNull(reader.GetOrdinal("Difuntos"))
                                           ? string.Empty
                                           : reader.GetString(reader.GetOrdinal("Difuntos"))
                            };

                            resultados.Add(parcela);
                        }
                    }
                }
            }

            return resultados;
        }
    }
}
