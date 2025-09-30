using CemSys2.Business;
using CemSys2.DTO.Factura;
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

        public IntroduccionController(IIntroduccionBusiness introduccionBusiness, IFacturaBusiness facturaBusiness, ITarifariaBusiness tarifariaBusiness)
        {
            _introduccionBusiness = introduccionBusiness;
            _facturaBusiness = facturaBusiness;
            _tarifariaBusiness = tarifariaBusiness;
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
                ListaRecibosFactura = await _introduccionBusiness.ListaRecibosFactura(factura.Id),
                IdTramite = tramiteId,
                IdFactura = factura.Id,
                infoAdicional = resumen.FirstOrDefault()?.informacionAdicionalTramite,
                HistorialEstadoTramites = await _introduccionBusiness.HistorialEstadoTramites(tramiteId),
                ListaConceptosTarifaria = _facturaBusiness.ListaConceptoTarifariaConPreciosConLogicaNegocio( await _facturaBusiness.ListaConceptoTarifariaIntroduccion(await _tarifariaBusiness.ConsultarIdTarifariaVigente()), resumen[0].FallecioEnTirolesa)
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


        //Emitir factura
        [HttpPost]
        public async Task<IActionResult> EmitirFactura(ResumenIntroduccionVM viewModel)
        {
            //validar el modelo
            if (!ModelState.IsValid)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.MensajeError = "Por favor, complete todos los campos obligatorios.";
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
                    Archivo = viewModel.Decreto ? viewModel.ArchivoRecibo : null, //si es decreto, el archivo es obligatorio
                    TramiteId = viewModel.IdTramite.Value
                };

                await _facturaBusiness.VerificarDetalleFactura(dto);
            }catch(ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                return View("ResumenIntroduccion", vmCompleto);
            }catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.MensajeError = "No se pudo generar la factura: " + ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }

            var recibo = new RecibosFactura
            {
                FacturaId = viewModel.IdFactura.Value,
                Concepto = viewModel.Descripcion!.Trim(),
                Monto = viewModel.Monto.Value,
                Decreto = viewModel.Decreto,
                Contribuyente = viewModel.IdContribuyente
            };



            try
            {
                //await _introduccionBusiness.RegistrarReciboFactura(recibo, viewModel.ArchivoRecibo, mimeType, viewModel.IdTramite.Value);
                TempData["MensajeExito"] = "Recibo cargado con éxito";
                return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite } );
            }
            catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Descripcion = viewModel.Descripcion?.Trim();
                vmCompleto.Monto = viewModel.Monto;
                viewModel.MensajeError = ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }

            
        }

        //Editar recibo
        [HttpPost]
        public async Task<IActionResult> EditarRecibo(ResumenIntroduccionVM viewModel)
        {

            if (!viewModel.EsEdicion && viewModel.ArchivoRecibo == null)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
            }

            // Validar Concepto
            if (string.IsNullOrWhiteSpace(viewModel.Descripcion))
            {
                ModelState.AddModelError("Concepto", "El concepto es obligatorio.");
            }

            // Validar archivo SOLO si se sube uno nuevo
            if (viewModel.ArchivoRecibo != null && viewModel.ArchivoRecibo.Length > 0)
            {
                var extension = Path.GetExtension(viewModel.ArchivoRecibo.FileName).ToLower();
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
                    viewModel.ArchivoRecibo
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
                vmCompleto.Monto = viewModel.Monto;
                viewModel.MensajeError = ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }
            return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });
        }

        //ver Recibo archivo
        public async Task<IActionResult> VerRecibo(Guid archivoId)
        {
            var archivo = await _introduccionBusiness.ObtenerArchivo(archivoId);

            if (archivo == null || archivo.Contenido == null)
                return NotFound("Archivo no encontrado.");
            string tipo = archivo.TipoArchivo.ToLower();

            if (tipo.StartsWith("image/"))
            {
                // Convertir la imagen a PDF
                archivo.Contenido = PdfHelper.ImagenComoPdf(archivo.Contenido);
                tipo = "application/pdf";
                archivo.NombreArchivo = Path.ChangeExtension(archivo.NombreArchivo, ".pdf");
            }

            // Forzar a que el navegador intente mostrarlo
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{archivo.NombreArchivo}\"";

            return File(archivo.Contenido, tipo);
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
                vmCompleto.Monto = viewModel.Monto;
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

        // Método para buscar contribuyente (AJAX)
        [HttpPost]
        public async Task<IActionResult> BuscarContribuyente([FromBody] BuscarContribuyenteRequest request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo))
                {
                    return Json(new { success = false, message = "DNI y sexo son obligatorios" });
                }

                Persona contribuyente = await _introduccionBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);

                if (contribuyente != null)
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = new
                        {
                            id = contribuyente.IdPersona,
                            nombre = contribuyente.Nombre,
                            apellido = contribuyente.Apellido,
                            dni = request.Dni, // Usar el DNI del request para mantener consistencia
                            sexo = contribuyente.Sexo
                        }
                    });
                }
                else
                {
                    return Json(new { success = true, contribuyente = (object)null,
                        dni = request.Dni, // Devolver el DNI aunque no se encuentre el contribuyente
                        sexo = request.Sexo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Método para registrar nuevo contribuyente (AJAX)
        [HttpPost]
        public async Task<IActionResult> RegistrarContribuyente([FromBody] RegistrarContribuyenteRequest request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo) ||
                    string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Apellido))
                {
                    return Json(new { success = false, message = "Todos los campos son obligatorios" });
                }

                // Validar que no exista ya
                Persona contribuyenteExistente = await _introduccionBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);
                if (contribuyenteExistente != null)
                {
                    return Json(new { success = false, message = "El contribuyente ya existe en el sistema" });
                }

                // Crear nuevo contribuyente
                var nuevoContribuyente = new Persona
                {
                    Dni = request.Dni.ToString(),
                    Nombre = request.Nombre.Trim(),
                    Apellido = request.Apellido.Trim(),
                    Sexo = request.Sexo
                };

                // Guardar en base de datos (ajusta según tu lógica de negocio)
                var contribuyenteCreado = await _introduccionBusiness.RegistrarContribuyente(nuevoContribuyente);

                return Json(new
                {
                    success = true,
                    contribuyente = new
                    {
                        id = contribuyenteCreado.IdPersona,
                        nombre = contribuyenteCreado.Nombre,
                        apellido = contribuyenteCreado.Apellido,
                        dni = request.Dni, // Usar el DNI del request
                        sexo = contribuyenteCreado.Sexo
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // Método para buscar contribuyente de decreto
        [HttpPost]
        public async Task<IActionResult> BuscarContribuyenteDecreto([FromBody] BuscarContribuyenteRequest request)
        {
            try
            {
                // Buscar contribuyente con DNI 00000000
                Persona contribuyente = await _introduccionBusiness.BuscarContribuyente("00000000", "otro");

                if (contribuyente != null)
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = new
                        {
                            id = contribuyente.IdPersona,
                            nombre = contribuyente.Nombre,
                            apellido = contribuyente.Apellido,
                            dni = "00000000",
                            sexo = "otro"
                        }
                    });
                }

                return Json(new
                {
                    success = true,
                    contribuyente = (object)null
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Método para registrar contribuyente de decreto
        [HttpPost]
        public async Task<IActionResult> RegistrarContribuyenteDecreto([FromBody] RegistrarContribuyenteRequest request)
        {
            try
            {
                // Verificar si ya existe
                Persona contribuyenteExistente = await _introduccionBusiness.BuscarContribuyente("00000000", "otro");
                if (contribuyenteExistente != null)
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = new
                        {
                            id = contribuyenteExistente.IdPersona,
                            nombre = contribuyenteExistente.Nombre,
                            apellido = contribuyenteExistente.Apellido,
                            dni = "00000000",
                            sexo = "otro"
                        }
                    });
                }

                // Crear nuevo contribuyente de decreto
                var nuevoContribuyente = new Persona
                {
                    Dni = "00000000",
                    Nombre = "MUNICIPALIDAD",
                    Apellido = "DECRETO",
                    Sexo = "otro"
                };

                var contribuyenteCreado = await _introduccionBusiness.RegistrarContribuyente(nuevoContribuyente);

                return Json(new
                {
                    success = true,
                    contribuyente = new
                    {
                        id = contribuyenteCreado.IdPersona,
                        nombre = contribuyenteCreado.Nombre,
                        apellido = contribuyenteCreado.Apellido,
                        dni = "00000000",
                        sexo = contribuyenteCreado.Sexo
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Clase para los requests AJAX
        public class BuscarContribuyenteRequest
        {
            public int? Dni { get; set; }
            public string Sexo { get; set; }
        }

        // Clase para los requests AJAX
        public class RegistrarContribuyenteRequest
        {
            public int? Dni { get; set; }
            public string Sexo { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
        }
    }
}
