using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Parcelas;
using CemSys2.Interface.Tramite;
using CemSys2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CemSys2.Data
{
    public class ConcesionesBD : IConcesionesDB
    {
        public readonly AppDbContext _context;
        private readonly ITramiteBD _tramiteBD;

        public ConcesionesBD(AppDbContext context, ITramiteBD tramiteBD)
        {
            _context = context;
            _tramiteBD = tramiteBD;
        }

        public async Task<List<CantidadCuota>> CantidadCuotas()
        {
            return await _context.CantidadCuotas
                .OrderBy(c => c.Cuota)
                .ToListAsync();
        }

        //Obtiene los datos de la parcela para hacer un contrato de concesion
        public async Task<DTO_Datos_Concesion> DatosParcela(int parcelaId)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                DTO_Datos_Concesion datosConcesion = new DTO_Datos_Concesion();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DatosParcelaConcesion"; // Nombre del SP
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@parcelaId", parcelaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            datosConcesion = new DTO_Datos_Concesion
                            {
                                ParcelaId = reader.GetInt32(reader.GetOrdinal("parcelaId")),
                                TipoParcela = reader.GetInt32(reader.GetOrdinal("tipoParcela")),
                                SeccionId = reader.GetInt32(reader.GetOrdinal("seccionId")),
                                NombreSeccion = reader.IsDBNull(reader.GetOrdinal("NombreSeccion"))
                                               ? string.Empty
                                               : reader.GetString(reader.GetOrdinal("NombreSeccion")),
                                NroParcela = reader.GetInt32(reader.GetOrdinal("NroParcela")),
                                NroFila = reader.GetInt32(reader.GetOrdinal("NroFila"))
                            };   
                        }
                    }
                }

                return datosConcesion;
            }
        }

        //Genera el contrato de concesion
        public async Task<bool> GenerarContrato(DTO_DatosGenerarContratoConcesion contrato, Tramite tramite)
        {
            bool exito = false;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Registrar el trámite
                    int tramiteId = await _tramiteBD.RegistrarTramite(tramite);
                    if (tramiteId > 0)
                    {
                        // Crear el contrato de concesión
                        var nuevoContrato = new ContratoConcesion
                        {
                            IdTramite = tramiteId,
                            CantidadAnios = contrato.CantidadAnios,
                            Vencimiento = contrato.Vencimiento.ToDateTime(TimeOnly.MinValue),
                            Concesion = contrato.NroConcesion,
                            PrecioTarifariaId = contrato.PrecioId,
                            CuotaId = contrato.CuotaId,
                            PagoDescripcion = contrato.PagoDescripcion,
                            Visibilidad = true,
                            FechaGeneracion = contrato.fechaGeneracion,
                            Empleado = contrato.EmpleadoId,
                            TipoParcela = contrato.TipoParcela,
                            ContratoAnteriorId = contrato.ContratoAnteriorId,
                            Precio = contrato.Precio,
                        };

                        _context.ContratoConcesions.Add(nuevoContrato);
                        await _context.SaveChangesAsync();

                        // Actualizar el estado del trámite
                        HistorialEstadoTramite estadoTramite = new HistorialEstadoTramite
                        {
                            EstadoTramiteId = tramite.EstadoActualId ?? 0, //es iniciado la primera vez
                            Fecha = DateTime.Now,
                            TramiteId = tramiteId,
                        };

                        _context.HistorialEstadoTramites.Add(estadoTramite);
                        await _context.SaveChangesAsync();

                       await transaction.CommitAsync();
                        exito = true;
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Re-throw the exception after rolling back
                }
            }

            return exito;
        }

        //Obtene los difuntos actuales en parcela para hacer un contrato
        public async Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId)
        {
            var resultados = new List<DTO_Difuntos_Para_Concesion>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_GetDifuntosActualesPorParcela"; // Nombre del SP
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@parcelaId", parcelaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var difunto = new DTO_Difuntos_Para_Concesion
                            {
                                DifuntoId = reader.GetInt32(reader.GetOrdinal("DifuntoId")),
                                DNI = reader.IsDBNull(reader.GetOrdinal("DNI"))
                                      ? string.Empty
                                      : reader.GetString(reader.GetOrdinal("DNI")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre"))
                                         ? string.Empty
                                         : reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido"))
                                           ? string.Empty
                                           : reader.GetString(reader.GetOrdinal("Apellido")),
                                FechaIngreso = reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
                                EstadoDifunto = reader.IsDBNull(reader.GetOrdinal("EstadoDifunto"))
                                                ? string.Empty
                                                : reader.GetString(reader.GetOrdinal("EstadoDifunto"))
                            };

                            resultados.Add(difunto);
                        }
                    }
                }

                return resultados;
            }

        }

        //obtiene las parcelas sin contrato de concesion
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
                                           : reader.GetString(reader.GetOrdinal("Difuntos")),
                                EstadoTramiteIntroduccion = reader.GetInt32(reader.GetOrdinal("estadoTramite"))
                            };

                            resultados.Add(parcela);
                        }
                    }
                }
            }

            return resultados;
        }

        //obtiene los precios para hacer un contrato de concesion
        public async Task<List<DTO_Precios_Concesion>> PreciosConcesion(int conceptoTarifariaId, int seccionId, int nroFila)
        {
            var resultados = new List<DTO_Precios_Concesion>();

            //obtengo la tarifaria vigente, la fecha mas reciente
            var tarifariaVigente = await _context.Tarifarias
                .Where(t => t.Visibilidad == true)
                .OrderByDescending(t => t.FechaCreacionTarifaria)
                .FirstAsync();


            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "obtenerPreciosParcelaContrato"; // Nombre del SP
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@conceptoTarifariaId", conceptoTarifariaId);
                    command.Parameters.AddWithValue("@tarifarioId", tarifariaVigente.Id);
                    command.Parameters.AddWithValue("@seccionId", seccionId);
                    command.Parameters.AddWithValue("@nroFila", nroFila);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var precio = new DTO_Precios_Concesion
                            {
                                precioId = reader.GetInt32(reader.GetOrdinal("id")),
                                conceptoTarifariaId = reader.GetInt32(reader.GetOrdinal("conceptoTarifariaId")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                seccionId = reader.GetInt32(reader.GetOrdinal("seccionId")),
                                fila = reader.GetInt32(reader.GetOrdinal("nroFila")),
                                aniosConcesion = reader.GetInt32(reader.GetOrdinal("anios"))
                            };

                            resultados.Add(precio);
                        }
                    }
                }

                return resultados;
            }
        }

        //registra un nuevo titular para la concesion
        public async Task<Persona> RegistrarTitular(Persona titular)
        {
            // Asegurarnos de que la persona tenga visibilidad true al crearse
            titular.Visibilidad = true;
            titular.CategoriaPersona = (int)CategoriaPersonaEnum.Titular;

            // Agregar el contribuyente al contexto
            _context.Personas.Add(titular);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devolver el contribuyente con todos sus campos, incluyendo el ID generado
            return titular;
        }
    }
}
