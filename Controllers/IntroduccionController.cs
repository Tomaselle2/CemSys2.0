using CemSys2.Business;
using CemSys2.DTO.Factura;
using CemSys2.Enumerable;
using CemSys2.Interface.Archivos;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.ComponentModel.DataAnnotations;

namespace CemSys2.Controllers
{
    public class IntroduccionController : Controller
    {
        private readonly IIntroduccionBusiness _introduccionBusiness;
        private readonly IFacturaBusiness _facturaBusiness;
        private readonly ITarifariaBusiness _tarifariaBusiness;
        private readonly IArchivoBusiness _archivoBusiness;

        public IntroduccionController(IIntroduccionBusiness introduccionBusiness, IFacturaBusiness facturaBusiness, ITarifariaBusiness tarifariaBusiness, IArchivoBusiness archivoBusiness)
        {
            _introduccionBusiness = introduccionBusiness;
            _facturaBusiness = facturaBusiness;
            _tarifariaBusiness = tarifariaBusiness;
            _archivoBusiness = archivoBusiness;
        }

        public async Task<IActionResult> Index(int pagina = 1, string desdeFecha = null, string hastaFecha = null)
        {
            const int registrosPorPagina = 15;

            // Convertir las fechas de string a DateTime
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if(!string.IsNullOrEmpty(desdeFecha) && DateTime.TryParse(desdeFecha, out var tempDesde))
            {
                fechaDesde = tempDesde;
            }

            if (!string.IsNullOrEmpty(hastaFecha) && DateTime.TryParse(hastaFecha, out var tempHasta))
            {
                fechaHasta = tempHasta;
            }


            var (introducciones, totalRegistros) = await _introduccionBusiness.ListadoIntroducciones(fechaDesde, fechaHasta, registrosPorPagina, pagina);

            var viewModelIndex = new IntroduccionIndexVM
            {
                ListaIntroducciones = introducciones,
                PaginaActual = pagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina)
            };
            return View(viewModelIndex);
        }

