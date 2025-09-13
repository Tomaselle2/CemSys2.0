using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Parcelas;
using CemSys2.Interface.Tarifaria;
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
        private readonly IFacturasBD _facturasBD;
        private readonly ITarifariaBD _tarifariaBd;

        public ConcesionesBD(AppDbContext context, ITramiteBD tramiteBD, IFacturasBD facturasBD, ITarifariaBD tarifariaBD)
        {
            _context = context;
            _tramiteBD = tramiteBD;
            _facturasBD = facturasBD;
            _tarifariaBd = tarifariaBD;
        }

        public async Task<List<CantidadCuota>> CantidadCuotas()
        {
            return await _context.CantidadCuotas
                .OrderBy(c => c.Cuota)
                .ToListAsync();
        }

        public async Task<ContratoConcesion> ConsultarContratoConcesion(int tramiteId)
        {
            return await _context.ContratoConcesions
                .FirstAsync(c => c.IdTramite == tramiteId);
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
                            ParcelaId = contrato.ParcelaId,
                            CantidadAnios = contrato.CantidadAniosId,
                            Vencimiento = contrato.Vencimiento,
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

                        //registrar la parcela con el tramite
                        TramiteParcela tramiteParcela = new TramiteParcela
                        {
                            ParcelaId = contrato.ParcelaId,
                            TramiteId = tramiteId,
                            FechaRegistro = DateTime.Now,
                        };
                        _context.TramiteParcelas.Add(tramiteParcela);
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

        public async Task<bool> ModificarContratoConcesion(ContratoConcesion contrato)
        {
            bool exito = false;
            try
            {
                _context.ContratoConcesions.Update(contrato);
                await _context.SaveChangesAsync();
                exito = true;
            }
            catch (Exception)
            {
                exito = false;
            }

            return exito;
        }

        public async Task<bool> PasoPendienteDocumentacion(ContratoConcesion contrato, List<DTO_Titulares> titulares, int tipoConceptoTarifariaId)
        {
            bool exito = false;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Relacionar persona con trámite
                    // VERIFICAR SI LA RELACIÓN TRÁMITE-PERSONA YA EXISTE
                    foreach (var t in titulares)
                    {
                        bool relacionExistente = await _context.TramitePersonas
                        .AnyAsync(tp => tp.TramiteId == contrato.IdTramite && tp.PersonaId == t.Id);

                        // Solo crear la relación si no existe con cada titular
                        if (!relacionExistente)
                        {
                            TramitePersona tramitePersona = new TramitePersona
                            {
                                TramiteId = contrato.IdTramite,
                                PersonaId = t.Id
                            };
                            _context.TramitePersonas.Add(tramitePersona);
                            await _context.SaveChangesAsync();
                        }

                        //se relaciona el titular con la concesion tabla TitularesConcesion
                        TitularesContratoConcesion titularesContratoConcesion = new TitularesContratoConcesion
                        {
                            PersonaId = t.Id,
                            ContratoId = contrato.IdTramite,
                            Fecha = DateTime.Now
                        };
                        _context.TitularesContratoConcesions.Add(titularesContratoConcesion);
                        await _context.SaveChangesAsync();

                        //se guarda el historial de titulares por contrato
                        HistorialTitularesContrato historialTitularesContrato = new HistorialTitularesContrato
                        {
                            ContratoId = contrato.IdTramite,
                            PersonaId = t.Id,
                            FechaInicio = contrato.FechaGeneracion
                        };
                        _context.HistorialTitularesContratos.Add(historialTitularesContrato);
                        await _context.SaveChangesAsync();
                    }

                    //relaciona cada difunto con el tramite
                    var ListaDifuntos = await ListaDifuntosPorParcela(contrato.ParcelaId);
                    foreach (var difunto in ListaDifuntos)
                    {
                        bool relacionExistente = await _context.TramitePersonas
                        .AnyAsync(tp => tp.TramiteId == contrato.IdTramite && tp.PersonaId == difunto.DifuntoId);

                        // Solo crear la relación si no existe con cada difunto
                        if (!relacionExistente)
                        {
                            TramitePersona tramitePersona = new TramitePersona
                            {
                                TramiteId = contrato.IdTramite,
                                PersonaId = difunto.DifuntoId
                            };
                            _context.TramitePersonas.Add(tramitePersona);
                            await _context.SaveChangesAsync();
                        }
                    }

                    PreciosTarifaria precioConcesion = await _tarifariaBd.ConsultarUnPrecioTarifaria(contrato.PrecioTarifariaId);

                    //genero la factura
                    Factura factura = new Factura{
                        TramiteId = contrato.IdTramite,
                        FechaCreacion = contrato.FechaGeneracion,
                        Total = precioConcesion.Precio,
                        Pendiente = precioConcesion.Precio,
                        Visibilidad = true
                    };
                    int idFactura = await _facturasBD.RegistrarFactura(factura);

                    ConceptosFactura conceptoFactura = new ConceptosFactura
                    {
                        FacturaId = factura.Id,
                        ConceptoTarifariaId = precioConcesion.ConceptoTarifariaId,
                        PrecioUnitario = precioConcesion.Precio,
                        Cantidad = 1,
                        TipoConceptoFacturaId = tipoConceptoTarifariaId
                    };
                    int idConcepto = await _facturasBD.RegistrarConceptoFactura(conceptoFactura);

                   
                    //busco el tramite y actualizo el estado
                    Tramite tramite = await _tramiteBD.ConsultarTramite(contrato.IdTramite);
                    tramite.EstadoActualId = (int)EstadosContratoConcesion.PendienteDeDocumentacion;
                    int idTramite = await _tramiteBD.ModificarTramite(tramite); //actualizo el estado del tramite

                    // Actualizar el estado del trámite pasa el tramite a Pendiente de Documentacion
                    HistorialEstadoTramite estadoTramite = new HistorialEstadoTramite
                    {
                        EstadoTramiteId = tramite.EstadoActualId ?? 0, 
                        Fecha = DateTime.Now,
                        TramiteId = tramite.Id
                    };
                    _context.HistorialEstadoTramites.Add(estadoTramite);
                    await _context.SaveChangesAsync();


                    await transaction.CommitAsync();
                    exito = true;
                    
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Re-throw the exception after rolling back
                }
            }

            return exito;
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
                                aniosConcesion = reader.GetInt32(reader.GetOrdinal("anios")),
                                cantidadAniosId = reader.GetInt32(reader.GetOrdinal("cantidadAniosId"))
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

        //verifica si ya existe un contrato con ese numero de concesion
        public async Task<int> VerificarSiExisteContratoConcesion(string nroConcesion, int parcelaId)
        {
            // Normalizar el número de concesión removiendo todos los guiones
            string nroConcesionNormalizado = nroConcesion.Replace("-", "");

            // Obtener todos los contratos para la parcelaId específica
            var contratosDeParcela = await _context.ContratoConcesions
                .Where(c => c.ParcelaId == parcelaId)
                .Select(c => new { c.Concesion, c.IdTramite })
                .ToListAsync();

            // Buscar el contrato cuya concesión normalizada coincida
            foreach (var contrato in contratosDeParcela)
            {
                string concesionNormalizada = contrato.Concesion.Replace("-", "");
                if (concesionNormalizada == nroConcesionNormalizado)
                {
                    return contrato.IdTramite;
                }
            }

            return 0;
        }

        //devuelve los contratos de concesion paginados
        public async Task<DTO_Listado_Paginado_Concesiones> ListadoConcesiones(int paginaActual, int tamanoPagina)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var resultado = new DTO_Listado_Paginado_Concesiones
                {
                    PaginaActual = paginaActual,
                    TamanoPagina = tamanoPagina
                };

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_ListadoContratosConcesiones";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageNumber", paginaActual);
                    command.Parameters.AddWithValue("@PageSize", tamanoPagina);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // 1) Leer la lista de concesiones
                        while (await reader.ReadAsync())
                        {
                            var item = new DTO_Listado_Tabla_General_Concesiones
                            {
                                TramiteId = reader.GetInt32(reader.GetOrdinal("idTramite")),
                                NroConcesion = reader.IsDBNull(reader.GetOrdinal("concesion"))
                                                ? string.Empty
                                                : reader.GetString(reader.GetOrdinal("concesion")),
                                NombreSeccion = reader.GetString(reader.GetOrdinal("Seccion")),
                                TipoParcelaId = reader.GetInt32(reader.GetOrdinal("tipoParcela")),
                                NroParcela = reader.GetInt32(reader.GetOrdinal("NroParcela")),
                                NroFila = reader.GetInt32(reader.GetOrdinal("NroFila")),
                                Vencimiento = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("vencimiento"))),
                                EstadoActualId = reader.GetInt32(reader.GetOrdinal("estadoActualID")),
                                Difuntos = reader.IsDBNull(reader.GetOrdinal("Difuntos"))
                                           ? string.Empty
                                           : reader.GetString(reader.GetOrdinal("Difuntos")),
                                Titulares = reader.IsDBNull(reader.GetOrdinal("Titulares"))
                                            ? string.Empty
                                            : reader.GetString(reader.GetOrdinal("Titulares"))
                            };

                            resultado.Items.Add(item);
                        }

                        // 2) Pasar al siguiente resultset -> total de registros
                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            resultado.TotalRegistros = reader.GetInt32(reader.GetOrdinal("TotalRegistros"));
                        }
                    }
                }

                return resultado;
            }
        }
    }
}
