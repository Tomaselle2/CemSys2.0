using CemSys2.DTO.Concesiones;
using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Personas;
using CemSys2.Models;
using CemSys2.ViewModel.ConcesionesViewModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace CemSys2.Business
{
    public class ConcesionesBusiness : IConcesionesBusiness
    {
        public readonly IConcesionesDB _concesionesDB;
        public readonly IUnitOfWork _unitOfWork;

        public ConcesionesBusiness(IConcesionesDB concesionesBd, IUnitOfWork unitOfWork)
        {
           _concesionesDB = concesionesBd;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CantidadCuota>> CantidadCuotas()
        {
            return await _concesionesDB.CantidadCuotas();
        }

        public async Task<ContratoConcesion> ConsultarContratoConcesion(int tramiteId)
        {
            return await _concesionesDB.ConsultarContratoConcesion(tramiteId);
        }

        public async Task<DTO_Datos_Concesion> DatosParcela(int parcelaId)
        {
            return await _concesionesDB.DatosParcela(parcelaId);
        }

        public async Task FinalizarPendienteDocumentacion(int tramiteId)
        {
            await _concesionesDB.FinalizarPendienteDocumentacion(tramiteId);
        }

        //Registra el contrato de concesion
        public async Task<DTO_DatosGenerarContratoConcesion> GenerarContrato(GenerarContratoVM viewModel, int? usuarioId)
        {

            List<DTO_Difuntos_Para_Concesion> Difuntos = new List<DTO_Difuntos_Para_Concesion>();
            List<DTO_Titulares> Titulares = new List<DTO_Titulares>();
            DTO_DatosGenerarContratoConcesion dtoDatosGenerarConcesion = new DTO_DatosGenerarContratoConcesion();

            await _unitOfWork.ExecuteInTransactionAsync( async () => {

                
                //busca los difuntos en la parcela
                Difuntos = await _unitOfWork._concesionesBD.ListaDifuntosPorParcela(viewModel.ParcelaId.Value);

                //busca el/los titulares, lo actualiza y los agrega a la lista de titulares
                foreach (var t in viewModel.Titulares)
                {
                    Persona titular = await _unitOfWork._personasBD.ConsultarPersona(t.Id);

                    if (titular == null)
                        throw new ValidationException("El titular no existe");

                    if (!string.IsNullOrEmpty(t.CorreoElectronico))
                    {
                        try
                        {
                            var mailAddress = new MailAddress(t.CorreoElectronico);
                            // Validación adicional para asegurar que coincide exactamente
                            if (mailAddress.Address != t.CorreoElectronico.Trim())
                            {
                                throw new ValidationException("El correo electrónico no tiene un formato válido");
                            }
                        }
                        catch
                        {
                            throw new ValidationException("El correo electrónico no tiene un formato válido");
                        }
                    }

                    // Actualizo los datos del titular con la información del formulario
                    titular.Nombre = t.Nombre;
                    titular.Apellido = t.Apellido;
                    titular.Correo = t.CorreoElectronico;
                    titular.Celular = t.Celular;
                    titular.Domicilio = t.Domicilio;
                    titular.Sexo = t.Sexo;

                    int resultado = await _unitOfWork._personasBD.ModificarPersona(titular);

                    // Agrego el titular actualizado a la lista de titulares
                    Titulares.Add(new DTO_Titulares
                    {
                        Id = titular.IdPersona,
                        Dni = titular.Dni,
                        Nombre = titular.Nombre,
                        Apellido = titular.Apellido,
                        Sexo = titular.Sexo,
                        Celular = titular.Celular ?? "",
                        CorreoElectronico = titular.Correo ?? "",
                        Domicilio = titular.Domicilio ?? ""
                    });
                    
                }

                DateTime fechaGeneracion = DateTime.Now;

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
                    int nroTramite = await _unitOfWork._concesionesBD.VerificarSiExisteContratoConcesion(viewModel.NroConcesion, viewModel.ParcelaId ?? 0);
                    if (nroTramite > 0)
                    {
                        //busco el contrato existente por el nroTramite
                        CemSys2.Models.ContratoConcesion contratoConcesion = await _unitOfWork._concesionesBD.ConsultarContratoConcesion(nroTramite);
                        //modifico el contrato existente con los datos nuevos
                        contratoConcesion.CantidadAnios = dtoDatosGenerarConcesion.CantidadAniosId;
                        contratoConcesion.Vencimiento = dtoDatosGenerarConcesion.Vencimiento;
                        contratoConcesion.PrecioTarifariaId = dtoDatosGenerarConcesion.PrecioId;
                        contratoConcesion.CuotaId = dtoDatosGenerarConcesion.CuotaId;
                        contratoConcesion.PagoDescripcion = dtoDatosGenerarConcesion.PagoDescripcion;
                        contratoConcesion.Empleado = dtoDatosGenerarConcesion.EmpleadoId;
                        contratoConcesion.Precio = dtoDatosGenerarConcesion.Precio;


                        //modifico el tramite de contrato concesion
                        dtoDatosGenerarConcesion.contratoGenerado = await _unitOfWork._concesionesBD.ModificarContratoConcesion(contratoConcesion);
                    }
                    else //si no existe se crea un nuevo contrato
                    {
                        dtoDatosGenerarConcesion.contratoGenerado  = await _unitOfWork._concesionesBD.GenerarContrato(dtoDatosGenerarConcesion, tramite);
                    }
                }

            });

            return dtoDatosGenerarConcesion;
        }

        public async Task<List<DTO_Difuntos_Para_Concesion>> ListaDifuntosPorParcela(int parcelaId)
        {
            return await _concesionesDB.ListaDifuntosPorParcela(parcelaId);
        }

        public async Task<DTO_Listado_Paginado_Concesiones> ListadoConcesiones(int paginaActual, int tamanoPagina)
        {
            return await _concesionesDB.ListadoConcesiones(paginaActual, tamanoPagina);
        }

        public async Task<List<DTO_Parcelas_Sin_Contrato>> ListaParcelasSinContrato()
        {
           return await _concesionesDB.ListaParcelasSinContrato();
        }

        public async Task<List<DTO_Titulares>> ListaTitularesActualesContrato(int contratoId)
        {
            return await _concesionesDB.ListaTitularesActualesContrato(contratoId);
        }

        public async Task<bool> ModificarContratoConcesion(ContratoConcesion contrato)
        {
            return await  _concesionesDB.ModificarContratoConcesion(contrato);
        }

        public async Task<bool> PasoPendienteDocumentacion(ContratoConcesion contrato, List<DTO_Titulares> titulares, int tipoConceptoTarifariaId)
        {
            return await _concesionesDB.PasoPendienteDocumentacion(contrato, titulares, tipoConceptoTarifariaId);
        }

        public async Task<List<DTO_Precios_Concesion>> PreciosConcesion(int conceptoTarifariaId, int seccionId, int nroFila)
        {
            return await _concesionesDB.PreciosConcesion(conceptoTarifariaId, seccionId, nroFila);
        }

        public async Task<Persona> RegistrarTitular(Persona titular)
        {
            return await _concesionesDB.RegistrarTitular(titular);
        }

        public Task<bool> VerificarArchivoContratoSubido(int tramiteId)
        {
            return _concesionesDB.VerificarArchivoContratoSubido(tramiteId);
        }

        public async Task<int> VerificarSiExisteContratoConcesion(string nroConcesion, int parcelaId)
        {
            return await _concesionesDB.VerificarSiExisteContratoConcesion(nroConcesion, parcelaId);
        }
    }
}
