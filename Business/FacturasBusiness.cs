using CemSys2.Data;
using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Facturas;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CemSys2.Business
{
    public class FacturasBusiness : IFacturaBusiness
    {
        private readonly IFacturasBD _facturasBD;
        private readonly ITarifariaBusiness _tarifariaBusiness;
        public readonly IUnitOfWork _unitOfWork;

        public FacturasBusiness(IFacturasBD facturasBD, ITarifariaBusiness tarifariaBusiness, IUnitOfWork unitOfWork)
        {
            _facturasBD = facturasBD;
            _tarifariaBusiness = tarifariaBusiness;
            _unitOfWork = unitOfWork;
        }
        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _facturasBD.ConsultarFacturaPorTramiteId(idTramite);
        }

        public async Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura)
        {
            return await _facturasBD.ListaConceptosFacturaPorFactura(idFactura);
        }

        public async Task<List<DTO_ConceptosTarifaria>> ListaConceptoTarifariaIntroduccion(int tarifariaId)
        {
            return await _facturasBD.ListaConceptoTarifariaIntroduccion(tarifariaId);
        }

        //duplica el precio de los conceptos si no es "fallecido en tirolesa(false)"
        public List<DTO_ConceptosTarifaria> ListaConceptoTarifariaConPreciosConLogicaNegocio(List<DTO_ConceptosTarifaria> conceptosTarifaria, bool fallecidoEnTirolesa, bool domicilioEntirolesa)
        {
            var exclusiones = new[]
            {
                (int)ConceptosTarifariaEnum.CierreDeFosa,
                (int)ConceptosTarifariaEnum.CierreDeNicho
            };

            if (fallecidoEnTirolesa == false && domicilioEntirolesa == false)
            {
                foreach (var item in conceptosTarifaria)
                {
                    if (item.TipoConceptoTarifariaId == (int)TipoConceptoTarifariaEnum.Contribucion || item.TipoConceptoTarifariaId == (int)TipoConceptoTarifariaEnum.DerechoDeOficina)
                    {
                        if (!exclusiones.Contains(item.ConceptoTarifariaId))
                        {
                            item.Precio *= 2;
                        }
                    }
                }

                return conceptosTarifaria;
            }
            
            return conceptosTarifaria; //la devuelve tal cual
        }

        public async Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId)
        {
            return await _facturasBD.ListaRecibosFactura(facturaId);
        }

        public async Task<int> RegistrarConceptoFactura(ConceptosFactura concepto)
        {
            return await _facturasBD.RegistrarConceptoFactura(concepto);
        }

        public async Task<int> RegistrarFactura(Factura factura)
        {
            return await _facturasBD.RegistrarFactura(factura);
        }

        public async Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId)
        {
            await _facturasBD.RegistrarReciboFactura(recibo, archivo, mimeType, tramiteId);
        }

        //verifica el detalle de la factura para generar la factura
        public async Task<int> VerificarDetalleFactura(DTO_VerificarDetalleFactura DTO_verificarDetalleFactura)
        {
            int facturaId = 0;
            string mimeType = string.Empty;

            if (DTO_verificarDetalleFactura.Contribuyente == 0 || DTO_verificarDetalleFactura.Contribuyente == null) //si no hay contribuyente seleccionado
                throw new ValidationException("Debe seleccionar un titular para la factura");
            
            if (DTO_verificarDetalleFactura.Decreto == false && DTO_verificarDetalleFactura.DetallesFactura.Count == 0) //si no hay conceptos seleccionados
                throw new ValidationException("Debe seleccionar al menos un concepto para la factura");

            if(DTO_verificarDetalleFactura.Decreto && (DTO_verificarDetalleFactura.Archivo == null || DTO_verificarDetalleFactura.Archivo.Length == 0)) //decreto true y sin archivo
            {
                throw new ValidationException("Debe adjuntar el archivo del decreto");
            }

            if (DTO_verificarDetalleFactura.Decreto && DTO_verificarDetalleFactura.Archivo != null) //decreto true y con archivo
            {
                // Validar extensión
                var extension = Path.GetExtension(DTO_verificarDetalleFactura.Archivo.FileName).ToLower();
                var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
                if (!permitidas.Contains(extension))
                {
                    throw new ValidationException("Solo se permiten archivos PNG, JPG o PDF.");
                }

                // Mapear el tipo MIME
                mimeType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".pdf" => "application/pdf",
                    _ => "application/octet-stream"
                };
            }

            if (DTO_verificarDetalleFactura.Decreto == true)
            {
                //verifica que el monto sea positivo mayor a 0
                if (DTO_verificarDetalleFactura.MontoDecreto != null && DTO_verificarDetalleFactura.MontoDecreto <= 0)
                    throw new ValidationException($"El monto no puede ser nulo o negativo");

                //verifica que el monto no supere el pendiente de la factura
                if (DTO_verificarDetalleFactura.MontoDecreto != null && DTO_verificarDetalleFactura.MontoDecreto > DTO_verificarDetalleFactura.Pendiente)
                    throw new ValidationException($"El monto no puede ser superior a $ {DTO_verificarDetalleFactura.Pendiente}");
            }
            
            decimal totalDetalleFactura = DTO_verificarDetalleFactura.DetallesFactura.Sum(d => d.PrecioUnitario) * (1 + await _tarifariaBusiness.ConsultarPorcentajeFondoActual());

            if (DTO_verificarDetalleFactura.Decreto == false)
            {
                //verifica que el monto sea positivo mayor a 0
                if (totalDetalleFactura <= 0)
                    throw new ValidationException($"El monto no puede ser nulo o negativo");

                //verifica que el monto no supere el pendiente de la factura
                if (totalDetalleFactura > DTO_verificarDetalleFactura.Pendiente)
                    throw new ValidationException($"El monto no puede ser superior a $ {DTO_verificarDetalleFactura.Pendiente}");
            }

            List<DTO_VerificarMontoFactura> FacturasEmitidasYPendientes = await _facturasBD.ListaFacturasEmitidasYPendientesParaVerificarPorTramite(DTO_verificarDetalleFactura.TramiteId);

            //si hay facturas emitidas y pendientes
            if (FacturasEmitidasYPendientes != null && FacturasEmitidasYPendientes.Count > 0)
            {
                decimal totalFacturasEmitidas = FacturasEmitidasYPendientes.Sum(f => f.MontoTotal);
                decimal permitidoEmitir = DTO_verificarDetalleFactura.Pendiente - totalFacturasEmitidas;

                if (permitidoEmitir <= 0)
                    throw new ValidationException($"Ya no es posible emitir nuevas facturas: el total del servicio ya está cubierto con facturas previas por $ {totalFacturasEmitidas}.");

                if (totalDetalleFactura > permitidoEmitir)
                    throw new ValidationException($"El monto a facturar no puede ser superior al restante permitido ($ {permitidoEmitir}).");

                if (DTO_verificarDetalleFactura.Decreto && DTO_verificarDetalleFactura.MontoDecreto != null &&
                    DTO_verificarDetalleFactura.MontoDecreto > permitidoEmitir)
                    throw new ValidationException($"El monto del decreto no puede ser superior al restante permitido ($ {permitidoEmitir}).");
            }

            //si llego hasta aca es porque paso todas las validaciones----------------------------------------
            Tramite tramite = await _unitOfWork._tramiteBD.ConsultarTramite(DTO_verificarDetalleFactura.TramiteId);

            if (DTO_verificarDetalleFactura.Decreto == true)
            {
                //registrar el archivo del decreto 
                // y descuenta el monto del decreto al total del tramite que puede ser introduccion o contrato
                await RegistrarArchivoDecreto(DTO_verificarDetalleFactura.Archivo, tramite, mimeType, DTO_verificarDetalleFactura.Descripcion,
                    DTO_verificarDetalleFactura.MontoDecreto.Value, DTO_verificarDetalleFactura.Pendiente, DTO_verificarDetalleFactura.Contribuyente.Value);

                facturaId = 1; //indica que es decreto
                return facturaId;
            }

            //crea el dto de la factura
            DTO_Factura dtoFactura = new DTO_Factura
            {
                TramiteId = DTO_verificarDetalleFactura.TramiteId,
                ContribuyenteId = DTO_verificarDetalleFactura.Contribuyente.Value,
                Total = totalDetalleFactura,
                Visibilidad = true,
                TipoTramiteId = tramite.TipoTramiteId,
                UsuarioEmiteId = DTO_verificarDetalleFactura.UsuarioEmiteId, 
                Descripcion = DTO_verificarDetalleFactura.Descripcion,
                EstadoId = DTO_verificarDetalleFactura.EstadoFacturaId,
            };

            facturaId = await CrearFactura(dtoFactura, DTO_verificarDetalleFactura.DetallesFactura);

            return facturaId;

        }

        //crea la factura en una transaccion
        public async Task<int> CrearFactura(DTO_Factura dtoFactura, List<DTO_DetalleFactura> dtoDetalleFactura, int cantidadMesesVencimientoProximo = 1)
        {
            int facturaId = 0;
            await _unitOfWork.ExecuteInTransactionAsync(async () => {
                //registro la factura
                Factura nuevaFactura = new Factura
                {
                    TramiteId = dtoFactura.TramiteId,
                    ContribuyenteId = dtoFactura.ContribuyenteId,
                    FechaCreacion = DateTime.Now,
                    Total = dtoFactura.Total,
                    Visibilidad = true,
                    TipoTramiteId = dtoFactura.TipoTramiteId,
                    UsuarioEmiteId = dtoFactura.UsuarioEmiteId,
                    Descripcion = dtoFactura.Descripcion,
                    EstadoId = dtoFactura.EstadoId,
                    FechaVencimiento = DateOnly.FromDateTime(DateTime.Now.AddMonths(cantidadMesesVencimientoProximo))
                };

                nuevaFactura.Id = await _unitOfWork._facturasBD.RegistrarFactura(nuevaFactura);
                await _unitOfWork.SaveChangesAsync();

                //registro los conceptos de la factura
                foreach (var detalle in dtoDetalleFactura)
                {
                    ConceptosFactura conceptoFactura = new ConceptosFactura{
                        FacturaId = nuevaFactura.Id,
                        ConceptoTarifariaId = detalle.ConceptoTarifariaId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        TipoConceptoFacturaId = detalle.TipoConceptoFacturaId
                    };

                    await _unitOfWork._facturasBD.RegistrarConceptoFactura(conceptoFactura);
                }
                await _unitOfWork.SaveChangesAsync();

                //registro el historial de estados
                HistorialEstadosFactura historial = new HistorialEstadosFactura
                {
                    FacturaId = nuevaFactura.Id,
                    EstadoId = nuevaFactura.EstadoId.Value,
                    FechaCambio = DateTime.Now,
                };

                await _unitOfWork._historialesBD.RegistrarHistorialFactura(historial);


                //VERIFICAR SI LA RELACIÓN TRÁMITE-PERSONA YA EXISTE
                bool relacionExistente = await _unitOfWork._personasBD.VerificarRelacioPersonaTramiteExiste(dtoFactura.TramiteId, dtoFactura.ContribuyenteId.Value);

                // Solo crear la relación si no existe
                if (!relacionExistente)
                {
                    TramitePersona tramitePersona = new TramitePersona
                    {
                        TramiteId = dtoFactura.TramiteId,
                        PersonaId = dtoFactura.ContribuyenteId.Value
                    };
                    await _unitOfWork._personasBD.AgregarTramitePersona(tramitePersona);
                }

                facturaId = nuevaFactura.Id;
            });

            return facturaId;
        }


        //para resumen introduccion
        public async Task<DTO_FacturaInternaPrecios> ConsultarFacturaInternaPorTramiteId(int idTramite)
        {
            return await _facturasBD.ConsultarFacturaInternaPorTramiteId(idTramite);
        }

        //para resumen introduccion
        public async Task<List<ConceptosFacturaInternasPrecio>> ListaConceptosFacturaInternaPorFactura(int idFactura)
        {
            return await _facturasBD.ListaConceptosFacturaInternaPorFactura(idFactura);
        }

        private async Task RegistrarArchivoDecreto(IFormFile archivo, Tramite tramite, string mimeType, string descipcion, decimal montoDecreto, decimal totalTramite, int contribuyente)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                //registro el archivo del decreto
                await _unitOfWork._archivoBD.RegistrarArchivo(archivo, mimeType, tramite.Id, CategoriaArchivosEnum.Decreto.ToString(), descipcion);

                totalTramite = totalTramite - montoDecreto;

                // 🔹 Actualizar el pendiente según el tipo de trámite
                switch ((TipotamiteEmun)tramite.TipoTramiteId)
                {
                    case TipotamiteEmun.Introduccion:
                        var introduccion = await _unitOfWork._introduccionBD.ObtenerPorTramiteId(tramite.Id);
                        if (introduccion != null)
                        {
                            introduccion.Pendiente = totalTramite < 0 ? 0 : totalTramite;
                            await _unitOfWork._introduccionBD.ModificarIntroduccion(introduccion);
                        }

                        if (introduccion.Pendiente.Value <= 1) //se abono todo
                        {
                            int estadoTramiteId = (int)EstadosIntroduccion.Cobrado;

                            //se agrega el estado en historial estado
                            HistorialEstadoTramite historial = new HistorialEstadoTramite
                            {
                                TramiteId = tramite.Id,
                                EstadoTramiteId = estadoTramiteId,
                                Fecha = DateTime.Now
                            };
                            await _unitOfWork._historialesBD.RegistrarHistorialTramite(historial);

                            //se actualiza el estado actual en el tramite
                            tramite.EstadoActualId = estadoTramiteId;
                            await _unitOfWork._tramiteBD.ModificarTramite(tramite);

                            //buscar todas las facturas en estado creado, emitido y pendiente de cobro y anularlas
                            List<DTO_Factura> listaFacturas = await _facturasBD.ListaFacturasPorTramiteId(tramite.Id);
                            foreach (var f in listaFacturas)
                            {
                                if(f.EstadoId == (int)EstadosFactura.Creado || f.EstadoId == (int)EstadosFactura.Emitido || f.EstadoId == (int)EstadosFactura.PendienteDeCobro)
                                {
                                    await PasarFacturaEstadoAnulado(f.Id.Value, "Anulado automáticamente por sistema");
                                }
                            }
                        }
                        break;

                    case TipotamiteEmun.ContratoDeConcesion:
                        var contrato = await _unitOfWork._concesionesBD.ConsultarContratoConcesion(tramite.Id);
                        if (contrato != null)
                        {
                            contrato.Pendiente = totalTramite < 0 ? 0 : totalTramite;
                            await _unitOfWork._concesionesBD.ModificarContratoConcesion(contrato);
                            if(contrato.Pendiente <= 1)
                            {
                                contrato.Pendiente = 0;
                                //buscar todas las facturas en estado creado, emitido y pendiente de cobro y anularlas
                                List<DTO_Factura> listaFacturas = await _facturasBD.ListaFacturasPorTramiteId(tramite.Id);
                                foreach (var f in listaFacturas)
                                {
                                    if (f.EstadoId == (int)EstadosFactura.Creado || f.EstadoId == (int)EstadosFactura.Emitido || f.EstadoId == (int)EstadosFactura.PendienteDeCobro)
                                    {
                                        await PasarFacturaEstadoAnulado(f.Id.Value, "Anulado automáticamente por sistema");
                                    }
                                }
                            }
                        }
                        break;
                }

                //VERIFICAR SI LA RELACIÓN TRÁMITE-PERSONA YA EXISTE
                bool relacionExistente = await _unitOfWork._personasBD.VerificarRelacioPersonaTramiteExiste(tramite.Id, contribuyente);

                // Solo crear la relación si no existe
                if (!relacionExistente)
                {
                    TramitePersona tramitePersona = new TramitePersona
                    {
                        TramiteId = tramite.Id,
                        PersonaId = contribuyente //la municipalidad
                    };
                    await _unitOfWork._personasBD.AgregarTramitePersona(tramitePersona);
                }
            });
        }

        public async Task<List<DTO_Factura>> ListaFacturasPorTramiteId(int tramiteId)
        {
            return await _facturasBD.ListaFacturasPorTramiteId(tramiteId);
        }


        //estados de la factura ----------------------------------------
        public async Task PasarFacturaEstadoEmitir(int idfactura)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () => {

                Factura factura = await _unitOfWork._facturasBD.ConsultarFacturaPorIdd(idfactura);

                factura.EstadoId = (int)EstadosFactura.Emitido;

                //registro el historial de estados
                HistorialEstadosFactura historial = new HistorialEstadosFactura
                {
                    FacturaId = factura.Id,
                    EstadoId = (int)EstadosFactura.Emitido,
                    FechaCambio = DateTime.Now,
                };

                await _unitOfWork._historialesBD.RegistrarHistorialFactura(historial);

            });
        }

        public async Task PasarFacturaEstadoAnulado(int idfactura, string descripcion)
        {
            Factura factura = await _unitOfWork._facturasBD.ConsultarFacturaPorIdd(idfactura);

            if (factura.EstadoId == (int)EstadosFactura.Cobrado)
                throw new InvalidOperationException("No se puede anular una factura que ya ha sido cobrada.");

            if (factura.EstadoId == (int)EstadosFactura.Anulado)
                throw new InvalidOperationException("No se puede anular una factura que ya ha sido anulada.");

            if(string.IsNullOrEmpty(descripcion))
                throw new ValidationException("Debe ingresar una descripción para anular la factura.");

            factura.EstadoId = (int)EstadosFactura.Anulado;
            factura.Descripcion = descripcion;

            //registro el historial de estados
            HistorialEstadosFactura historial = new HistorialEstadosFactura
            {
                FacturaId = factura.Id,
                EstadoId = (int)EstadosFactura.Anulado,
                FechaCambio = DateTime.Now,
            };

            await _unitOfWork._historialesBD.RegistrarHistorialFactura(historial);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PasarFacturaEstadoPendienteCobro(int idFactura)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () => {

                Factura factura = await _unitOfWork._facturasBD.ConsultarFacturaPorIdd(idFactura);

                if(factura.EstadoId != (int)EstadosFactura.PendienteDeCobro)
                {
                    factura.EstadoId = (int)EstadosFactura.PendienteDeCobro;

                    //registro el historial de estados
                    HistorialEstadosFactura historial = new HistorialEstadosFactura
                    {
                        FacturaId = factura.Id,
                        EstadoId = (int)EstadosFactura.PendienteDeCobro,
                        FechaCambio = DateTime.Now,
                    };

                    await _unitOfWork._historialesBD.RegistrarHistorialFactura(historial);
                }
            });
        }

        public async Task PasarFacturaEstadoCobrado(int idFactura)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () => {

                Factura factura = await _unitOfWork._facturasBD.ConsultarFacturaPorIdd(idFactura);

                if (factura.EstadoId != (int)EstadosFactura.Cobrado)
                {
                    factura.EstadoId = (int)EstadosFactura.Cobrado;

                    //registro el historial de estados
                    HistorialEstadosFactura historial = new HistorialEstadosFactura
                    {
                        FacturaId = factura.Id,
                        EstadoId = (int)EstadosFactura.Cobrado,
                        FechaCambio = DateTime.Now,
                    };

                    await _unitOfWork._historialesBD.RegistrarHistorialFactura(historial);
                }
            });
        }

        //para la pantalla del cajero de facturas emitidas
        public async Task<List<DTO_Factura>> ListaTotalFacturasEmitidasYPendientes()
        {
            return await _facturasBD.ListaTotalFacturasEmitidasYPendientes();
        }

        public async Task<List<DTO_Factura>> ListaFacturasPorPersonaId(int personaId)
        {
            return await _facturasBD.ListaFacturasPorPersonaId(personaId);
        }

        public async Task<DTO_Factura> ConsultarFacturaPorId(int facturaId)
        {
            return await _facturasBD.ConsultarFacturaPorId(facturaId);
        }

        public async Task<List<MetodoPago>> ListaMetodoPago()
        {
            return await _facturasBD.ListaMetodoPago(); 
        }

        public async Task VerificarCobrarFactura(DTO_VerificarCobrarFactura dto)
        {
            if (dto.MetodoPagoId == (int)MetodoPagoEnum.Efectivo && dto.EfectivoRecibido != null && dto.EfectivoRecibido <= 0)
                throw new ValidationException("El efectivo recibido debe ser mayor a cero.");

            if (dto.MetodoPagoId == (int)MetodoPagoEnum.Efectivo && dto.EfectivoRecibido != null && dto.EfectivoRecibido < dto.MontoTotal)
                throw new ValidationException("El efectivo recibido no puede ser menor al monto total de la factura.");

            await LogicaCobrarFactura(dto);
        }

        private async Task LogicaCobrarFactura(DTO_VerificarCobrarFactura dto)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>  {

                Tramite tramite = await _unitOfWork._tramiteBD.ConsultarTramite(dto.TramiteId);

                // 🔹 Actualizar el pendiente del trámite según el tipo de trámite
                switch ((TipotamiteEmun)dto.TipoTramiteId)
                {
                    case TipotamiteEmun.Introduccion:
                        var introduccion = await _unitOfWork._introduccionBD.ObtenerPorTramiteId(dto.TramiteId);
                        if (introduccion != null)
                        {
                            introduccion.Pendiente -= dto.MontoTotal;
                            if (introduccion.Pendiente <= 1)
                            {
                                introduccion.Pendiente = 0;

                                tramite.EstadoActualId = (int)EstadosIntroduccion.Cobrado;

                                HistorialEstadoTramite historialEstadoTramite = new HistorialEstadoTramite
                                {
                                     TramiteId = tramite.Id,
                                     EstadoTramiteId = (int)EstadosIntroduccion.Cobrado,
                                     Fecha = DateTime.Now
                                };

                                await _unitOfWork._historialesBD.RegistrarHistorialTramite(historialEstadoTramite);
                                await _unitOfWork._tramiteBD.ModificarTramite(tramite);

                                await _unitOfWork.SaveChangesAsync();

                                //buscar todas las facturas en estado creado, emitido y pendiente de cobro y anularlas
                                List<DTO_Factura> listaFacturas = await _facturasBD.ListaFacturasPorTramiteId(tramite.Id);
                                foreach (var f in listaFacturas)
                                {
                                    if (f.EstadoId == (int)EstadosFactura.Creado || f.EstadoId == (int)EstadosFactura.Emitido || f.EstadoId == (int)EstadosFactura.PendienteDeCobro)
                                    {
                                        await PasarFacturaEstadoAnulado(f.Id.Value, "Anulado automáticamente por sistema");
                                    }
                                }
                            }
                            await _unitOfWork._introduccionBD.ModificarIntroduccion(introduccion);
                        }
                        break;

                    case TipotamiteEmun.ContratoDeConcesion:
                        var contrato = await _unitOfWork._concesionesBD.ConsultarContratoConcesion(dto.TramiteId);
                        if (contrato != null)
                        {
                            contrato.Pendiente -= dto.MontoTotal;

                            await _unitOfWork.SaveChangesAsync();

                            if (contrato.Pendiente <= 1)
                            {
                                contrato.Pendiente = 0;
                                //buscar todas las facturas en estado creado, emitido y pendiente de cobro y anularlas
                                List<DTO_Factura> listaFacturas = await _facturasBD.ListaFacturasPorTramiteId(tramite.Id);
                                foreach (var f in listaFacturas)
                                {
                                    if (f.EstadoId == (int)EstadosFactura.Creado || f.EstadoId == (int)EstadosFactura.Emitido || f.EstadoId == (int)EstadosFactura.PendienteDeCobro)
                                    {
                                        await PasarFacturaEstadoAnulado(f.Id.Value, "Anulado automáticamente por sistema");
                                    }
                                }
                            }
                            await _unitOfWork._concesionesBD.ModificarContratoConcesion(contrato);
                        }
                        break;
                }

                decimal vuelto = 0;

                //si el efectivo recibido es mayor al monto total, se calcula el vuelto
                if (dto.MetodoPagoId == (int)MetodoPagoEnum.Efectivo && dto.EfectivoRecibido != null && dto.EfectivoRecibido > dto.MontoTotal)
                {
                    vuelto = dto.EfectivoRecibido.Value - dto.MontoTotal;
                }

                Factura factura = await _unitOfWork._facturasBD.ConsultarFacturaPorIdd(dto.FacturaId);
                factura.Vuelto = vuelto;
                factura.MetodoPagoId = dto.MetodoPagoId;
                factura.UsuarioCajeroId = dto.CajeroId;
                factura.Total += dto.Interes;
                factura.InteresAplicado = dto.Interes;
            });

            await PasarFacturaEstadoCobrado(dto.FacturaId);
        }


        public async Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasCobradas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            return await _facturasBD.ListaTotalFacturasCobradas(paginaActual, registrosPorPagina, fechaDesde, fechaHasta);
        }

        public async Task<(List<DTO_Factura> Lista, int TotalRegistros)> ListaTotalFacturasAnuladas(int paginaActual, int registrosPorPagina, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            return await _facturasBD.ListaTotalFacturasAnuladas(paginaActual, registrosPorPagina, fechaDesde, fechaHasta);
        }

        public async Task<List<DTO_HistorialEstadoFactura>> HistorialEstadoFacturaPorFacturaId(int facturaId)
        {
            return await _facturasBD.HistorialEstadoFacturaPorFacturaId(facturaId);
        }

        private const double TasaMensual = 0.055; // 5.5% mensual

        public decimal CalcularInteres(decimal totalFactura, DateTime fechaVencimiento)
        {
            DateTime hoy = DateTime.Today;

            if (hoy <= fechaVencimiento)
                return 0;

            int diasAtraso = (hoy - fechaVencimiento).Days;
            DateTime fechaActual = fechaVencimiento;

            double montoConInteres = (double)totalFactura;

            for (int i = 0; i < diasAtraso; i++)
            {
                int diasEnMes = DateTime.DaysInMonth(fechaActual.Year, fechaActual.Month);
                double tasaDiaria = TasaMensual / diasEnMes;

                montoConInteres *= (1 + tasaDiaria);

                fechaActual = fechaActual.AddDays(1);
            }

            decimal interes = (decimal)montoConInteres - totalFactura;
            return Math.Round(interes, 2);
        }
    }
}
