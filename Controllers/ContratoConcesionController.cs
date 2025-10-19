using CemSys2.Business;
using CemSys2.Data;
using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Archivos;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Historiales;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Personas;
using CemSys2.Interface.Tarifaria;
using CemSys2.Interface.Tramite;
using CemSys2.Models;
using CemSys2.ViewModel;
using CemSys2.ViewModel.ConcesionesViewModel;
using CemSys2.ViewModel.ContratoViewModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace CemSys2.Controllers
{
    public class ContratoConcesionController : Controller
    {
        private readonly IConcesionesBusiness _concesionesBusiness;
        private readonly IPersonasBusiness _personasBusiness;
        private readonly IPdfService _pdfService;
        private readonly ITramiteBusiness _tramiteBusiness;
        private readonly IFacturaBusiness _facturaBusiness;
        private readonly ITarifariaBusiness _tarifariaBusiness;
        private readonly IArchivoBusiness _archivoBusiness;
        private readonly IHistorialesBusiness _historialesBusiness;

        public ContratoConcesionController(IConcesionesBusiness concesionesBusiness, IArchivoBusiness archivoBusiness, 
            IPersonasBusiness personasBusiness, IPdfService pdfService, ITramiteBusiness tramiteBusiness,
            IFacturaBusiness facturaBusiness, ITarifariaBusiness tarifariaBusiness, IHistorialesBusiness historialesBusiness)
        {
            _concesionesBusiness = concesionesBusiness;
            _personasBusiness = personasBusiness;
            _pdfService = pdfService;
            _tramiteBusiness = tramiteBusiness;
            _facturaBusiness = facturaBusiness;
            _tarifariaBusiness = tarifariaBusiness;
            _archivoBusiness = archivoBusiness;
            _historialesBusiness = historialesBusiness;
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
        private async Task<ContratoInicadoVM> CargarDatosContratoYaInicado(ContratoInicadoVM viewModel, int parcelaId, int tramiteId)
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
                viewModel.HistorialEstadoTramites = await _historialesBusiness.HistorialEstadoTramites(tramiteId);
                viewModel.Pendiente = contratoConcesion.Pendiente;
                //trae los titulares actuales del contrato
                viewModel.Titulares = await _concesionesBusiness.ListaTitularesActualesContrato(contratoConcesion.IdTramite);

                //genera la factura
                var facturaInterna = await _facturaBusiness.ConsultarFacturaInternaPorTramiteId(tramiteId);
               
                viewModel.FacturaInterna = facturaInterna;
                viewModel.ListaConceptosFactura = await _facturaBusiness.ListaConceptosFacturaInternaPorFactura(facturaInterna.Id); //conceptos
                viewModel.MontoMinimoFondo = await _tarifariaBusiness.ConsultarMontoMinimoFondoActual();
                viewModel.PorcentajeFondo = await _tarifariaBusiness.ConsultarPorcentajeFondoActual();
                viewModel.ListaArchivos = await _archivoBusiness.ListaArchivosTramiteId(tramiteId); //archivos
                viewModel.TramiteId = tramiteId;
                viewModel.IdFactura = facturaInterna.Id;
                viewModel.Categorias = EnumHelper.ToSelectList<CategoriaArchivosEnum>();
                viewModel.ListaConceptosTarifaria = _facturaBusiness.ListaConceptoTarifariaConPreciosConLogicaNegocio(await _facturaBusiness.ListaConceptoTarifariaIntroduccion(await _tarifariaBusiness.ConsultarIdTarifariaVigente()), true, true);
                viewModel.ListaFacturas = await _facturaBusiness.ListaFacturasPorTramiteId(tramiteId);

            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }

            return viewModel;
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

                //metodo que pasa al VM el nro de concesion
                viewModel.NroConcesion = await _concesionesBusiness.UltimoNumeroContratoConcesion();
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
                int conceptoTarifariaId = 0;
                switch (contratoConcesion.TipoParcela)
                {
                    case 1: //nicho
                        tipoConceptoTarifaria = (int)TipoConceptoTarifariaEnum.ConcesionNicho;
                        conceptoTarifariaId = (int)ConceptosTarifariaEnum.ConcesionNicho;
                        break;
                    case 2: //fosa
                        tipoConceptoTarifaria = (int)TipoConceptoTarifariaEnum.ConcesionFosa;
                        conceptoTarifariaId = (int)ConceptosTarifariaEnum.ConcesionFosas;
                        break;
                }

                if (contratoConcesion.CuotaId != null)
                {
                    decimal precioFinalContrato = contratoConcesion.Precio;
                    int cantidadCuotas = contratoConcesion.CuotaId.Value;
                    decimal valorCuota = Math.Round(precioFinalContrato / cantidadCuotas, 2, MidpointRounding.AwayFromZero);
                    decimal porcentajeFondo = await _tarifariaBusiness.ConsultarPorcentajeFondoActual();



                    for (int i = 0; i < cantidadCuotas; i++)
                    {
                        //crea el dto de la factura
                        DTO_Factura dtoFactura = new DTO_Factura
                        {
                            TramiteId = contratoConcesion.IdTramite,
                            ContribuyenteId = viewModel.Titulares[0].Id,
                            Total = valorCuota,
                            Visibilidad = true,
                            TipoTramiteId = (int)TipotamiteEmun.ContratoDeConcesion,
                            UsuarioEmiteId = HttpContext.Session.GetInt32("idUsuario"),
                            Descripcion = $"{i+1}° CUOTA CONTRATO CONCESIÓN NRO {contratoConcesion.Concesion}",
                            EstadoId = (int)EstadosFactura.Creado
                        };

                        DTO_DetalleFactura dtoPrecioSinFondo = new DTO_DetalleFactura
                        {
                            ConceptoTarifariaId = conceptoTarifariaId,
                            PrecioUnitario = valorCuota / (1 + porcentajeFondo),
                            Cantidad = 1,
                            TipoConceptoFacturaId = tipoConceptoTarifaria
                        };

                        List<DTO_DetalleFactura> listaDetalleFactura = new List<DTO_DetalleFactura>();
                        listaDetalleFactura.Add(dtoPrecioSinFondo);

                        int idFactura = await _facturaBusiness.CrearFactura(dtoFactura, listaDetalleFactura, (i+1));
                    }
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


        //cargar Archivo
        [HttpPost]
        public async Task<IActionResult> SubirArchivo(ContratoInicadoVM viewModel)
        {
            // Desactivar validación automática para Factura
            ModelState.Remove("Factura.Tramite");

            // Primero validar el archivo específicamente
            if (viewModel.ArchivoDecreto == null || viewModel.ArchivoDecreto.Length == 0)
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
            var extension = Path.GetExtension(viewModel.ArchivoDecreto.FileName).ToLower();
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
                await _archivoBusiness.RegistrarArchivo(viewModel.ArchivoDecreto, mimeType, viewModel.TramiteId.Value, viewModel.Categoria, viewModel.Concepto.ToString());
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






        //Editar Archivo
        [HttpPost]
        public async Task<IActionResult> EditarArchivo(ContratoInicadoVM viewModel)
        {
            if (!viewModel.EsEdicion && viewModel.ArchivoDecreto == null)
            {
                ModelState.AddModelError("ArchivoRecibo", "Debe seleccionar un archivo.");
            }

            // Validar Concepto
            if (string.IsNullOrWhiteSpace(viewModel.Concepto))
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

            try
            {
                await _archivoBusiness.EditarArchivo(
                    viewModel.IdArchivo.Value,
                    viewModel.Concepto!.Trim(),
                    viewModel.Categoria,
                    viewModel.ArchivoDecreto
                );

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
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);

                vmCompleto.MensajeError = ex.Message;
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);

                return View("ContratoIniciado", vmCompleto);
            }

            if (!pendienteFinalizado)
            {
                Tramite tramite = new Tramite();
                viewModel.EstadoTramiteId = tramite.EstadoActualId;

                tramite.Id = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion!, viewModel.ParcelaId ?? 0);
                tramite = await _tramiteBusiness.ConsultarTramite(tramite.Id);

                //regreso mensaje que falta subir contrato de concesion
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                vmCompleto.EstadoTramiteId = tramite.EstadoActualId;

                vmCompleto.MensajeError = "Falta subir el contrato de concesión";
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);

                return View("ContratoIniciado", vmCompleto);
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
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = ex.Message;

                return View("ContratoIniciado", vmCompleto);
            }
        }

       


        private void CargarDatosContribuyenteyFactura(ContratoInicadoVM vmNuevo, ContratoInicadoVM vmAnterior)
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
        public async Task<IActionResult> EmitirFactura(ContratoInicadoVM viewModel)
        {
            int facturaExito = 0;
            //validar el modelo
            if (!ModelState.IsValid)
            {
               var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                viewModel.MensajeError = "Por favor, complete todos los campos obligatorios.";
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                return View("ContratoIniciado", vmCompleto);
            }

            try
            {
                DTO_VerificarDetalleFactura dto = new DTO_VerificarDetalleFactura
                {
                    Contribuyente = viewModel.IdContribuyente,
                    DetallesFactura = viewModel.ListaDetalleFactura,
                    Pendiente = viewModel.Pendiente.Value,
                    Decreto = viewModel.Decreto,
                    Archivo = viewModel.Decreto ? viewModel.ArchivoDecreto : null, //si es decreto, el archivo es obligatorio
                    TramiteId = viewModel.TramiteId.Value,
                    MontoDecreto = viewModel.MontoDecreto,
                    Descripcion = viewModel.Descripcion?.Trim() ?? string.Empty,
                    EstadoFacturaId = (int)EstadosFactura.Creado, //cambiar dependiento del caso,
                    UsuarioEmiteId = HttpContext.Session.GetInt32("idUsuario")
                };

                facturaExito = await _facturaBusiness.VerificarDetalleFactura(dto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                return View("ContratoIniciado", vmCompleto);
            }
            catch (Exception ex)
            {
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo generar la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }

            if (facturaExito > 0 && !viewModel.Decreto)
            {
                await _facturaBusiness.PasarFacturaEstadoEmitir(facturaExito);
                TempData["MensajeExito"] = "Factura emitida con éxito";
            }

            if (facturaExito > 0 && viewModel.Decreto)
            {
                TempData["MensajeExito"] = "Decreto emitido con éxito";
            }

            return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });

        }


        //pasar factura a estado emitir en caso de cuotas creadas automaticamente
        [HttpGet]
        public async Task<IActionResult> PasarFacturaEstadoEmitido(ContratoInicadoVM viewModel)
        {
            try
            {
                await _facturaBusiness.PasarFacturaEstadoEmitir(viewModel.IdFactura.Value);
                TempData["MensajeExito"] = "Factura emitida con éxito";

            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo emitir la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo emitir la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }
            catch (Exception ex)
            {
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo emitir la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }
            return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
        }

        //Anular factura
        [HttpPost]
        public async Task<IActionResult> AnularFactura(ContratoInicadoVM viewModel)
        {
            try
            {
                await _facturaBusiness.PasarFacturaEstadoAnulado(viewModel.IdFactura.Value, viewModel.MotivoAnulacion);
                TempData["MensajeExito"] = "Factura anulada con éxito";

            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo generar la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo generar la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }
            catch (Exception ex)
            {
                var vmCompleto = await CargarDatosContratoYaInicado(viewModel, viewModel.ParcelaId.Value, viewModel.TramiteId.Value);
                CargarDatosContribuyenteyFactura(vmCompleto, viewModel);
                vmCompleto.MensajeError = "No se pudo generar la factura: " + ex.Message;
                return View("ContratoIniciado", vmCompleto);
            }
            return RedirectToAction("ContratoIniciado", new { nroConcesion = viewModel.NroConcesion, parcelaId = viewModel.ParcelaId });
        }



    }
}
