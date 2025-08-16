using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Reportes;
using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Introduccion;
using CemSys2.Models;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CemSys2.Data
{
    public class IntroduccionBD : IIntroduccionBD
    {
        private readonly AppDbContext _context;
        private readonly IRepositoryDB<EstadoDifunto> _estadoDifuntoBD;
        private readonly IRepositoryDB<TipoParcela> _tipoParcelaBD;
        private readonly IRepositoryDB<EmpresaFunebre> _empresaFunebreBD;

        private int tipoConceptosTarifariaId_Contribucion = 2;
        private int tipoConceptosTarifariaId_RegistroCivil = 5;
        private int tipoConceptosTarifariaId_DerechoDeOficina = 6;
        private int tipoConceptosTarifariaId_Generales = 1;

        private decimal porcentajeFondo = 0.05m; // 5% del fondo
        private decimal montoMinimoDeFondo = 500m;

        public IntroduccionBD(AppDbContext context, IRepositoryDB<EstadoDifunto> estadoDifuntoBD, IRepositoryDB<TipoParcela> tipoParcelaBD, IRepositoryDB<EmpresaFunebre> empresaFunebreBD)
        {
            _context = context;
            _estadoDifuntoBD = estadoDifuntoBD;
            _tipoParcelaBD = tipoParcelaBD;
            _empresaFunebreBD = empresaFunebreBD;
        }



        //Lista para combos
        public async Task<List<EstadoDifunto>> ListaEstadoDifunto()
        {
            return await _estadoDifuntoBD.EmitirListado();
        }

        public async Task<List<TipoParcela>> ListaTipoParcela()
        {
            return await _tipoParcelaBD.EmitirListado();
        }

        public async Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela)
        {
            return await _context.Secciones
                .Where(s => s.TipoParcela == idTipoParcela && s.Visibilidad == true)
                .Select(s => new DTO_SeccionIntroduccion
                {
                    Id = s.Id,
                    Nombre = s.Nombre
                }).ToListAsync();
        }

        public async Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId)
        {
            var tipoParcela = await _context.Secciones
                .Where(s => s.Id == idSeccion)
                .Select(s => s.TipoParcela)
                .FirstOrDefaultAsync();

            var parcelasQuery = _context.Parcelas
                .Where(p => p.Seccion == idSeccion && p.Visibilidad == true);

            if (estadoDifuntoId == 1) //cuerpo completo
            {
                if (tipoParcela == 1) //nicho
                {
                    parcelasQuery = parcelasQuery.Where(p => p.CantidadDifuntos == 0 && p.TipoNicho != 2);
                }
                // si tipoParcela != 1 no aplicamos más filtros
            }
            else
            {
                if (tipoParcela == 1 || tipoParcela == 2) //nicho o fosa
                {
                    parcelasQuery = parcelasQuery.Where(p => p.CantidadDifuntos < 5);
                }
                else if (tipoParcela == 3) //panteon
                {
                    parcelasQuery = parcelasQuery.Where(p => p.CantidadDifuntos < 20);
                }
            }

            return await parcelasQuery
                .Select(p => new DTO_parcelaIntroduccion
                {
                    Id = p.Id,
                    NroParcela = p.NroParcela,
                    NroFila = p.NroFila,
                    SeccionId = p.Seccion,
                    CantidadDifuntos = p.CantidadDifuntos,
                    NombrePanteon = p.NombrePanteon,
                    TipoNichoId = p.TipoNicho,
                    TipoPanteonId = p.TipoPanteonId
                }).ToListAsync();
        }

        public async Task<List<EmpresaFunebre>> ListaEmpresasFunebres()
        {
            // Filtrar empresas que están marcadas como visibles
            List<EmpresaFunebre> listadoVisible = new List<EmpresaFunebre>();
            listadoVisible =  await _empresaFunebreBD.EmitirListado();
            return listadoVisible
                .Where(e => e.Visibilidad == true)
                .ToList();
        }

        public async Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados()
        {
            return await _context.Usuarios
                .Where(u => u.Visibilidad == true) 
                .Select(u => new DTO_UsuarioIntroduccion
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                }).ToListAsync();
        }

        public async Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model)
        {
            model.Visibilidad = true;
            return await _empresaFunebreBD.Registrar(model);
        }

        public async Task<Persona?> ConsultarDifunto(string dni)
        {
           return await _context.Personas.Where(p => p.Visibilidad == true && p.Dni == dni).FirstOrDefaultAsync();
        }

        public async Task<int> RegistrarIntroduccionCompleta(ActaDefuncion actaDefuncion, Persona difunto, int empleadoId, int empresaSepelioId, int ParcelaId, DateTime fechaIngreso, List<ConceptosFactura> conceptosFacturas)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Registrar Acta de Defunción
                _context.ActaDefuncions.Add(actaDefuncion);
                await _context.SaveChangesAsync();

                var estadoDifunto = await _context.EstadoDifuntos
                    .Where(x => x.Id == difunto.EstadoDifunto)
                    .Select(x => x.Estado)
                    .FirstOrDefaultAsync();

                // Asignar el acta al difunto
                difunto.ActaDefuncion = actaDefuncion.Id;
                _context.Personas.Add(difunto);
                await _context.SaveChangesAsync();


                int tipoTramiteId = 1; // fijo introduccion
                int estadoTramiteId = (int)EstadosIntroduccion.Registrado; // estado inicial, "Registrado"

                // Registrar Trámite
                Tramite tramite = new Tramite
                {
                    Id = await ObtenerProximoIdTramite(),
                    TipoTramiteId = tipoTramiteId,
                    FechaCreacion = DateTime.Now,
                    Usuario = empleadoId, 
                    Visibilidad = true,
                    EstadoActualId = estadoTramiteId
                };

                _context.Tramites.Add(tramite);
                await _context.SaveChangesAsync();

                // Relacionar persona con trámite
                TramitePersona tramitePersona = new TramitePersona
                {
                    TramiteId = tramite.Id,
                    PersonaId = difunto.IdPersona
                };
                _context.TramitePersonas.Add(tramitePersona);
                await _context.SaveChangesAsync();

                // Relacionar parcela con trámite
                TramiteParcela tramiteParcela = new TramiteParcela
                {
                    TramiteId = tramite.Id,
                    ParcelaId = ParcelaId
                };
                _context.TramiteParcelas.Add(tramiteParcela);
                await _context.SaveChangesAsync();

                // Registrar Historial Estado Trámite
                HistorialEstadoTramite historial = new HistorialEstadoTramite
                {
                    TramiteId = tramite.Id,
                    EstadoTramiteId = estadoTramiteId,
                    Fecha = DateTime.Now
                };
                _context.HistorialEstadoTramites.Add(historial);

                // Registrar Introducción
                Introduccione introduccion = new Introduccione
                {
                    IdTramite = tramite.Id,
                    Visibilidad = true,
                    FechaIngreso = fechaIngreso,
                    Empleado = tramite.Usuario,
                    EmpresaFunebre = empresaSepelioId, 
                    ParcelaId = ParcelaId, 
                    DifuntoId = difunto.IdPersona,
                    EstadoDifunto = estadoDifunto,
                    IntroduccionNueva = true,
                    FechaRetiro = null
                };
                _context.Introducciones.Add(introduccion);

                Parcela? parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == ParcelaId);
                if(parcela != null)
                    parcela.CantidadDifuntos++; //suma uno a cantidad de difuntos

                await _context.SaveChangesAsync();
                
                // Registrar ParcelaDifuntos
                ParcelaDifunto parcelaDifunto = new ParcelaDifunto
                {
                    ParcelaId = introduccion.ParcelaId,
                    DifuntoId = difunto.IdPersona,
                    FechaIngreso = DateTime.Now,
                    EstadoActual = true,
                    TramiteIngresoId = tramite.Id
                };
                _context.ParcelaDifuntos.Add(parcelaDifunto);

                await _context.SaveChangesAsync();

                //facturacion

                //separar los conceptos por tipo de conceptoTarifaria
                List<ConceptosFactura> conceptosContribucion = conceptosFacturas
                    .Where(c => c.TipoConceptoFacturaId == tipoConceptosTarifariaId_Contribucion).ToList();

                List<ConceptosFactura> conceptosRegistroCivil = conceptosFacturas
                    .Where(c => c.TipoConceptoFacturaId == tipoConceptosTarifariaId_RegistroCivil).ToList();

                List<ConceptosFactura> conceptosDerechoDeOficina = conceptosFacturas
                    .Where(c => c.TipoConceptoFacturaId == tipoConceptosTarifariaId_DerechoDeOficina).ToList();

                List<ConceptosFactura> conceptosGenerales = conceptosFacturas
                    .Where(c => c.TipoConceptoFacturaId == tipoConceptosTarifariaId_Generales).ToList();

                //sumar los montos y sumar el 5% del fondo, este debe ser una variable de la tarifaria vigente
                decimal totalContribucion = conceptosContribucion.Sum(c => c.PrecioUnitario * c.Cantidad);
                decimal calcular5porciento = totalContribucion * porcentajeFondo;
                if(calcular5porciento < montoMinimoDeFondo)
                {
                    totalContribucion += montoMinimoDeFondo;
                }
                else
                {
                    totalContribucion += totalContribucion * porcentajeFondo; //suma el 5%
                }

                decimal totalRegistroCivil = conceptosRegistroCivil.Sum(c => c.PrecioUnitario * c.Cantidad);
                calcular5porciento = totalRegistroCivil * porcentajeFondo;
                if (calcular5porciento < montoMinimoDeFondo)
                {
                    totalRegistroCivil += montoMinimoDeFondo;
                }
                else
                {
                    totalRegistroCivil += totalRegistroCivil * porcentajeFondo; //suma el 5%
                }


                decimal totalDerechoDeOficina = conceptosDerechoDeOficina.Sum(c => c.PrecioUnitario * c.Cantidad);
                calcular5porciento = totalDerechoDeOficina * porcentajeFondo;
                if (calcular5porciento < montoMinimoDeFondo)
                {
                    totalDerechoDeOficina += montoMinimoDeFondo;
                }
                else
                {
                    totalDerechoDeOficina += totalDerechoDeOficina * porcentajeFondo; //suma el 5%
                }

                decimal totalGenerales = conceptosGenerales.Sum(c => c.PrecioUnitario * c.Cantidad);
                if(totalGenerales != 0)
                {
                    calcular5porciento = totalGenerales * porcentajeFondo;
                    if (calcular5porciento < montoMinimoDeFondo)
                    {
                        totalGenerales += montoMinimoDeFondo;
                    }
                    else
                    {
                        totalGenerales += totalGenerales * porcentajeFondo; //suma el 5%
                    }
                }
                


                // 1. Calcular total factura
                decimal totalFactura = totalContribucion + totalRegistroCivil + totalDerechoDeOficina + totalGenerales;

                // 2. Registrar la factura
                Factura factura = new Factura
                {
                    TramiteId = tramite.Id,
                    FechaCreacion = DateTime.Now,
                    Total = totalFactura,
                    Pendiente = totalFactura,
                    Visibilidad = true
                };
                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                // 3. Obtener el ID generado para la factura
                int facturaId = factura.Id;

                // 4. Registrar los conceptos de la factura
                foreach (var concepto in conceptosFacturas)
                {
                    ConceptosFactura conceptoFactura = new ConceptosFactura
                    {
                        FacturaId = facturaId,
                        ConceptoTarifariaId = concepto.ConceptoTarifariaId,
                        PrecioUnitario = concepto.PrecioUnitario,
                        Cantidad = concepto.Cantidad,
                        TipoConceptoFacturaId = concepto.TipoConceptoFacturaId,

                    };
                    _context.ConceptosFacturas.Add(conceptoFactura);
                }

                await _context.SaveChangesAsync();


                await transaction.CommitAsync();
                return tramite.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<int> ObtenerProximoIdTramite()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }

        public async Task<(List<Introduccione> introducciones, int totalRegistros)> ListadoIntroducciones(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int registrosPorPagina = 10, int pagina = 1)
        {
            var query = _context.Introducciones
             .Include(d => d.Difunto)
             .Include(p => p.Parcela)
             .ThenInclude(s => s.SeccionNavigation)
             .AsQueryable();

            // Aplicar filtros de fecha si existen
            if (fechaDesde.HasValue)
            {
                query = query.Where(x => x.FechaIngreso >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                // Añadir un día para incluir todo el día hasta
                query = query.Where(x => x.FechaIngreso < fechaHasta.Value.AddDays(1));
            }

            var totalRegistros = await query.CountAsync();

            var introducciones = await query
                .OrderByDescending(x => x.IdTramite)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            return (introducciones, totalRegistros);
        }


        public async Task ActualizarInfoAdicionalTramite(int tramiteId, string infoAdicional)
        {
            Introduccione introduccion = await _context.Introducciones.FirstAsync(f=>f.IdTramite == tramiteId);
            introduccion.InformacionAdicional = infoAdicional;
            _context.Introducciones.Update(introduccion);
            await _context.SaveChangesAsync();
        }



        //reportes
        public async Task<List<Introduccione>> ReporteIntroducciones(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = _context.Introducciones
                .Include(i => i.Parcela)
                    .ThenInclude(p => p.SeccionNavigation)
                        .ThenInclude(s => s.TipoParcelaNavigation)
                    .Include(e => e.EmpresaFunebreNavigation)
                    .Include(em => em.EmpleadoNavigation)
                        .Where(i => i.FechaIngreso.HasValue);

            if(desde.HasValue && hasta.HasValue)
            {
                query = query.Where(i => i.FechaIngreso >= desde.Value && i.FechaIngreso <= hasta);
            }

            return await query.ToListAsync();
        }

        public async Task<List<DTO_Resumen_Introduccion>> ObtenerResumenIntroduccion(int idTramite)
        {
            var resultado = new List<DTO_Resumen_Introduccion>();
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ResumenIntroduccion";
                    command.CommandType = CommandType.StoredProcedure;
                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@IdTramite";
                    parametro.Value = idTramite;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dto = new DTO_Resumen_Introduccion
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FechaIngreso = reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
                                Empresa = reader.GetString(reader.GetOrdinal("Empresa")),
                                dni = reader.IsDBNull(reader.GetOrdinal("dni")) ? null : reader.GetString(reader.GetOrdinal("dni")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                                FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fechaNacimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fechaNacimiento")),
                                FechaDefuncion = reader.GetDateTime(reader.GetOrdinal("fechaDefuncion")),
                                EstadoDifunto = reader.GetString(reader.GetOrdinal("EstadoDifunto")),
                                InformacionAdicional = reader.IsDBNull(reader.GetOrdinal("informacionAdicional")) ? null : reader.GetString(reader.GetOrdinal("informacionAdicional")),
                                Acta = reader.IsDBNull(reader.GetOrdinal("acta")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("acta")),
                                Tomo = reader.IsDBNull(reader.GetOrdinal("tomo")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("tomo")),
                                Folio = reader.IsDBNull(reader.GetOrdinal("folio")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("folio")),
                                Serie = reader.IsDBNull(reader.GetOrdinal("serie")) ? "" : reader.GetString(reader.GetOrdinal("serie")),
                                Age = reader.IsDBNull(reader.GetOrdinal("age")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("age")),
                                Empleado = reader.GetString(reader.GetOrdinal("Empleado")),
                                NroParcela = reader.GetInt32(reader.GetOrdinal("NroParcela")).ToString(),
                                NroFila = reader.GetInt32(reader.GetOrdinal("NroFila")).ToString(),
                                Seccion = reader.GetString(reader.GetOrdinal("Seccion")),
                                TipoParcela = reader.GetInt32(reader.GetOrdinal("TipoParcela")),
                                DomicilioEnTirolesa = reader.IsDBNull(reader.GetOrdinal("domicilioEnTirolesa")) ? false: reader.GetBoolean(reader.GetOrdinal("domicilioEnTirolesa")),
                                FallecioEnTirolesa = reader.IsDBNull(reader.GetOrdinal("fallecioEnTirolesa")) ? false : reader.GetBoolean(reader.GetOrdinal("fallecioEnTirolesa")),
                                CantidadDifuntos = reader.GetInt32(reader.GetOrdinal("cantidadDifuntos")),
                                estadoTramite = reader.GetInt32(reader.GetOrdinal("estadoActualID")),
                                informacionAdicionalTramite = reader.IsDBNull(reader.GetOrdinal("informacionAdicionalTramite")) ? "" : reader.GetString(reader.GetOrdinal("informacionAdicionalTramite"))
                            };
                            resultado.Add(dto);
                        }
                    }
                }
            }
            return resultado;
        }

        //facturacion

        public async Task<Tarifaria> TarifariaVigente()
        {
            var tarifaria = await _context.Tarifarias
                .Where(t => t.Visibilidad == true)
                .OrderByDescending(t => t.FechaCreacionTarifaria)
                .FirstOrDefaultAsync();

            return tarifaria ?? throw new Exception("No se encontró una tarifaria vigente.");
        }

        public async Task<PreciosTarifaria?> PrecioTarifaria(int tarifariaVigente, int conceptoTarifaria)
        {
            return await _context.PreciosTarifarias
                .Include(p => p.ConceptoTarifaria)
                .Where(p => p.TarifarioId == tarifariaVigente && p.ConceptoTarifariaId == conceptoTarifaria)
                .FirstOrDefaultAsync();
        }

        public Task GenerarFactura(List<ConceptosFactura> conceptosFacturas)
        {
            throw new NotImplementedException();
        }

        public async Task<Parcela> ConsultarParcela(int idParcela)
        {
            return await _context.Parcelas
                .Include(p => p.SeccionNavigation)
                .ThenInclude(s => s.TipoParcelaNavigation)
                .FirstOrDefaultAsync(p => p.Id == idParcela && p.Visibilidad == true);
        }

        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _context.Facturas
                .FirstOrDefaultAsync(f => f.TramiteId == idTramite);
        }

        public async Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura)
        {
            return await _context.ConceptosFacturas
                .Include(c => c.ConceptoTarifaria)
                .Where(c => c.FacturaId == idFactura)
                .ToListAsync();
        }

        public async Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️ Insertar ReciboFactura
                var reciboFactura = new RecibosFactura
                {
                    FacturaId = recibo.FacturaId,
                    FechaPago = DateTime.Now,
                    Concepto = recibo.Concepto!,
                    Monto = recibo.Monto,
                    Decreto = recibo.Decreto
                };
                _context.RecibosFacturas.Add(reciboFactura);
                await _context.SaveChangesAsync();

                // 2️ Insertar archivo en ArchivosDocumentacion (FILESTREAM)
                byte[] contenido;
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }
                CategoriaArchivosEnum categoriaArchivo = CategoriaArchivosEnum.Recibo;
                var archivoRecibo = new ArchivosDocumentacion
                {
                    CategoriaArchivo = categoriaArchivo.ToString(),
                    ReciboId = reciboFactura.Id,
                    NombreArchivo = Path.GetFileName(archivo.FileName),
                    TipoArchivo = mimeType,
                    TamanoBytes = archivo.Length,
                    Contenido = contenido,
                    Descripcion = $"Recibo {recibo.Id} - Factura {reciboFactura.FacturaId}",
                    FechaCreacion = DateTime.Now,
                    Visibilidad = true,
                };
                _context.ArchivosDocumentacions.Add(archivoRecibo);
                await _context.SaveChangesAsync();

                // 3️⃣ Actualizar FK del Recibo con el archivoID
                reciboFactura.ArchivoId = archivoRecibo.ArchivoId;
                _context.RecibosFacturas.Update(reciboFactura);
                await _context.SaveChangesAsync();


                //busco la factura
                Factura factura = await _context.Facturas.FirstAsync(f => f.Id == reciboFactura.FacturaId);

                //busco el tramite
                Tramite tramite = await _context.Tramites.FirstAsync(t => t.Id == tramiteId);

                if (factura != null)
                {
                    //resto del monto que llega, nunca puede ser mayor que el pendiente
                    factura.Pendiente = factura.Pendiente - reciboFactura.Monto;

                    if (factura.Pendiente <= 0) //se abono todo
                    {
                        int estadoTramiteId = (int)EstadosIntroduccion.Cobrado;

                        //se agrega el estado en historial estado
                        HistorialEstadoTramite historial = new HistorialEstadoTramite
                        {
                            TramiteId = tramite.Id,
                            EstadoTramiteId = estadoTramiteId,
                            Fecha = DateTime.Now
                        };
                        _context.HistorialEstadoTramites.Add(historial);
                        await _context.SaveChangesAsync();

                        //se actualiza el estado actual en el tramite
                        tramite.EstadoActualId = estadoTramiteId;
                        _context.Tramites.Update(tramite);
                        await _context.SaveChangesAsync();

                        //actualizo la factura
                        factura.Pendiente = 0;
                        _context.Facturas.Update(factura);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Facturas.Update(factura);
                        await _context.SaveChangesAsync();
                    }
                }

                
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId)
        {
            return await _context.RecibosFacturas.Where(f=>f.FacturaId == facturaId).OrderByDescending(t=> t.FechaPago).ToListAsync();
        }

        public async Task FinalizarTramite(int tramiteId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //busco el tramite
                Tramite tramite = await _context.Tramites.FirstAsync(t => t.Id == tramiteId);

                int estadoTramiteId = (int)EstadosIntroduccion.Finalizado;

                //se agrega el estado en historial estado
                HistorialEstadoTramite historial = new HistorialEstadoTramite
                {
                    TramiteId = tramite.Id,
                    EstadoTramiteId = estadoTramiteId,
                    Fecha = DateTime.Now
                };
                _context.HistorialEstadoTramites.Add(historial);
                await _context.SaveChangesAsync();

                //se actualiza el estado actual en el tramite
                tramite.EstadoActualId = estadoTramiteId;
                _context.Tramites.Update(tramite);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ArchivosDocumentacion> ObtenerArchivo(Guid archivoGuid)
        {
            return await _context.ArchivosDocumentacions.FirstOrDefaultAsync(a => a.ArchivoId == archivoGuid);
        }

        public async Task<Persona> BuscarContribuyente(string DniContribuyente, string sexo)
        {
            return await _context.Personas.FirstOrDefaultAsync(p => p.Visibilidad == true && p.Dni == DniContribuyente &&
            p.CategoriaPersona != (int)CategoriaPersonaEnum.Fallecido && p.Sexo == sexo);
        }

        public async Task<Persona> RegistrarContribuyente(Persona contribuyente)
        {
            // Asegurarnos de que la persona tenga visibilidad true al crearse
            contribuyente.Visibilidad = true;
            contribuyente.CategoriaPersona = (int)CategoriaPersonaEnum.Contribuyente;

            // Agregar el contribuyente al contexto
            _context.Personas.Add(contribuyente);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devolver el contribuyente con todos sus campos, incluyendo el ID generado
            return contribuyente;
        }
    }
}
