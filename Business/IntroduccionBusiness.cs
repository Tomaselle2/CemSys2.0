using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Reportes;
using CemSys2.Interface.Introduccion;
using CemSys2.Models;
using System.Numerics;

namespace CemSys2.Business
{
    public class IntroduccionBusiness : IIntroduccionBusiness
    {
        private readonly IIntroduccionBD _introduccionBD;
        private int conceptoTarifariaId_inhumacionNichoFeretro = 11;
        private int conceptoTarifariaId_inhumacionNichoUrna = 18;
        private int conceptoTarifariaId_inhumacionFosaFeretro = 19;
        private int conceptoTarifariaId_inhumacionFosaUrna = 20;
        private int conceptoTarifariaId_inhumacionPanteonFeretro = 21;
        private int conceptoTarifariaId_inhumacionPanteonUrna = 22;
        private int conceptoTarifariaId_cierreNicho = 23;
        private int conceptoTarifariaId_cierreFosa = 24;
        private int conceptoTarifariaId_defuncion = 12;
        private int conceptoTarifariaId_Transcripcion = 13;
        private int conceptoTarifariaId_introduccionFeretro = 14;
        private int conceptoTarifariaId_introduccionUrna = 15;
        private int conceptoTarifariaId_AperturaFosa = 3;
        private int conceptoTarifariaId_AperturaNichoConPlaca = 1;
        private int conceptoTarifariaId_AperturaNichoSinPlaca = 2;


        public IntroduccionBusiness(IIntroduccionBD introduccionBD)
        {
            _introduccionBD = introduccionBD;
        }

        public async Task<Persona?> ConsultarDifunto(string dni)
        {
            return await _introduccionBD.ConsultarDifunto(dni);
        }



        public async Task<(List<Introduccione> introducciones, int totalRegistros)> ListadoIntroducciones(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int registrosPorPagina = 10, int pagina = 1)
        {
            return await _introduccionBD.ListadoIntroducciones(fechaDesde, fechaHasta, registrosPorPagina, pagina);
        }

        public async Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados()
        {
            return await _introduccionBD.ListaEmpleados();
        }

        public async Task<List<EmpresaFunebre>> ListaEmpresasFunebres()
        {
            return await _introduccionBD.ListaEmpresasFunebres();
        }

        public Task<List<EstadoDifunto>> ListaEstadoDifunto()
        {
            return _introduccionBD.ListaEstadoDifunto();
        }

        public async Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId)
        {
            return await _introduccionBD.ListaParcelas(idSeccion, estadoDifuntoId);
        }

