using CemSys2.Business;
using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Personas;
using CemSys2.Interface.Tramite;
using CemSys2.Models;
using CemSys2.ViewModel.ConcesionesViewModel;
using CemSys2.ViewModel.ContratoViewModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace CemSys2.Controllers
{
    public class ContratoConcesionController : Controller
    {
        private readonly IConcesionesBusiness _concesionesBusiness;
        private readonly IIntroduccionBusiness _introduccionBusiness;
        private readonly IPersonasBusiness _personasBusiness;
        private readonly IPdfService _pdfService;
        private readonly ITramiteBusiness _tramiteBusiness;
        private readonly IFacturaBusiness _facturaBusiness;

        public ContratoConcesionController(IConcesionesBusiness concesionesBusiness, IIntroduccionBusiness introduccionBusiness, 
            IPersonasBusiness personasBusiness, IPdfService pdfService, ITramiteBusiness tramiteBusiness,
            IFacturaBusiness facturaBusiness)
        {
            _concesionesBusiness = concesionesBusiness;
            _introduccionBusiness = introduccionBusiness;
            _personasBusiness = personasBusiness;
            _pdfService = pdfService;
            _tramiteBusiness = tramiteBusiness;
            _facturaBusiness = facturaBusiness;
        }

        //vista principal de concesiones
        public async Task<IActionResult> Index(int pagina = 1, int tamanoPagina = 10)
        {
            IndexConcesionesVM viewModel = new IndexConcesionesVM();
            try
            {
                viewModel.ListaParcelasSinContrato = await _concesionesBusiness.ListaParcelasSinContrato();
                var resultado = await _concesionesBusiness.ListadoConcesiones(pagina, tamanoPagina);

                viewModel.ListaConcesiones = resultado.Items;
                viewModel.PaginaActual = resultado.PaginaActual;
                viewModel.TotalRegistros = resultado.TotalRegistros;
                viewModel.TamanoPagina = resultado.TamanoPagina;

                // Calcular total de páginas
                viewModel.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / resultado.TamanoPagina);

            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }

            return View(viewModel);
        }

        //vista de generar contrato concesion
        public async Task<IActionResult> ContratoConcesion(int parcelaId, string? nroConcesion) //recibe el parcelaId de las parcelas sin contrato
        {
            GenerarContratoVM viewModel = new GenerarContratoVM();

            if (!string.IsNullOrEmpty(nroConcesion)) //va a entrar cuando esta iniciado para completar
            {
                int tamiteId = await _concesionesBusiness.VerificarSiExisteContratoConcesion(nroConcesion, parcelaId);
                //se busca el tramite
                Tramite tramite = await _tramiteBusiness.ConsultarTramite(tamiteId);

                viewModel.NroConcesion = nroConcesion;
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                viewModel.TramiteId = tramite.Id;
            }

            //se ejecuta cuando no inicio un contrato de concesion
            await CargarDatosPantallaContrato(viewModel, parcelaId); //metodo privado que carga los datos en la pantalla de contrato concesion que no inicio
            return View(viewModel);
        }

        //vista para los contratos que ya se iniciaron
        [HttpGet]
        public async Task<IActionResult> ContratoIniciado(string nroConcesion, int parcelaId)
        {
            ContratoInicadoVM viewModel = new ContratoInicadoVM();
            Tramite tramite = new Tramite();
            try
            {
                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(nroConcesion, parcelaId);

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }


            if (tramite.EstadoActualId == (int)EstadosContratoConcesion.Iniciado) //quiere decir que no llego al paso pendiente de documentacion
            {
                return RedirectToAction("ContratoConcesion", new { parcelaId = parcelaId, nroConcesion = nroConcesion }); //redirigo a la vista de generar contrato
            }

            viewModel.EstadoTramiteId = tramite.EstadoActualId;
            await CargarDatosContratoYaInicado(viewModel, parcelaId, tramite.Id);

            return View(viewModel);

        }

        //carga los datos de contratos ya iniciados
        private async Task CargarDatosContratoYaInicado(ContratoInicadoVM viewModel, int parcelaId, int tramiteId)
        {
            try
            {
                //metodo que recibe el parcelaId y buscar los difuntos en esa parcela
                viewModel.DifuntosEnParcela = await _concesionesBusiness.ListaDifuntosPorParcela(parcelaId);

                //metodo que recibe el parcelaId y buscar los datos de la parcela
                DTO_Datos_Concesion datosConcesion = await _concesionesBusiness.DatosParcela(parcelaId);
                viewModel.DatosParcela = datosConcesion;

                viewModel.ParcelaId = parcelaId;
                viewModel.TramiteId = tramiteId;

                //Se busca el contrato de concesión
                CemSys2.Models.ContratoConcesion contratoConcesion = await _concesionesBusiness.ConsultarContratoConcesion(tramiteId);
                viewModel.CantidadCuotaSeleccionada = contratoConcesion.CuotaId;
                viewModel.CantidadAniosId = contratoConcesion.CantidadAnios;
                viewModel.NroConcesion = contratoConcesion.Concesion;
                viewModel.Vencimiento = contratoConcesion.Vencimiento;
                viewModel.PrecioFinal = contratoConcesion.Precio;
                viewModel.PrecioSeleccionado = contratoConcesion.PrecioTarifariaId;
                viewModel.OtraFormaPago = contratoConcesion.PagoDescripcion ?? "";
                viewModel.HistorialEstadoTramites = await _introduccionBusiness.HistorialEstadoTramites(tramiteId);
                //trae los titulares actuales del contrato
                viewModel.Titulares = await _concesionesBusiness.ListaTitularesActualesContrato(contratoConcesion.IdTramite);

                //genera la factura
                Factura factura = await _facturaBusiness.ConsultarFacturaPorTramiteId(tramiteId);
                var conceptosFactura = await _facturaBusiness.ListaConceptosFacturaPorFactura(factura.Id);
                var listaRecibosFactura = await _facturaBusiness.ListaRecibosFactura(factura.Id);

                viewModel.Factura = factura;
                viewModel.ListaConceptosFactura = conceptosFactura; //conceptos
                viewModel.ListaRecibosFactura = listaRecibosFactura; //recibos
               // viewModel.ListaArchivos = await _facturaBusiness.ListaArchivosTramiteId(tramiteId); //archivos

                viewModel.Categorias = EnumHelper.ToSelectList<CategoriaArchivosEnum>();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }
        }

        //metodo privado que carga los datos en la pantalla de contrato concesion de tramites no iniciados
        private async Task CargarDatosPantallaContrato(GenerarContratoVM viewModel, int parcelaId)
        {
            try
            {
                //metodo que recibe el parcelaId y buscar los difuntos en esa parcela
                viewModel.DifuntosEnParcela = await _concesionesBusiness.ListaDifuntosPorParcela(parcelaId);
               
                //metodo que recibe el parcelaId y buscar los datos de la parcela
                DTO_Datos_Concesion datosConcesion = await _concesionesBusiness.DatosParcela(parcelaId);
                viewModel.DatosParcela = datosConcesion;

                //si el tipo parcela es nicho o fosa
                int conceptoTarifariaId = 0;

                if (datosConcesion.TipoParcela == (int)TipoParcelaEnum.Nicho)
                {
                    conceptoTarifariaId = (int)ConceptosTarifariaEnum.ConcesionNicho;
                }

                if (datosConcesion.TipoParcela == (int)TipoParcelaEnum.Fosa)
                {
                    conceptoTarifariaId = (int)ConceptosTarifariaEnum.ConcesionFosas;
                }

                //metodo que recibe el conceptoTarifariaId, seccionId y nroFila para buscar los precios de concesion
                List<DTO_Precios_Concesion> precios = await _concesionesBusiness.PreciosConcesion(conceptoTarifariaId, datosConcesion.SeccionId, datosConcesion.NroFila);
                viewModel.PreciosConcesion = precios;

                viewModel.ParcelaId = parcelaId;

                //metodo que busca las cantidades de cuotas
                List<CantidadCuota> cuotas = await _concesionesBusiness.CantidadCuotas();
                List<DTO_Cuotas> dtoCuota = cuotas.Select(c => new DTO_Cuotas
                {
                    Id = c.Id,
                    Texto = c.Cuota == 1 ? "1 pago" : $"{c.Cuota} cuotas"
                }).ToList();

                viewModel.CantidadCuotas = dtoCuota;

            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }
        }

        //metodo de genera el contrato concesion en formato pdf
        public async Task<IActionResult> GenerarContratoConcesionPDF(GenerarContratoVM viewModel)
        {

            //valido el modelo
            if (!ModelState.IsValid)
            {
                await CargarDatosPantallaContrato(viewModel, viewModel.ParcelaId ?? 0);
                //mensaje de error
                viewModel.MensajeError = "Por favor, complete todos los campos obligatorios.";
                return View("ContratoConcesion", viewModel);
            }

            int? usuarioId = HttpContext.Session.GetInt32("idUsuario");
            DTO_DatosGenerarContratoConcesion dtoDatosGenerarConcesion = new();

            try
            {
                dtoDatosGenerarConcesion = await _concesionesBusiness.GenerarContrato(viewModel, usuarioId);

            }catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarDatosPantallaContrato(viewModel, viewModel.ParcelaId ?? 0);
                return View("ContratoConcesion", viewModel);
            }catch (Exception)
            {
                viewModel.MensajeError = "No se pudo generar el contrato";
                return View("ContratoConcesion", viewModel);
            }

            if (!dtoDatosGenerarConcesion.contratoGenerado) //No se genero el contrato
            {
                await CargarDatosPantallaContrato(viewModel, viewModel.ParcelaId ?? 0);
                viewModel.MensajeError = "No se pudo generar el contrato";
                return View("ContratoConcesion", viewModel);
            }
   
            // Preparar el ViewModel para la vista del contrato en PDF -----------------------------------------------------------------------------------------
            ContratoPDF_VM contratoPDF_VM = new ContratoPDF_VM();
            contratoPDF_VM.datosContrato = dtoDatosGenerarConcesion;
            contratoPDF_VM.baseUrl = $"{Request.Scheme}://{Request.Host}";
            contratoPDF_VM.PrecioEnLetras = NumeroALetras.ConvertirALetras(dtoDatosGenerarConcesion.Precio);

            //si es nicho voy a vista de contrato nicho sino contrato fosa
            string nombreVistaContrato = "";
            switch (viewModel.tipoParcela)
            {
                case (int)TipoParcelaEnum.Nicho:
                    nombreVistaContrato = TipoParcelaEnum.Nicho.ToString();
                    break;
                case (int)TipoParcelaEnum.Fosa:
                    nombreVistaContrato = TipoParcelaEnum.Fosa.ToString();
                    break;
            }

            try
            {
                // Generar PDF con Puppeteer
                var pdfBytes = await _pdfService.GeneratePdfAsync(nombreVistaContrato, contratoPDF_VM, HttpContext);

                // se abre en el visor del navegador
                return File(pdfBytes, "application/pdf");

                // Si quieres forzar descarga:
                // return File(pdfBytes, "application/pdf", "contrato.pdf");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return BadRequest($"Error generando PDF: {ex.Message}");
            }
        }

        //metodo que recibe el parcelaId y nroConcesion para pasar a subir Contrato de concesion
        [HttpPost]
        public async Task<IActionResult> PendienteDocumentacion(GenerarContratoVM viewModel)
        {
            //si existe el nroTramite es > 0
            int nroTramite = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion, viewModel.ParcelaId ?? 0);
            if(nroTramite > 0)
            {
                //Se busca el contrato de concesión
                CemSys2.Models.ContratoConcesion contratoConcesion = await _concesionesBusiness.ConsultarContratoConcesion(nroTramite);

                //verifico el tipo de concepto
                int tipoConceptoTarifaria = 0;
                switch (contratoConcesion.TipoParcela)
                {
                    case 1: //nicho
                        tipoConceptoTarifaria = (int)TipoConceptoTarifariaEnum.ConcesionNicho;
                        break;
                    case 2: //fosa
                        tipoConceptoTarifaria = (int)TipoConceptoTarifariaEnum.ConcesionFosa;
                        break;
                }

                //metodo que carga los datos del paso Pendiente de documentacion
                bool exito = await _concesionesBusiness.PasoPendienteDocumentacion(contratoConcesion, viewModel.Titulares, tipoConceptoTarifaria);
            }
            else //el nro de conces en incorrecto, no existe par
            {
                await CargarDatosPantallaContrato(viewModel, viewModel.ParcelaId ?? 0);
                viewModel.MensajeError = "El número de concesión es incorrecto";
                return View("ContratoConcesion", viewModel);
            }

            return RedirectToAction("ContratoIniciado", new {nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
        }

        //cargar el recibo
        [HttpPost]
        public async Task<IActionResult> CargarRecibo(ContratoInicadoVM viewModel)
        {
            // Desactivar validación automática para Factura
            ModelState.Remove("Factura.Tramite");
            ModelState.Remove("Categoria");

            // Primero validar el archivo específicamente
            if (viewModel.ArchivoRecibo == null || viewModel.ArchivoRecibo.Length == 0)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
                Tramite tramite = new Tramite();
                try
                {
                    tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                    //se busca el tramite
                    tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                    viewModel.EstadoTramiteId = tramite.EstadoActualId;
                    await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                    viewModel.Concepto = viewModel.Concepto?.Trim();
                    viewModel.Monto = viewModel.Monto;
                    viewModel.MensajeError = "Debe seleccionar un archivo";
                }
                catch(Exception ex)
                {
                    viewModel.MensajeError = ex.Message;
                }
                
                return View("ContratoIniciado", viewModel);
            }

            // Validar extensión
            var extension = Path.GetExtension(viewModel.ArchivoRecibo.FileName).ToLower();
            var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
            if (!permitidas.Contains(extension))
            {
                ModelState.AddModelError("ArchivoRecibo", "Solo se permiten archivos PNG, JPG o PDF.");
                Tramite tramite = new Tramite();
                try
                {
                    //se busca el tramite
                    tramite = await _tramiteBusiness.ConsultarTramite(viewModel.TramiteId ?? 0);
                    viewModel.EstadoTramiteId = tramite.EstadoActualId;
                    await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                    viewModel.Concepto = viewModel.Concepto?.Trim();
                    viewModel.Monto = viewModel.Monto;
                    viewModel.MensajeError = "Solo se permiten archivos PNG, JPG o PDF";
                }
                catch (Exception ex)
                {
                    viewModel.MensajeError = ex.Message;
                }

                return View("ContratoIniciado", viewModel);
            }

            if(viewModel.Monto != 0 && viewModel.Monto > viewModel.DatosParcela.Pendiente)
            {
                ModelState.AddModelError("Monto", $"El monto no puede ser superior a $ {viewModel.DatosParcela.Pendiente}");
                Tramite tramite = await _tramiteBusiness.ConsultarTramite(viewModel.TramiteId ?? 0);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                viewModel.MensajeError = $"El monto no puede ser superior a $ {viewModel.DatosParcela.Pendiente}";
                return View("ContratoIniciado", viewModel);
            }

            // Luego validar el modelo completo
            if (!ModelState.IsValid)
            {
                Tramite tramite = new Tramite();
                try
                {
                    tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                    //se busca el tramite
                    tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                    viewModel.EstadoTramiteId = tramite.EstadoActualId;
                    await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                    viewModel.Concepto = viewModel.Concepto?.Trim();
                    viewModel.Monto = viewModel.Monto;
                    viewModel.MensajeError = "Revice los campos obligatorios";
                }
                catch (Exception ex)
                {
                    viewModel.MensajeError = ex.Message;
                }

                return View("ContratoIniciado", viewModel);
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
                Concepto = viewModel.Concepto!.Trim(),
                Monto = viewModel.Monto.Value,
                Decreto = viewModel.Decreto,
                Contribuyente = viewModel.IdContribuyente
            };



            try
            {
                await _facturaBusiness.RegistrarReciboFactura(recibo, viewModel.ArchivoRecibo, mimeType, viewModel.TramiteId.Value);
                TempData["MensajeExito"] = "Recibo cargado con éxito";
                return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
            }
            catch (Exception ex)
            {
                Tramite tramite = new Tramite();
                
                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                viewModel.Concepto = viewModel.Concepto?.Trim();
                viewModel.Monto = viewModel.Monto;
                viewModel.MensajeError = ex.Message;
                
                return View("ContratoIniciado", viewModel);
            }
        }


        //cargar Archivo
        [HttpPost]
        public async Task<IActionResult> SubirArchivo(ContratoInicadoVM viewModel)
        {
            // Desactivar validación automática para Factura
            ModelState.Remove("Factura.Tramite");

            // Primero validar el archivo específicamente
            if (viewModel.ArchivoRecibo == null || viewModel.ArchivoRecibo.Length == 0)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
                Tramite tramite = new Tramite();
                try
                {
                    tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                    //se busca el tramite
                    tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                    viewModel.EstadoTramiteId = tramite.EstadoActualId;
                    await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                    viewModel.Concepto = viewModel.Concepto?.Trim();
                    viewModel.Monto = viewModel.Monto;
                    viewModel.MensajeError = "Debe seleccionar un archivo";
                }
                catch (Exception ex)
                {
                    viewModel.MensajeError = ex.Message;
                }

                return View("ContratoIniciado", viewModel);
            }

            // Validar extensión
            var extension = Path.GetExtension(viewModel.ArchivoRecibo.FileName).ToLower();
            var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
            if (!permitidas.Contains(extension))
            {
                ModelState.AddModelError("ArchivoRecibo", "Solo se permiten archivos PNG, JPG o PDF.");
                Tramite tramite = new Tramite();
                try
                {
                    tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                    //se busca el tramite
                    tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                    viewModel.EstadoTramiteId = tramite.EstadoActualId;
                    await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                    viewModel.Concepto = viewModel.Concepto?.Trim();
                    viewModel.Monto = viewModel.Monto;
                    viewModel.MensajeError = "Solo se permiten archivos PNG, JPG o PDF";
                }
                catch (Exception ex)
                {
                    viewModel.MensajeError = ex.Message;
                }

                return View("ContratoIniciado", viewModel);
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


            try
            {
                //await _facturaBusiness.RegistrarArchivo(viewModel.ArchivoRecibo, mimeType, viewModel.TramiteId.Value, viewModel.Categoria, viewModel.Concepto!);
                TempData["MensajeExito"] = $"Archivo {viewModel.Categoria} cargado con éxito";
                return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
            }
            catch (Exception ex)
            {
                Tramite tramite = new Tramite();

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                viewModel.Concepto = viewModel.Concepto?.Trim();
                viewModel.Monto = viewModel.Monto;

                viewModel.MensajeError = ex.Message;

                return View("ContratoIniciado", viewModel);
            }
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

                Persona contribuyente = await _personasBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);

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
                            sexo = contribuyente.Sexo,
                            celular = contribuyente.Celular,
                            correo = contribuyente.Correo,
                            domicilio = contribuyente.Domicilio,

                        }
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = (object)null,
                        dni = request.Dni, // Devolver el DNI aunque no se encuentre el contribuyente
                        sexo = request.Sexo,

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
                    string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Apellido) || string.IsNullOrEmpty(request.Domicilio))
                {
                    return Json(new { success = false, message = "Todos los campos son obligatorios" });
                }

                // Validar que no exista ya
                Persona contribuyenteExistente = await _personasBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);
                if (contribuyenteExistente != null)
                {
                    return Json(new { success = false, message = "El titular ya existe en el sistema" });
                }

                // Crear nuevo titular
                var nuevoTitular = new Persona
                {
                    Dni = request.Dni.ToString(),
                    Nombre = request.Nombre.Trim(),
                    Apellido = request.Apellido.Trim(),
                    Sexo = request.Sexo,
                    Celular = request.Celular?.Trim(),
                    Correo = request.Correo?.Trim(),
                    Domicilio = request.Domicilio.Trim(),
                };

                // Guardar en base de datos
                var TitularCreado = await _concesionesBusiness.RegistrarTitular(nuevoTitular);

                return Json(new
                {
                    success = true,
                    contribuyente = new
                    {
                        id = TitularCreado.IdPersona,
                        nombre = TitularCreado.Nombre,
                        apellido = TitularCreado.Apellido,
                        dni = request.Dni, // Usar el DNI del request
                        sexo = TitularCreado.Sexo,
                        celular = TitularCreado.Celular,
                        correo = TitularCreado.Correo,
                        domicilio = TitularCreado.Domicilio
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // Método para registrar nuevo contribuyente (AJAX)
        [HttpPost]
        public async Task<IActionResult> RegistrarContribuyenteParaRecibo([FromBody] RegistrarContribuyenteRequest request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo) ||
                    string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Apellido))
                {
                    return Json(new { success = false, message = "Todos los campos son obligatorios" });
                }

                // Validar que no exista ya
                Persona contribuyenteExistente = await _personasBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);
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
                var contribuyenteCreado = await _personasBusiness.RegistrarContribuyente(nuevoContribuyente);

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

        //Editar recibo
        [HttpPost]
        public async Task<IActionResult> EditarRecibo(ContratoInicadoVM viewModel)
        {
            ModelState.Remove("Categoria");
            if (!viewModel.EsEdicion && viewModel.ArchivoRecibo == null)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
            }

            // Validar Concepto
            if (string.IsNullOrWhiteSpace(viewModel.Concepto))
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

            try
            {
                await _introduccionBusiness.EditarReciboFactura(
                    viewModel.IdRecibo.Value,
                    viewModel.Concepto!.Trim(),
                    viewModel.ArchivoRecibo
                );

                TempData["MensajeExito"] = "Recibo editado con éxito";
                return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
            }
            catch (Exception ex)
            {
                Tramite tramite = new Tramite();

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                viewModel.Concepto = viewModel.Concepto?.Trim();
                viewModel.Monto = viewModel.Monto;

                viewModel.MensajeError = ex.Message;

                return View("ContratoIniciado", viewModel);
            }
        }


        //Editar Archivo
        [HttpPost]
        public async Task<IActionResult> EditarArchivo(ContratoInicadoVM viewModel)
        {
            if (!viewModel.EsEdicion && viewModel.ArchivoRecibo == null)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
            }

            // Validar Concepto
            if (string.IsNullOrWhiteSpace(viewModel.Concepto))
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

            try
            {
                //await _facturaBusiness.EditarArchivo(
                //    viewModel.IdArchivo.Value,
                //    viewModel.Concepto!.Trim(),
                //    viewModel.Categoria,                    
                //    viewModel.ArchivoRecibo
                //);

                TempData["MensajeExito"] = $"Archivo {viewModel.Categoria} editado con éxito";
                return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
            }
            catch (Exception ex)
            {
                Tramite tramite = new Tramite();

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                viewModel.Concepto = viewModel.Concepto?.Trim();
                viewModel.Monto = viewModel.Monto;

                viewModel.MensajeError = ex.Message;

                return View("ContratoIniciado", viewModel);
            }
        }


        //Finaliza el paso pendiente de documentacion
        [HttpPost]
        public async Task<IActionResult> FinalizarPasoPendieteDocumentacion(ContratoInicadoVM viewModel)
        {
            bool pendienteFinalizado = false;
            try
            {
                pendienteFinalizado = await _concesionesBusiness.VerificarArchivoContratoSubido(viewModel.TramiteId.Value);
            }
            catch (Exception ex)
            {
                Tramite tramite = new Tramite();

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);

                viewModel.MensajeError = ex.Message;

                return View("ContratoIniciado", viewModel);
            }

            if (!pendienteFinalizado)
            {
                Tramite tramite = new Tramite();
                viewModel.EstadoTramiteId = tramite.EstadoActualId;

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);

                //regreso mensaje que falta subir contrato de concesion
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;

                viewModel.MensajeError = "Falta subir el contrato de concesión";

                return View("ContratoIniciado", viewModel);
            }


            //paso al paso de "activa"
            try
            {
                await _concesionesBusiness.FinalizarPendienteDocumentacion(viewModel.TramiteId.Value);
                TempData["MensajeExito"] = "Pendiente de documentación finalizado exitosamente";
                return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
            }
            catch (Exception ex)
            {
                Tramite tramite = new Tramite();

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;

                //se busca el tramite
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);
                viewModel.EstadoTramiteId = tramite.EstadoActualId;
                await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId ?? 0, tramite.Id);

                viewModel.MensajeError = ex.Message;

                return View("ContratoIniciado", viewModel);
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
            public string Domicilio { get; set; }
            public string? Celular { get; set; }
            public string? Correo { get; set; }
        }


    }
}
