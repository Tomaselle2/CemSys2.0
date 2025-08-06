using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Reportes;
using CemSys2.Interface.Introduccion;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class IntroduccionBusiness : IIntroduccionBusiness
    {
        private readonly IIntroduccionBD _introduccionBD;
        private int conceptoTarifariaId_inhumacion = 11;
        private int conceptoTarifariaId_cierreNicho = 4;
        private int conceptoTarifariaId_defuncion = 12;
        private int conceptoTarifariaId_Transcripcion = 13;
        private int conceptoTarifariaId_introduccionFeretro = 14;
        private int conceptoTarifariaId_introduccionUrna = 15;

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

        public async Task<int> RegistrarIntroduccionCompleta(ActaDefuncion actaDefuncion, Persona difunto, int empleadoId, int empresaSepelioId, int ParcelaId, DateTime fechaIngreso)
        {
            return await _introduccionBD.RegistrarIntroduccionCompleta(
                actaDefuncion, difunto, empleadoId, empresaSepelioId, ParcelaId, fechaIngreso, 
                await ListaConceptoFactura(difunto.DomicilioEnTirolesa, difunto.FallecioEnTirolesa, ParcelaId, difunto.EstadoDifunto.Value)
                );
        }






        //reportes
        public async Task<List<Introduccione>> ReporteIntroducciones(DateTime? desde = null, DateTime? hasta = null)
        {
           return await _introduccionBD.ReporteIntroducciones(desde, hasta);
        }

        //genero la lista de conceptos de factura
        public async Task<List<ConceptosFactura>> ListaConceptoFactura(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int parcelaId, int estadoDifuntoId)
        {
            List<ConceptosFactura> conceptosFactura = new List<ConceptosFactura>();

            //verifico el tipo de parcela
            Parcela parcela = await _introduccionBD.ConsultarParcela(parcelaId);
            switch (parcela.SeccionNavigation.TipoParcela)
            {
                case 1: //logica de precios de nicho
                    if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                    {
                        conceptosFactura = await PreciosNichosFeretroYReduccion(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos);
                    }
                    else //cremado
                    {
                        //logica de precios de cenizas
                        conceptosFactura = await PreciosNichosUrnas(domicilioEnTirolesa, fallecioEnTirolesa, parcela.CantidadDifuntos);
                    }

                    break;
                case 2: //logica de precios de fosa
                    if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                    {

                    }
                    else //cremado
                    {
                        //logica de precios de cenizas
                    }
                    break;
                case 3: //logica de precios de panteon
                    if (estadoDifuntoId == 1 || estadoDifuntoId == 2) //cuerpo completo
                    {

                    }
                    else //cremado
                    {
                        //logica de precios de cenizas
                    }
                    break;
            }

            return conceptosFactura;
        }

        private async Task<List<ConceptosFactura>> PreciosNichosFeretroYReduccion(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos)
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
                    //se cobra la apertura de nicho
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacion));

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
                    //se cobra la apertura de nicho
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacion));

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
                    switch (precio.ConceptoTarifariaId)
                    {
                        case 11: // Inhumación
                            precioFinal += precioFinal * 2; // +100%
                            break;
                        case 14: // Introducción de féretro
                            precioFinal += precioFinal * 2; // +100%
                            break;
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
                    //se cobra la apertura de nicho
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacion));

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

        private async Task<List<ConceptosFactura>> PreciosNichosUrnas(bool? domicilioEnTirolesa, bool? fallecioEnTirolesa, int cantidadDeDifuntos)
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
                    //se cobra la apertura de nicho
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacion));

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

                    // Aplico porcentaje según el ConceptoTarifariaId
                    switch (precio.ConceptoTarifariaId)
                    {
                        case 11: // Inhumación
                            precioFinal += precioFinal * 2; // +100%
                            break;
                        case 15: // Introducción de urna
                            precioFinal += precioFinal * 2; // +100%
                            break;
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
                    //se cobra la apertura de nicho
                }

                //busco el precio de tarifa de inhumación(11)
                listaPrecios.Add(await _introduccionBD.PrecioTarifaria(tarifaria.Id, conceptoTarifariaId_inhumacion));

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
    }
}
