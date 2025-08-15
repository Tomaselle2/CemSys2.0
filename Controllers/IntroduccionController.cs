using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using CemSys2.Interface.Introduccion;
using System.Threading.Tasks;
using CemSys2.Models;
using CemSys2.ViewModel.Reportes;
using CemSys2.DTO.Reportes;
using Rotativa.AspNetCore;
using Microsoft.Extensions.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CemSys2.Controllers
{
    public class IntroduccionController : Controller
    {
        private readonly IIntroduccionBusiness _introduccionBusiness;

        public IntroduccionController(IIntroduccionBusiness introduccionBusiness)
        {
            _introduccionBusiness = introduccionBusiness;
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


                tramiteId = await _introduccionBusiness.RegistrarIntroduccionCompleta(actaDefuncion, difuntoNuevo, viewModel.EmpleadoID.Value, viewModel.EmpresaFunebreID.Value,
                    viewModel.ParcelaID.Value, viewModel.FechaHoraIngreso.Value, placa);
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

        //pantalla de resumen
        [HttpGet]
        public async Task<IActionResult> ResumenIntroduccion(int tramiteId)
        {
            try
            {
                var resumen = await _introduccionBusiness.ObtenerResumenIntroduccion(tramiteId);
                Factura factura = await _introduccionBusiness.ConsultarFacturaPorTramiteId(tramiteId);
                var conceptosFactura = await _introduccionBusiness.ListaConceptosFacturaPorFactura(factura.Id);
                var listaRecibosFactura = await _introduccionBusiness.ListaRecibosFactura(factura.Id);
                if (resumen == null || resumen.Count == 0)
                {
                    return NotFound("No se encontraron datos para el trámite especificado.");
                }

                var viewModel = new ResumenIntroduccionVM
                {
                    ResumenIntroduccion = resumen,
                    Factura = factura,
                    ListaConceptosFactura = conceptosFactura,
                    ListaRecibosFactura = listaRecibosFactura,
                    infoAdicional = resumen.FirstOrDefault()?.informacionAdicionalTramite
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                var viewModel = new ResumenIntroduccionVM
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

        //recontruye el ViewModel de ResumenIntroduccion cuando hay un error de validacion
        private async Task<ResumenIntroduccionVM> ReconstruirViewModel(int tramiteId)
        {
            var resumen = await _introduccionBusiness.ObtenerResumenIntroduccion(tramiteId);
            Factura factura = await _introduccionBusiness.ConsultarFacturaPorTramiteId(tramiteId);
            var conceptosFactura = await _introduccionBusiness.ListaConceptosFacturaPorFactura(factura.Id);
            var listaRecibosFactura = await _introduccionBusiness.ListaRecibosFactura(factura.Id);

            return new ResumenIntroduccionVM
            {
                ResumenIntroduccion = resumen,
                Factura = factura,
                ListaConceptosFactura = conceptosFactura,
                ListaRecibosFactura = listaRecibosFactura,
                IdTramite = tramiteId,
                IdFactura = factura.Id,
                infoAdicional = resumen.FirstOrDefault()?.informacionAdicionalTramite
            };
        }

        //cargar el recibo
        [HttpPost]
        public async Task<IActionResult> CargarRecibo(ResumenIntroduccionVM viewModel)
        {
            // Desactivar validación automática para Factura
            ModelState.Remove("Factura.Tramite");

            // Primero validar el archivo específicamente

            if (viewModel.ArchivoRecibo == null || viewModel.ArchivoRecibo.Length == 0)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Concepto = viewModel.Concepto;
                vmCompleto.Monto = viewModel.Monto;
                return View("ResumenIntroduccion", vmCompleto);
            }

            // Validar extensión
            var extension = Path.GetExtension(viewModel.ArchivoRecibo.FileName).ToLower();
            var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
            if (!permitidas.Contains(extension))
            {
                ModelState.AddModelError("ArchivoRecibo", "Solo se permiten archivos PNG, JPG o PDF.");
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Concepto = viewModel.Concepto;
                vmCompleto.Monto = viewModel.Monto;
                return View("ResumenIntroduccion", vmCompleto);
            }

            // Luego validar el modelo completo
            if (!ModelState.IsValid)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Concepto = viewModel.Concepto;
                vmCompleto.Monto = viewModel.Monto;
                return View("ResumenIntroduccion", vmCompleto);
            }

            // Mapear el tipo MIME
            string mimeType = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            var recibo = new RecibosFactura
            {
                FacturaId = viewModel.IdFactura.Value,
                Concepto = viewModel.Concepto!,
                Monto = viewModel.Monto.Value,
                Decreto = viewModel.Decreto
            };



            try
            {
                await _introduccionBusiness.RegistrarReciboFactura(recibo, viewModel.ArchivoRecibo, mimeType, viewModel.IdTramite.Value);
                TempData["MensajeExito"] = "Recibo cargado con éxito";
                return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite } );
            }
            catch (Exception ex)
            {
                var vmCompleto = await ReconstruirViewModel(viewModel.IdTramite.Value);
                vmCompleto.Concepto = viewModel.Concepto;
                vmCompleto.Monto = viewModel.Monto;
                viewModel.MensajeError = ex.Message;
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
                vmCompleto.Concepto = viewModel.Concepto;
                vmCompleto.Monto = viewModel.Monto;
                viewModel.MensajeError = ex.Message;
                return View("ResumenIntroduccion", vmCompleto);
            }
            return RedirectToAction("ResumenIntroduccion", new { tramiteId = viewModel.IdTramite });
        }

        //finaliza el tramite
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
                vmCompleto.Concepto = viewModel.Concepto;
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
    }
}