        [HttpGet]
        public async Task<IActionResult> IntroduccionDifunto()
        { 
            IntroduccionDifuntoVM viewModel = new();
            await CargarCombos(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IntroduccionDifunto(IntroduccionDifuntoVM viewModel)
        {
            int tramiteId = 0; //inicializo el tramiteId en 0
            if (viewModel.NN)
            {
                // Si es NN, limpiar DNI y Nombre para evitar conflictos
                viewModel.Dni = null;
                viewModel.Nombre = null;
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(viewModel);
                viewModel.MensajeError = "Revise los campos incompletos o las advertencias";
                return View(viewModel);
            }

            try
            {
                if (viewModel.Dni.HasValue) //si DNI tiene algo
                {
                    Persona? difunto = await _introduccionBusiness.ConsultarDifunto(viewModel.Dni.ToString()); //consulto el dni
                    if (difunto != null) //si el resultado es != null esta en la base de datos
                    {
                        viewModel.MensajeError = $"El DNI {viewModel.Dni.ToString()} ya esta registrado";
                        await CargarCombos(viewModel);
                        return View(viewModel);
                    }
                }

                //Si llega hasta aquí, el difunto no existe, se puede registrar
                //acta defuncion
                ActaDefuncion actaDefuncion = viewModel.ActaDefuncion;

                // difunto
                Persona difuntoNuevo = new Persona //crea el difunto
                {
                    Visibilidad = true,
                    Dni = viewModel.Dni.HasValue ? viewModel.Dni.Value.ToString() : "nn",
                    Nombre = string.IsNullOrWhiteSpace(viewModel.Nombre) ? "nn" : viewModel.Nombre.Trim(),
                    Apellido = viewModel.Apellido.Trim(),
                    FechaNacimiento = viewModel.FechaNacimiento,
                    FechaDefuncion = viewModel.FechaDefuncion,
                    CategoriaPersona = 2, //id fallecido
                    Sexo = viewModel.Sexo,
                    EstadoDifunto = viewModel.EstadoDifuntoId,
                    InformacionAdicional = viewModel.InformacionAdicional,
                    DomicilioEnTirolesa = viewModel.DomicilioEnTirolesa,
                    FallecioEnTirolesa = viewModel.FallecioEnTirolesa
                };

                Parcela parcela = await _introduccionBusiness.ConsultarParcela(viewModel.ParcelaID.Value);
                bool placa = false;
                if (parcela.CantidadDifuntos >= 1)
                {
                    placa = viewModel.Placa.HasValue && viewModel.Placa.Value;
                }

                int? usuarioId = HttpContext.Session.GetInt32("idUsuario");

                tramiteId = await _introduccionBusiness.RegistrarIntroduccionCompleta(actaDefuncion, difuntoNuevo, viewModel.EmpleadoID.Value, viewModel.EmpresaFunebreID.Value,
                    viewModel.ParcelaID.Value, viewModel.FechaHoraIngreso.Value, usuarioId.Value, placa);
                if (tramiteId == 0)
                {
                    viewModel.MensajeError = "No se pudo registrar la introducción. Intente nuevamente.";
                    await CargarCombos(viewModel);
                    return View(viewModel);
                }

            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "Error al consultar el difunto: " + ex.Message;
                await CargarCombos(viewModel);
                return View(viewModel);
            }

            return RedirectToAction("ResumenIntroduccion", new { tramiteId = tramiteId });

        }

        private async Task CargarCombos(IntroduccionDifuntoVM viewModel)
        {
            try
            {
                viewModel.ListaEstadoDifunto = await _introduccionBusiness.ListaEstadoDifunto();
                viewModel.ListaTipoParcela = await _introduccionBusiness.ListaTipoParcela();
                viewModel.ListaEmpresasSepelio = await _introduccionBusiness.ListaEmpresasFunebres();
                viewModel.ListaEmpleados = await _introduccionBusiness.ListaEmpleados();

             
            
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = "Error al cargar: " + ex.Message;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            var secciones = await _introduccionBusiness.ListaSecciones(tipoParcelaId);
            return Json(secciones);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParcelasPorSeccion(int seccionId, int estadoDifuntoId)
        {
            var parcelas = await _introduccionBusiness.ListaParcelas(seccionId, estadoDifuntoId);
            return Json(parcelas);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarEmpresa(string nombreEmpresa)
        {
            try
            {
                int idEmpresa = await _introduccionBusiness.RegistrarEmpresaSepelio(new EmpresaFunebre { Nombre = nombreEmpresa });
                return Json(new { success = true, idEmpresa = idEmpresa, message = "Empresa agregada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al agregar empresa: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult VistaReportesIntroducciones()
        {
            return View();
        }

        //----------------------------------------------------------------------------
        //--------------------ACCIONES PANTALLA DE REPORTES DE INTRODUCCIONES-----------------
        [HttpGet]
        public async Task<JsonResult> ReporteGeneralIntroducciones(string opcion, string desdeFecha, string hastaFecha)
        {
            try
            {
                List<Introduccione> introducciones;

                if (opcion == "fecha")
                {
                    if (!DateTime.TryParse(desdeFecha, out var desde) || !DateTime.TryParse(hastaFecha, out var hasta))
                    {
                        return Json(new { success = false, message = "Fechas inválidas." });
                    }

                    introducciones = await _introduccionBusiness.ReporteIntroducciones(desde, hasta);
                }
                else
                {
                    introducciones = await _introduccionBusiness.ReporteIntroducciones();
                }

                if (introducciones == null || introducciones.Count == 0)
                {
                    return Json(new { success = false, message = "No se encontraron introducciones." });
                }

                // Procesamiento para el gráfico por mes (barras)
                var datosPorMes = introducciones
                    .Where(i => i.FechaIngreso.HasValue)
                    .GroupBy(i => new {
                        Mes = i.FechaIngreso.Value.Month,
                        Año = i.FechaIngreso.Value.Year
                    })
                    .Select(g => new {
                        mes = g.Key.Mes,
                        año = g.Key.Año,
                        cantidad = g.Count()
                    })
                    .OrderBy(x => x.año)
                    .ThenBy(x => x.mes)
                    .ToList();
                // Calcular el total general
                int total = introducciones.Count;


                // Procesamiento para el gráfico por tipo de parcela (torta)
                var datosPorTipo = introducciones
                    .Where(i => i.Parcela?.SeccionNavigation?.TipoParcelaNavigation != null)
                    .GroupBy(i => i.Parcela.SeccionNavigation.TipoParcelaNavigation.TipoParcela1)
                    .Select(g => new {
                        tipoParcela = g.Key,
                        cantidadPorTipo = g.Count()
                    })
                    .OrderByDescending(x => x.cantidadPorTipo)
                    .ToList();

                // Calcular fechas mínima y máxima
                var fechasIngreso = introducciones
                    .Where(i => i.FechaIngreso.HasValue)
                    .Select(i => i.FechaIngreso.Value)
                    .ToList();

                // Nuevo: Datos para el gráfico de lista
                var datosLista = datosPorTipo.Select(x => new {
                    tipo = x.tipoParcela,
                    cantidad = x.cantidadPorTipo,
                    porcentaje = Math.Round((x.cantidadPorTipo / (double)total) * 100, 1)
                }).ToList();

                // Nuevo: Procesamiento para el gráfico por empleado
                var datosPorEmpleado = introducciones
                .Where(i => i.EmpleadoNavigation != null) // Asumiendo que hay una relación con Empleado
                .GroupBy(i => new {
                    Id = i.EmpleadoNavigation.Id,
                    Nombre = i.EmpleadoNavigation.Nombre // Ajusta según tu modelo
                })
                .Select(g => new {
                    empleadoId = g.Key.Id,
                    nombreEmpleado = g.Key.Nombre,
                    cantidad = g.Count()
                })
                .OrderByDescending(x => x.cantidad)
                .ToList();

                var fechaMinima = fechasIngreso.Any() ? fechasIngreso.Min().ToString("dd-MM-yyyy") : null;
                var fechaMaxima = fechasIngreso.Any() ? fechasIngreso.Max().ToString("dd-MM-yyyy") : null;

                return Json(new
                {
                    success = true,
                    dataBarra= datosPorMes,    // Para el gráfico de barras
                    dataTorta= datosPorTipo,   // Para el gráfico de torta
                    fechaDesde = fechaMinima,
                    dataEmpleados = datosPorEmpleado,
                    fechaHasta = fechaMaxima,
                    dataLista = datosLista,  // ← Nuevo conjunto de datos
                    total = total,          // ← Total general
                    message = "Datos obtenidos correctamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error al generar el reporte: {ex.Message}"
                });
            }
        }



        //----------------------------------------------------------------------------
        //--------------------ACCIONES SOBRE EL RESUMEN DE INTRODUCCION-----------------
        //recontruye el ViewModel de ResumenIntroduccion cuando hay un error de validacion

        private async Task<ResumenIntroduccionVM> ReconstruirViewModel(int tramiteId)
        {
            var resumen = await _introduccionBusiness.ObtenerResumenIntroduccion(tramiteId);
            var factura = await _facturaBusiness.ConsultarFacturaInternaPorTramiteId(tramiteId);

            return new ResumenIntroduccionVM
            {
                ResumenIntroduccion = resumen,
                FacturaInterna = factura,
                ListaConceptosFactura = await _facturaBusiness.ListaConceptosFacturaInternaPorFactura(factura.Id),
                ListaFacturas = await _facturaBusiness.ListaFacturasPorTramiteId(tramiteId),
                IdTramite = tramiteId,
                IdFactura = factura.Id,
                infoAdicional = resumen.FirstOrDefault()?.informacionAdicionalTramite,
                HistorialEstadoTramites = await _introduccionBusiness.HistorialEstadoTramites(tramiteId),
                ListaConceptosTarifaria = _facturaBusiness.ListaConceptoTarifariaConPreciosConLogicaNegocio( await _facturaBusiness.ListaConceptoTarifariaIntroduccion(await _tarifariaBusiness.ConsultarIdTarifariaVigente()), resumen[0].FallecioEnTirolesa),
                MontoMinimoFondo = await _tarifariaBusiness.ConsultarMontoMinimoFondoActual(),
                PorcentajeFondo = await _tarifariaBusiness.ConsultarPorcentajeFondoActual(),
                ListaArchivos = await _archivoBusiness.ListaArchivosTramiteId(tramiteId)
            };
        }

        //pantalla de resumen
        [HttpGet]
        public async Task<IActionResult> ResumenIntroduccion(int tramiteId)
        {
            ResumenIntroduccionVM viewModel = new();
            try
            {
                viewModel = await ReconstruirViewModel(tramiteId);

                if (viewModel.ResumenIntroduccion == null || viewModel.ResumenIntroduccion.Count == 0)
                {
                    return NotFound("No se encontraron datos para el trámite especificado.");
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                viewModel = new ResumenIntroduccionVM
                {
                    MensajeError = ex.Message,
                };
                return View(viewModel);
            }
        }

        //imprime en PDF el resumen del tramite
        [HttpGet]
        public async Task<IActionResult> ResumenIntroduccionEnPDF(int idtramite)
        {
            var resumen = await _introduccionBusiness.ObtenerResumenIntroduccion(idtramite);
            if (resumen == null || resumen.Count == 0)
            {
                return NotFound("No se encontraron datos para el trámite especificado.");
            }

            var viewModel = new ResumenIntroduccionVM
            {
                ResumenIntroduccion = resumen,
            };

            var pdf = new ViewAsPdf("ResumenIntroduccionEnPDF", viewModel)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 5, 5, 10),
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = $"Tramite introduccion {viewModel.ResumenIntroduccion[0].Id}.pdf"
            };

            // Agregá el valor directamente a su ViewData actual
            pdf.ViewData["BaseUrl"] = $"{Request.Scheme}://{Request.Host}";
            pdf.ViewData["UsuarioLogueado"] = HttpContext.Session.GetString("nombreUsuario");


            return pdf;
        }

        private void CargarDatosContribuyenteyFactura(ResumenIntroduccionVM vmNuevo, ResumenIntroduccionVM vmAnterior)
        {
            vmNuevo.ListaDetalleFactura = vmAnterior.ListaDetalleFactura; //mantener los conceptos seleccionados
            vmNuevo.IdContribuyente = vmAnterior.IdContribuyente;
            vmNuevo.Nombre = vmAnterior.Nombre;
            vmNuevo.Apellido = vmAnterior.Apellido;
            vmNuevo.Sexo = vmAnterior.Sexo;
            vmNuevo.Dni = vmAnterior.Dni;
        }

        //Emitir factura
        [HttpPost]
        public async Task<IActionResult> EmitirFactura(ResumenIntroduccionVM viewModel)
        {
            int facturaExito = 0;
            //validar el modelo
            if (!ModelState.IsValid)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.MensajeError = "Por favor, complete todos los campos obligatorios.";
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                return View("ResumenIntroduccion", vmCompleto);
            }
            
            try
            {
                DTO_VerificarDetalleFactura dto = new DTO_VerificarDetalleFactura
                {
                    Contribuyente = viewModel.IdContribuyente,
                    DetallesFactura = viewModel.ListaDetalleFactura,
                    Pendiente = viewModel.ResumenIntroduccion[0].Pendiente,
                    Decreto = viewModel.Decreto,
                    Archivo = viewModel.Decreto ? viewModel.ArchivoDecreto : null, //si es decreto, el archivo es obligatorio
                    TramiteId = viewModel.IdTramite.Value,
                    MontoDecreto = viewModel.MontoDecreto,
                    Descripcion = viewModel.Descripcion?.Trim() ?? string.Empty,
                    EstadoFacturaId = (int)EstadosFactura.Creado, //cambiar dependiento del caso,
                    UsuarioEmiteId = HttpContext.Session.GetInt32("idUsuario")
                };

               facturaExito = await _facturaBusiness.VerificarDetalleFactura(dto);
            }catch(ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                return View("ResumenIntroduccion", vmCompleto);
            }catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo generar la factura: " + ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }

            if(facturaExito > 0 && !viewModel.Decreto)
            {
                await _facturaBusiness.PasarFacturaEstadoEmitir(facturaExito);
                TempData["MensajeExito"] = "Factura emitida con éxito";
            }

            if (facturaExito > 0 && viewModel.Decreto)
            {
                TempData["MensajeExito"] = "Decreto emitido con éxito";
            }

            return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });

        }

        //Editar Archivo
        [HttpPost]
        public async Task<IActionResult> EditarArchivo(ResumenIntroduccionVM viewModel)
        {

            if (!viewModel.EsEdicion && viewModel.ArchivoDecreto == null)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
            }

            // Validar Concepto
            if (string.IsNullOrWhiteSpace(viewModel.Descripcion))
            {
                ModelState.AddModelError("Concepto", "El concepto es obligatorio.");
            }

            // Validar archivo SOLO si se sube uno nuevo
            if (viewModel.ArchivoDecreto != null && viewModel.ArchivoDecreto.Length > 0)
            {
                var extension = Path.GetExtension(viewModel.ArchivoDecreto.FileName).ToLower();
                var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
                if (!permitidas.Contains(extension))
                {
                    ModelState.AddModelError("ArchivoRecibo", "Solo se permiten archivos PNG, JPG o PDF.");
                }
            }

            //if (!ModelState.IsValid)
            //{
            //    var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
            //    return View("ResumenIntroduccion", vmCompleto);
            //}

            try
            {
                await _introduccionBusiness.EditarReciboFactura(
                    viewModel.IdRecibo.Value,
                    viewModel.Descripcion!,
                    viewModel.ArchivoDecreto
                );

                TempData["MensajeExito"] = "Recibo editado con éxito";
                return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });
            }
            catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.MensajeError = ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }
        }

        //actualiza el info adicional del tramite
        [HttpPost]
        public async Task<IActionResult> ActualizarInfoAdicionalTramite(ResumenIntroduccionVM viewModel)
        {

            try
            {
                await _introduccionBusiness.ActualizarInfoAdicionalTramite(viewModel.IdTramite.Value, viewModel.infoAdicional);
                TempData["MensajeExito"] = "Actualización exitosa";
            }
            catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Descripcion = viewModel.Descripcion?.Trim();
                vmCompleto.MontoDecreto = viewModel.MontoDecreto;
                viewModel.MensajeError = ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }
            return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });
        }

        

        //finaliza el tramite introduccion
        [HttpPost]
        public async Task<IActionResult> FinalizarTramite(ResumenIntroduccionVM viewModel)
        {
            try
            {
                await _introduccionBusiness.FinalizarTramite(viewModel.IdTramite.Value);
                TempData["MensajeExito"] = "Trámite finalizado";
            }
            catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Descripcion = viewModel.Descripcion;
                vmCompleto.MontoDecreto = viewModel.MontoDecreto;
                viewModel.MensajeError = ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }
            return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });

        }

        //reportesGraficos
        [HttpPost]
        public IActionResult ReporteGraficosPDF(string imagenBase64, string fechaDesde, string fechaHasta)
        {
            var pdf = new ViewAsPdf("ReporteGraficosPDF")
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 5, 5, 10),
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = $"Reporte.pdf"
            };

            // Agregá el valor directamente a su ViewData actual
            pdf.ViewData["ImagenBase64"] = imagenBase64;
            pdf.ViewData["BaseUrl"] = $"{Request.Scheme}://{Request.Host}";
            pdf.ViewData["FechaDesde"] = fechaDesde;
            pdf.ViewData["FechaHasta"] = fechaHasta;
            pdf.ViewData["UsuarioLogueado"] = HttpContext.Session.GetString("nombreUsuario");


            return pdf;
        }

      

        //Anular factura
        [HttpPost]
        public async Task<IActionResult> AnularFactura(ResumenIntroduccionVM viewModel)
        {
            try
            {
                await _facturaBusiness.PasarFacturaEstadoAnulado(viewModel.IdFactura.Value, viewModel.MotivoAnulacion);
                TempData["MensajeExito"] = "Factura anulada con éxito";

            }catch(ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                return View("ResumenIntroduccion", vmCompleto);
            }catch(InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                return View("ResumenIntroduccion", vmCompleto);
            }
            catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo eliminar la factura: " + ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }
            return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });
        }

        
    }
}
