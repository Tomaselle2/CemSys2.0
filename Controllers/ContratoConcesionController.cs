using CemSys2.Business;
using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Personas;
using CemSys2.Interface.Tramite;
using CemSys2.Models;
using CemSys2.ViewModel.ConcesionesViewModel;
using CemSys2.ViewModel.ContratoViewModel;
using Microsoft.AspNetCore.Mvc;


namespace CemSys2.Controllers
{
    public class ContratoConcesionController : Controller
    {
        private readonly IConcesionesBusiness _concesionesBusiness;
        private readonly IIntroduccionBusiness _introduccionBusiness;
        private readonly IPersonasBusiness _personasBusiness;
        private readonly IPdfService _pdfService;
        private readonly ITramiteBusiness _tramiteBusiness;

        public ContratoConcesionController(IConcesionesBusiness concesionesBusiness, IIntroduccionBusiness introduccionBusiness, IPersonasBusiness personasBusiness, IPdfService pdfService, ITramiteBusiness tramiteBusiness)
        {
            _concesionesBusiness = concesionesBusiness;
            _introduccionBusiness = introduccionBusiness;
            _personasBusiness = personasBusiness;
            _pdfService = pdfService;
            _tramiteBusiness = tramiteBusiness;
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
        public async Task<IActionResult> ContratoConcesion(int parcelaId) //recibe el parcelaId de las parcelas sin contrato
        {
            GenerarContratoVM viewModel = new GenerarContratoVM();
            await CargarDatosPantallaContrato(viewModel, parcelaId); //metodo privado que carga los datos en la pantalla de contrato concesion


            return View(viewModel);
        }

        //metodo privado que carga los datos en la pantalla de contrato concesion
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


            List<DTO_Difuntos_Para_Concesion> Difuntos = new List<DTO_Difuntos_Para_Concesion>();
            List<DTO_Titulares> Titulares = new List<DTO_Titulares>();
            DTO_DatosGenerarContratoConcesion dtoDatosGenerarConcesion = new DTO_DatosGenerarContratoConcesion();

            try
            {
                //busca los difuntos en la parcela
                Difuntos = await _concesionesBusiness.ListaDifuntosPorParcela(viewModel.ParcelaId.Value);

                //busca el/los titulares, lo actualiza y los agrega a la lista de titulares
                foreach (var t in viewModel.Titulares)
                {
                    Persona titular = await _personasBusiness.ConsultarPersona(t.Id);

                    if (titular != null)
                    {
                        // Actualizo los datos del titular con la información del formulario
                        titular.Nombre = t.Nombre;
                        titular.Apellido = t.Apellido;
                        titular.Correo = t.CorreoElectronico;
                        titular.Celular = t.Celular;
                        titular.Domicilio = t.Domicilio;
                        titular.Sexo = t.Sexo;
                        int resultado = await _personasBusiness.ModificarPersona(titular);

                        Persona titularActualizado = await _personasBusiness.ConsultarPersona(t.Id); // Vuelvo a consultar para asegurarme de tener los datos actualizados

                        // Agrego el titular actualizado a la lista de titulares
                        Titulares.Add(new DTO_Titulares
                        {
                            Id = titularActualizado.IdPersona,
                            Dni = titularActualizado.Dni,
                            Nombre = titular.Nombre,
                            Apellido = titularActualizado.Apellido,
                            Sexo = titularActualizado.Sexo,
                            Celular = titularActualizado.Celular ?? "",
                            CorreoElectronico = titularActualizado.Correo ?? "",
                            Domicilio = titularActualizado.Domicilio ?? ""
                        });
                    }
                }

                DateTime fechaGeneracion = DateTime.Now;
                int? usuarioId = HttpContext.Session.GetInt32("idUsuario");

                //creo el dto para enviar a la siguiente pantalla de generacion de contrato concesion
                dtoDatosGenerarConcesion.Titulares = Titulares;
                dtoDatosGenerarConcesion.Difuntos = Difuntos;
                dtoDatosGenerarConcesion.TipoParcela = viewModel.tipoParcela.Value;
                dtoDatosGenerarConcesion.SeccionNombre = viewModel.seccion;
                dtoDatosGenerarConcesion.ParcelaString = viewModel.ParcelaString;
                dtoDatosGenerarConcesion.PrecioId = viewModel.PrecioSeleccionado.Value;
                dtoDatosGenerarConcesion.Precio = viewModel.PrecioFinal;
                dtoDatosGenerarConcesion.ParcelaId = viewModel.ParcelaId.Value;
                dtoDatosGenerarConcesion.CantidadAniosId = viewModel.CantidadAniosId.Value; //aca debe estar el id de cantidad de años años
                dtoDatosGenerarConcesion.Vencimiento = viewModel.Vencimiento.Value;
                dtoDatosGenerarConcesion.NroConcesion = viewModel.NroConcesion ?? "";
                dtoDatosGenerarConcesion.formaPago = viewModel.FormaDePago;
                dtoDatosGenerarConcesion.NroParcela = viewModel.NroParcela;
                dtoDatosGenerarConcesion.NroFila = viewModel.NroFila;
                dtoDatosGenerarConcesion.fechaGeneracion = fechaGeneracion;
                dtoDatosGenerarConcesion.EmpleadoId = usuarioId ?? 0;

                if (viewModel.FormaDePago == "cuota")
                {
                    dtoDatosGenerarConcesion.CuotaId = viewModel.CantidadCuotaSeleccionada;
                }
                else // otra forma de pago
                {
                    dtoDatosGenerarConcesion.CuotaId = null;
                    dtoDatosGenerarConcesion.PagoDescripcion = viewModel.otraFormaPago ?? "";
                }

                //se genera el tramite de contrato de concesion en estado iniciado
                Tramite tramite = new Tramite
                {
                    TipoTramiteId = (int)TipotamiteEmun.ContratoDeConcesion,
                    FechaCreacion = DateTime.Now,
                    EstadoActualId = (int)EstadosContratoConcesion.Iniciado,
                    Visibilidad = true,
                    Usuario = usuarioId ?? 0
                };


                //si el par (parcelaId y nro de concesion) existe se modifica con los valores nuevos
                if (!string.IsNullOrEmpty(viewModel.NroConcesion))
                {
                    //si existe el nroTramite es > 0
                    int nroTramite = await _concesionesBusiness.VerificarSiExisteContratoConcesion(viewModel.NroConcesion, viewModel.ParcelaId ?? 0);
                    if (nroTramite > 0)
                    {
                        //busco el contrato existente por el nroTramite
                        CemSys2.Models.ContratoConcesion contratoConcesion = await _concesionesBusiness.ConsultarContratoConcesion(nroTramite);
                        //modifico el contrato existente con los datos nuevos
                        contratoConcesion.CantidadAnios = dtoDatosGenerarConcesion.CantidadAniosId;
                        contratoConcesion.Vencimiento = dtoDatosGenerarConcesion.Vencimiento;
                        contratoConcesion.PrecioTarifariaId = dtoDatosGenerarConcesion.PrecioId;
                        contratoConcesion.CuotaId = dtoDatosGenerarConcesion.CuotaId;
                        contratoConcesion.PagoDescripcion = dtoDatosGenerarConcesion.PagoDescripcion;
                        contratoConcesion.Empleado = dtoDatosGenerarConcesion.EmpleadoId;
                        contratoConcesion.Precio = dtoDatosGenerarConcesion.Precio;

                        //modifico el tramite de contrato concesion
                        bool contratoModificado = await _concesionesBusiness.ModificarContratoConcesion(contratoConcesion);
                    }
                    else //si no existe se crea un nuevo contrato
                    {
                        bool contratoGenerado = await _concesionesBusiness.GenerarContrato(dtoDatosGenerarConcesion, tramite);
                    }
                }
                


            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
                await CargarDatosPantallaContrato(viewModel, viewModel.ParcelaId ?? 0);
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
            //return new ViewAsPdf($"{nombreVistaContrato}", contratoPDF_VM)
            //{
            //    PageMargins = new Rotativa.AspNetCore.Options.Margins(5, 5, 5, 10),
            //    PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //    FileName = null // null = lo abre en el visor del navegador
            //};
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
                await CargarDatosPantallaContrato(viewModel, viewModel.ParcelaId ?? 0);
                return View("ContratoConcesion", viewModel);
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
                Persona contribuyenteExistente = await _introduccionBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);
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
