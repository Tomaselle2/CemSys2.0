using CemSys2.Business;
using CemSys2.Data;
using CemSys2.DTO.Personas;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Personas;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;


namespace CemSys2.Controllers
{
    public class PersonasController : Controller
    {
        private readonly IPersonasBusiness _personasBusiness;
        private readonly IIntroduccionBusiness _introduccionBusiness;
        private int registrosPorPagina = 15; 

        public PersonasController(IPersonasBusiness personasBusiness, IIntroduccionBusiness introduccionBusiness)
        {
            _personasBusiness = personasBusiness;
            _introduccionBusiness = introduccionBusiness;
        }

        public async Task<IActionResult> Index(PersonasVM? viewModel = null)
        {
            if(viewModel == null)
            {
                viewModel = new PersonasVM();
                await CargarCombo(viewModel);
            }
            
            await CargarCombo(viewModel);

            return View(viewModel);
        }

        private async Task CargarCombo(PersonasVM viewModel)
        {
            try
            {
                viewModel.ListaCondicionPersona = await _personasBusiness.ListaCategoriaPersonas();
                viewModel.ListaTipoParcela = await _introduccionBusiness.ListaTipoParcela();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al cargar las categorías de personas: {ex.Message}";
            }

        }

        //funcion que reciba el DNI, nombre, apellido o condicion, puede ser null todos los campos
        [HttpGet]
        public async Task<IActionResult> BuscarPersona(PersonasVM viewModel, int pagina = 1)
        {
            try
            {
                // Realizar la búsqueda con paginación
                var (personas, totalRegistros) = await _personasBusiness.ListaPersonasIndex(
                    viewModel.Dni,
                    viewModel.Nombre,
                    viewModel.Apellido,
                    viewModel.CondicionPersonaId,
                    viewModel.TipoParcelaId,
                    viewModel.SeccionId,
                    registrosPorPagina, 
                    pagina);

                // Actualizar el ViewModel con los resultados
                viewModel.ListaPersonasIndex = personas;
                viewModel.TotalRegistros = totalRegistros;
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                // Cargar el combo de categorías
                await CargarCombo(viewModel);
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = $"Error al buscar la persona: {ex.Message}";
            }

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> HistorialPersona(int personaId, Persona_Historial_VM model = null)
        {
            // Inicializa el viewModel correctamente
            Persona_Historial_VM viewModel = model ?? new Persona_Historial_VM();
            ModelState.Clear(); // Limpia errores de validación

            // Recuperar mensajes de TempData
            if (TempData.TryGetValue("MensajeError", out object mensajeError))
            {
                viewModel.MensajeError = mensajeError.ToString();
                TempData.Remove("MensajeError");
            }

            DTO_Persona_Historial personaHistorial = new DTO_Persona_Historial();
            List<DTO_Persona_Historial_Parcelas> historialParcelas = new List<DTO_Persona_Historial_Parcelas>();
            List<DTO_Persona_Historial_Tramites> historialTramites = new List<DTO_Persona_Historial_Tramites>();
            try
            {
                personaHistorial = await _personasBusiness.DatosPersonalesPersona(personaId);
                historialParcelas = await _personasBusiness.ListaHistorialParcelas(personaId);
                historialTramites = await _personasBusiness.ListaHistorialTramites(personaId);

            }
            catch(Exception ex)
            {
                viewModel.MensajeError = $"Error al obtener los datos de la persona: {ex.Message}";
            }

            viewModel.Id = personaHistorial.IdPersona;
            viewModel.Dni = (personaHistorial.Dni == "nn") ? null : int.Parse(personaHistorial.Dni);
            viewModel.Nombre = (personaHistorial.Nombre == "nn") ? null : personaHistorial.Nombre;
            viewModel.Apellido = personaHistorial.Apellido;
            viewModel.FechaDefuncion = personaHistorial.FechaDefuncion;
            viewModel.FechaNacimiento = personaHistorial.FechaNacimiento;
            viewModel.Sexo = personaHistorial.Sexo;
            viewModel.EstadoDifunto = personaHistorial.EstadoDifunto;
            viewModel.ActaDefuncion.Id = personaHistorial.ActaDefuncion.Id;
            viewModel.ActaDefuncion.Acta = personaHistorial.ActaDefuncion?.Acta;
            viewModel.ActaDefuncion.Tomo = personaHistorial.ActaDefuncion?.Tomo;
            viewModel.ActaDefuncion.Folio = personaHistorial.ActaDefuncion?.Folio;
            viewModel.ActaDefuncion.Serie = personaHistorial.ActaDefuncion?.Serie;
            viewModel.ActaDefuncion.Age = personaHistorial.ActaDefuncion?.Age;
            viewModel.InformacionAdicional = (personaHistorial.InformacionAdicional == null) ? null : personaHistorial.InformacionAdicional;
            viewModel.DomicilioEnTirolesa = personaHistorial.DomicilioEnTirolesa;
            viewModel.FallecioEnTirolesa = personaHistorial.FallecioEnTirolesa;
            viewModel.NN = (personaHistorial.Dni == "nn") ? true : false;


            viewModel.ListaHistorialParcelas = historialParcelas;
            viewModel.ListaHistorialTramites = historialTramites;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarPersona(Persona_Historial_VM viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, retornar a la vista con los errores
                TempData["MensajeError"] = "Por favor, corrija los errores en el formulario.";
                return View("HistorialPersona", viewModel);
            }

            try
            {
                Persona? difunto = new Persona();

                Persona model = await _personasBusiness.ConsultarPersona(viewModel.Id.Value);
                model.Dni = (viewModel.NN) ? "nn" : viewModel.Dni?.ToString() ?? "nn";
                model.Nombre = viewModel.Nombre?.Trim() ?? "nn";
                model.Apellido = viewModel.Apellido.Trim();
                model.FechaDefuncion = viewModel.FechaDefuncion;
                model.FechaNacimiento = viewModel.FechaNacimiento;
                model.Sexo = viewModel.Sexo;

                model.ActaDefuncionNavigation.Acta = viewModel.ActaDefuncion.Acta;
                model.ActaDefuncionNavigation.Tomo = viewModel.ActaDefuncion.Tomo;
                model.ActaDefuncionNavigation.Folio = viewModel.ActaDefuncion.Folio;
                model.ActaDefuncionNavigation.Serie = viewModel.ActaDefuncion.Serie;
                model.ActaDefuncionNavigation.Age = viewModel.ActaDefuncion.Age;

                model.InformacionAdicional = viewModel.InformacionAdicional;

                int resultado = await _personasBusiness.ModificarPersona(model);

                if (resultado == 0)
                {
                    TempData["MensajeError"]  = "No se pudo modificar. Por favor, inténtelo de nuevo.";
                    return View("HistorialPersona", viewModel);
                }

                TempData["MensajeExito"] = "Modificación exitosa";
                return RedirectToAction("HistorialPersona", new { personaId = viewModel.Id });

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al modificar la persona: {ex.Message}";
                return View("HistorialPersona", viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            var secciones = await _introduccionBusiness.ListaSecciones(tipoParcelaId);
            return Json(secciones);
        }

        //recibe una lista de personas de la vista (viewModel.ListaPersonasIndex) y manda al metodo un lista con los id de las personas
        //[HttpGet]
        //public async Task<IActionResult> ResultadosDifuntosExcel(PersonasVM viewModel)
        //{
        //    // Volvés a obtener la lista filtrada desde la base de datos
        //    try
        //    {
        //        var idsLista = await _personasBusiness.ListaIdsPersonasFiltradasParaExcel(
        //        viewModel.Dni,
        //        viewModel.Nombre,
        //        viewModel.Apellido,
        //        viewModel.CondicionPersonaId,
        //        viewModel.TipoParcelaId,
        //        viewModel.SeccionId

        //    );

        //        if (idsLista == null || !idsLista.Any())
        //        {
        //            TempData["MensajeError"] = "No se encontraron resultados para exportar.";
        //            return RedirectToAction("Index");
        //        }

        //        var excelData = await _personasBusiness.ListaDifuntosExcel(idsLista);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["MensajeError"] = $"Error al obtener los IDs de las personas: {ex.Message}";
        //        return RedirectToAction("Index");
        //    }




        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public async Task<IActionResult> ResultadosDifuntosExcel(PersonasVM viewModel)
        {
            try
            {
                var idsLista = await _personasBusiness.ListaIdsPersonasFiltradasParaExcel(
                    viewModel.Dni,
                    viewModel.Nombre,
                    viewModel.Apellido,
                    viewModel.CondicionPersonaId,
                    viewModel.TipoParcelaId,
                    viewModel.SeccionId
                );

                if (idsLista == null || !idsLista.Any())
                {
                    TempData["MensajeError"] = "No se encontraron resultados para exportar.";
                    return RedirectToAction("Index");
                }

                var excelData = await _personasBusiness.ListaDifuntosExcel(idsLista);

                // Crear el archivo Excel con ClosedXML
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Difuntos");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "DNI";
                    worksheet.Cell(1, 2).Value = "Apellido y Nombre";
                    worksheet.Cell(1, 3).Value = "Estado";
                    worksheet.Cell(1, 4).Value = "Tipo";
                    worksheet.Cell(1, 5).Value = "Sección";
                    worksheet.Cell(1, 6).Value = "Parcela";
                    worksheet.Cell(1, 7).Value = "Fecha Defunción";
                    worksheet.Cell(1, 8).Value = "Fecha Nacimiento";
                    worksheet.Cell(1, 9).Value = "Acta";
                    worksheet.Cell(1, 10).Value = "Tomo";
                    worksheet.Cell(1, 11).Value = "Folio";
                    worksheet.Cell(1, 12).Value = "Serie";
                    worksheet.Cell(1, 13).Value = "Año";
                    worksheet.Cell(1, 14).Value = "Datos adicional";

                    // Formato de encabezados
                    var headerRange = worksheet.Range(1, 1, 1, 14);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Datos
                    int row = 2;
                    foreach (var difunto in excelData)
                    {
                        string nombreCompleto = $"{difunto.Apellido} {difunto.Nombre}";
                        string parcela = GetParcelaDescription(difunto);

                        worksheet.Cell(row, 1).Value = string.IsNullOrEmpty(difunto.Dni) ? "---" : difunto.Dni;
                        worksheet.Cell(row, 2).Value = nombreCompleto;
                        worksheet.Cell(row, 3).Value = difunto.Estado ?? "---";
                        worksheet.Cell(row, 4).Value = GetTipoParcelaNombre(difunto.TipoParcela);
                        worksheet.Cell(row, 5).Value = difunto.NombreSeccion ?? "---";
                        worksheet.Cell(row, 6).Value = parcela;
                        worksheet.Cell(row, 7).Value = difunto.FechaDefuncion?.ToString("dd/MM/yyyy") ?? "---";
                        worksheet.Cell(row, 8).Value = difunto.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "---";
                        worksheet.Cell(row, 9).Value = difunto.ActaDefuncion?.Acta?.ToString() ?? "---";
                        worksheet.Cell(row, 10).Value = difunto.ActaDefuncion?.Tomo?.ToString() ?? "---";
                        worksheet.Cell(row, 11).Value = difunto.ActaDefuncion?.Folio?.ToString() ?? "---";
                        worksheet.Cell(row, 12).Value = difunto.ActaDefuncion?.Serie ?? "---";
                        worksheet.Cell(row, 13).Value = difunto.ActaDefuncion?.Age?.ToString() ?? "---";
                        worksheet.Cell(row, 14).Value = difunto.InformacionAdicional ?? "---";

                        row++;
                    }

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Preparar la respuesta
                    var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"Difuntos_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al generar el archivo Excel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private string GetParcelaDescription(DTO_Excel_Difuntos opc)
        {
            if (opc == null) return "---";

            return opc.TipoParcela switch
            {
                1 => $"Nicho {opc.NroParcela} Fila {opc.NroFila}",
                2 => $"Fosa {opc.NroParcela}",
                3 => $"Lote {opc.NroParcela}",
                _ => "---"
            };
        }

        private string GetTipoParcelaNombre(int tipoParcela)
        {
            return tipoParcela switch
            {
                1 => "Nicho",
                2 => "Fosa",
                3 => "Panteón",
                _ => "---"
            };
        }




    }
}