        public async Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela)
        {
            return await _introduccionBD.ListaSecciones(idTipoParcela);
        }

        public Task<List<TipoParcela>> ListaTipoParcela()
        {
            return _introduccionBD.ListaTipoParcela();
        }

        public async Task<List<DTO_Resumen_Introduccion>> ObtenerResumenIntroduccion(int idTramite)
        {
            return await _introduccionBD.ObtenerResumenIntroduccion(idTramite);
        }

        public async Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model)
        {
            return await _introduccionBD.RegistrarEmpresaSepelio(model);
        }

        public async Task<int> RegistrarIntroduccionCompleta(ActaDefuncion actaDefuncion, Persona difunto, int empleadoId, int empresaSepelioId, int ParcelaId, DateTime fechaIngreso, bool? placa = null)
        {
            return await _introduccionBD.RegistrarIntroduccionCompleta(
                actaDefuncion, difunto, empleadoId, empresaSepelioId, ParcelaId, fechaIngreso, 
                await ListaConceptoFactura(difunto.DomicilioEnTirolesa, difunto.FallecioEnTirolesa, ParcelaId, difunto.EstadoDifunto.Value, placa.Value)
                );
        }






        //reportes
        public async Task<List<Introduccione>> ReporteIntroducciones(DateTime? desde = null, DateTime? hasta = null)
        {
           return await _introduccionBD.ReporteIntroducciones(desde, hasta);
        }

        //genero la lista de conceptos de factura
        public async Task<List<ConceptosFactura>> ListaConceptoFactura(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int parcelaId, int estadoDifuntoId, bool? placa = null)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();

            //verifico el tipo de parcela
            Parcela parcela = await _introduccionBD.ConsultarParcela(parcelaId);
            switch (parcela.SeccionNavigation.TipoParcela)
            {
                case 1: //logica de precios de nicho
                    if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                    {
                        conceptosFactura = await PreciosNichosFeretroYReduccion(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos, placa);
                    }
                    else //cremado
                    {
                        //logica de precios de cenizas
                        conceptosFactura = await PreciosNichosUrnas(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos, placa);
                    }

                    break;
                case 2: //logica de precios de fosa
                    if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                    {
                        conceptosFactura = await PreciosFosaFeretroYReduccion(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos);
                    }
                    else //cremado
                    {
                        //logica de precios de cenizas
                        conceptosFactura = await PreciosFosaUrnas(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos);
                    }
                    break;
                case 3: //logica de precios de panteon
                    if(parcela.TipoPanteonId == 1) //con nichos
                    {
                        if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                        {
                            conceptosFactura = await PreciosPanteonConNichosFeretroYReduccion(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos, placa);
                        }
                        else //cremado
                        {
                            //logica de precios de cenizas
                            conceptosFactura = await PreciosPanteonConNichosUrna(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos, placa);
                        }
                    }
                    else if(parcela.TipoPanteonId == 2) //sin nichos
                    {
                        if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                        {
                            conceptosFactura = await PreciosPanteonSinNichosFeretroYReduccion(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos);
                        }
                        else //cremado
                        {
                            //logica de precios de cenizas
                            conceptosFactura = await PreciosPanteonSinNichosUrnas(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos);
                        }
                    }
                    
                    break;
            }

            return conceptosFactura;
        }

        private async Task<List<ConceptosFactura>> PreciosNichosFeretroYReduccion(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos, bool? placa = null)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            //introducción de cuerpo completo
            if (fallecioEnTirolesa == true) //fallecido en tirolesa
            {
                if(cantidadDeDifuntos >= 1) //para apertura de nicho
                {
                    
                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionNichoFeretro));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            if (fallecioEnTirolesa == false && domicilioEnTirolesa == false) //de otra localidad
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionNichoFeretro));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    if(precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionNichoFeretro 
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionFeretro)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                        {
                            ConceptoTarifariaId = precio.ConceptoTarifariaId,
                            PrecioUnitario = precioFinal,
                            Cantidad = 1,
                            TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                        });
                }
            }
            else if (fallecioEnTirolesa == false && domicilioEnTirolesa == true) //fallecido en otra localidad de tirolesa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionNichoFeretro));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }


            return conceptosFactura;
        }

        private async Task<List<ConceptosFactura>> PreciosNichosUrnas(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos, bool? placa = null)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            if(domicilioEnTirolesa == false) //urnas de otra localidad
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionNichoUrna));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionNichoUrna
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionUrna)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }

            if (domicilioEnTirolesa == true) //urnas de tiroelsa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionNichoUrna));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            return conceptosFactura;

        }

        private async Task<List<ConceptosFactura>> PreciosFosaFeretroYReduccion(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            //introducción de cuerpo completo
            if (fallecioEnTirolesa == true) //fallecido en tirolesa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {
                    listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaFosa));
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionFosaFeretro));

                //busco el precio de tarifa de cierre de fosa(3)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreFosa));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            if (fallecioEnTirolesa == false && domicilioEnTirolesa == false) //de otra localidad
            {
                if (cantidadDeDifuntos >= 1) //para apertura de fosa
                {
                    listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaFosa));
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionFosaFeretro));

                //busco el precio de tarifa de cierre de fosa(3)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreFosa));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    // Aplico porcentaje según el ConceptoTarifariaId
                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionFosaFeretro
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionFeretro)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }
            else if (fallecioEnTirolesa == false && domicilioEnTirolesa == true) //fallecido en otra localidad de tirolesa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de fosa
                {
                    listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaFosa));
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionFosaFeretro));

                //busco el precio de tarifa de cierre de fosa(3)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreFosa));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }


            return conceptosFactura;
        }

        private async Task<List<ConceptosFactura>> PreciosFosaUrnas(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            if (domicilioEnTirolesa == false) //urnas de otra localidad
            {
                if (cantidadDeDifuntos >= 1) //para apertura de fosa
                {
                    //se cobra la apertura de fosa
                    listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaFosa));
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionFosaUrna));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    // Aplico porcentaje según el ConceptoTarifariaId
                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionFosaUrna
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionUrna)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }

            if (domicilioEnTirolesa == true) //urnas de tiroelsa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de fosa
                {
                    //se cobra la apertura de fosa
                    listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaFosa));
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionFosaUrna));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            return conceptosFactura;

        }

        private async Task<List<ConceptosFactura>> PreciosPanteonSinNichosFeretroYReduccion(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            //introducción de cuerpo completo
            if (fallecioEnTirolesa == true) //fallecido en tirolesa
            {
                //sin apertura de nicho ya que no tiene

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonFeretro));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            if (fallecioEnTirolesa == false && domicilioEnTirolesa == false) //de otra localidad
            {

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonFeretro));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    // Aplico porcentaje según el ConceptoTarifariaId
                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionPanteonFeretro
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionFeretro)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }
            else if (fallecioEnTirolesa == false && domicilioEnTirolesa == true) //fallecido en otra localidad de tirolesa
            {

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonFeretro));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }


            return conceptosFactura;
        }

        private async Task<List<ConceptosFactura>> PreciosPanteonSinNichosUrnas(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            if (domicilioEnTirolesa == false) //urnas de otra localidad
            {

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonUrna));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    // Aplico porcentaje según el ConceptoTarifariaId
                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionPanteonUrna
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionUrna)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }

            if (domicilioEnTirolesa == true) //urnas de tiroelsa
            {
 

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonUrna));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            return conceptosFactura;

        }

        private async Task<List<ConceptosFactura>> PreciosPanteonConNichosFeretroYReduccion(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos, bool? placa = null)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            //introducción de cuerpo completo
            if (fallecioEnTirolesa == true) //fallecido en tirolesa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonFeretro));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            if (fallecioEnTirolesa == false && domicilioEnTirolesa == false) //de otra localidad
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonFeretro));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    // Aplico porcentaje según el ConceptoTarifariaId
                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionPanteonFeretro
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionFeretro)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }
            else if (fallecioEnTirolesa == false && domicilioEnTirolesa == true) //fallecido en otra localidad de tirolesa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonFeretro));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de transcripcion (13)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_Transcripcion));

                //busco el precio de tarifa de introduccion de feretro(14)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionFeretro));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }


            return conceptosFactura;
        }

        private async Task<List<ConceptosFactura>> PreciosPanteonConNichosUrna(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos, bool? placa = null)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();
            List<PreciosTarifaria> listaPrecios = new();

            if (domicilioEnTirolesa == null || fallecioEnTirolesa == null)//si vienen nulos los trato como false
            {
                domicilioEnTirolesa = false;
                fallecioEnTirolesa = false;
            }

            //busco la tarifaria actual
            Tarifaria tarifaria = await _introduccionBD.TarifariaVigente();

            if (tarifaria == null)
            {
                throw new Exception("No hay tarifaria vigente");
            }

            if (domicilioEnTirolesa == false) //urnas de otra localidad
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonUrna));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                foreach (var precio in listaPrecios)
                {
                    decimal precioFinal = precio.Precio;

                    // Aplico porcentaje según el ConceptoTarifariaId
                    if (precio.ConceptoTarifariaId == conceptoTarifariaId_inhumacionPanteonUrna
                        || precio.ConceptoTarifariaId == conceptoTarifariaId_introduccionUrna)
                    {
                        precioFinal = precioFinal * 2; // +100%
                    }

                    // Agrego a la lista de conceptos de factura
                    conceptosFactura.Add(new ConceptosFactura
                    {
                        ConceptoTarifariaId = precio.ConceptoTarifariaId,
                        PrecioUnitario = precioFinal,
                        Cantidad = 1,
                        TipoConceptoFacturaId = precio.ConceptoTarifaria.TipoConceptoId
                    });
                }
            }

            if (domicilioEnTirolesa == true) //urnas de tiroelsa
            {
                if (cantidadDeDifuntos >= 1) //para apertura de nicho
                {

                    if (placa == true)
                    {
                        //se cobra la apertura de nicho con placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoConPlaca));
                    }
                    else
                    {
                        //se cobra la apertura de nicho sin placa
                        listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_AperturaNichoSinPlaca));
                    }
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacionPanteonUrna));

                //busco el precio de tarifa de cierre de nicho(4)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_cierreNicho));

                //busco el precio de tarifa de defucion(12)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_defuncion));

                //busco el precio de tarifa de introduccion de urna(15)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_introduccionUrna));

                //itero sobre la lista de precios y agrego los conceptos a la lista de conceptosFactura
                for (int i = 0; i < listaPrecios.Count(); i++)
                {
                    conceptosFactura.Add( //agregar el concepto a la lista de conceptos de factura

                        new ConceptosFactura
                        {
                            ConceptoTarifariaId = listaPrecios[i].ConceptoTarifariaId,
                            PrecioUnitario = listaPrecios[i].Precio,
                            Cantidad = 1,
                            TipoConceptoFacturaId = listaPrecios[i].ConceptoTarifaria.TipoConceptoId,
                        }
                     );
                }
            }

            return conceptosFactura;

        }

        public async Task<Parcela> ConsultarParcela(int idParcela)
        {
            return await _introduccionBD.ConsultarParcela(idParcela);
        }

        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _introduccionBD.ConsultarFacturaPorTramiteId(idTramite);
        }

        public async Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura)
        {
            return await _introduccionBD.ListaConceptosFacturaPorFactura(idFactura);
        }

        public async Task RegistrarReciboFactura(RecibosFactura recibo, IFormFile archivo, string mimeType, int tramiteId)
        {
            await _introduccionBD.RegistrarReciboFactura(recibo, archivo, mimeType, tramiteId);
        }

        public async Task<List<RecibosFactura>> ListaRecibosFactura(int facturaId)
        {
            return await _introduccionBD.ListaRecibosFactura(facturaId);
        }
    }
}
