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
            const int registrosPorPagina = 5;

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


                int introduccion = await _introduccionBusiness.RegistrarIntroduccionCompleta(actaDefuncion, difuntoNuevo, viewModel.EmpleadoID.Value, viewModel.EmpresaFunebreID.Value, viewModel.ParcelaID.Value, viewModel.FechaHoraIngreso.Value);
                if (introduccion == 0)
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


            return RedirectToAction("Index");
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

                var fechaMinima = fechasIngreso.Any() ? fechasIngreso.Min().ToString("dd-MM-yyyy") : null;
                var fechaMaxima = fechasIngreso.Any() ? fechasIngreso.Max().ToString("dd-MM-yyyy") : null;

                return Json(new
                {
                    success = true,
                    dataBarra= datosPorMes,    // Para el gráfico de barras
                    dataTorta= datosPorTipo,   // Para el gráfico de torta
                    fechaDesde = fechaMinima,
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
