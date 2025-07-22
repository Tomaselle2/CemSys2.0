using CemSys2.DTO;
using CemSys2.DTO.Parcelas;
using CemSys2.Interface;
using CemSys2.Interface.Parcelas;
using CemSys2.Models;
using System.Diagnostics;
namespace CemSys2.Business
{
    public class ParcelasBusiness : IParcelasBusiness
    {
        private readonly IRepositoryBusiness<Parcela> _parcelaRepository;
        private readonly IParcelaBD _parcelaBD;

        public ParcelasBusiness(IRepositoryBusiness<Parcela> parcelaRepository, IParcelaBD parcelaBD)
        {
            _parcelaRepository = parcelaRepository;
            _parcelaBD = parcelaBD;
        }

        public async Task RegistrarParcelas(DTO_secciones seccion)
        {

            switch (seccion.IdTipoParcela)
            {
                case 1: // Nicho
                   await RegistrarNichos(seccion);
                    break;
                case 2: //fosa
                    await RegistarFosas(seccion);
                    break;
                case 3: //panteon
                    await RegistarPanteones(seccion);
                    break;
            }
        }

        private async Task RegistrarNichos(DTO_secciones seccion)
        {
            int filas = seccion.Filas;
            int columnas = seccion.NroParcelas / filas;
            int nroNichoContador = 1;

            switch (seccion.IdTipoNumeracionParcela)
            {
                case 1: //numeracion nueva
                    for (int i = 1; i <= filas; i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            Parcela nicho = new Parcela();
                            nicho.NroFila = i;
                            nicho.NroParcela = j;
                            nicho.Visibilidad = true;
                            nicho.CantidadDifuntos = 0;
                            nicho.TipoNicho = seccion.IdTipoNicho;
                            nicho.Seccion = seccion.Id;

                            try
                            {
                                int idParcela = await _parcelaRepository.Registrar(nicho);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    break;
                case 2: //numeracion antigua
                    for (int i = 1; i <= filas; i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            Parcela nicho = new Parcela();
                            nicho.NroFila = i;
                            nicho.NroParcela = nroNichoContador;
                            nicho.Visibilidad = true;
                            nicho.CantidadDifuntos = 0;
                            nicho.TipoNicho = seccion.IdTipoNicho;
                            nicho.Seccion = seccion.Id;

                            try
                            {
                                int idParcela = await _parcelaRepository.Registrar(nicho);
                            }
                            catch (Exception)
                            {
                                throw;
                            }

                            nroNichoContador++; // Aumenta el contador después de cada nicho
                        }
                    }
                    break;
            }
        }

        private async Task RegistarFosas(DTO_secciones seccion)
        {
            for (int i = 1; i <= seccion.NroParcelas; i++)
            {
                Parcela fosa = new Parcela();
                fosa.NroParcela = i;
                fosa.Seccion = seccion.Id;
                fosa.Visibilidad = true;
                fosa.CantidadDifuntos = 0;
                fosa.NroFila = 1; // Asignar una fila por defecto, ya que las fosas no tienen filas

                try
                {
                    int idParcela = await _parcelaRepository.Registrar(fosa);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private async Task RegistarPanteones(DTO_secciones seccion)
        {
            for (int i = 1; i <= seccion.NroParcelas; i++)
            {
                Parcela panteon = new Parcela();
                panteon.NroParcela = i;
                panteon.Seccion = seccion.Id;
                panteon.Visibilidad = true;
                panteon.CantidadDifuntos = 0;
                panteon.NroFila = 1; // Asignar una fila por defecto, ya que las panteones no tienen filas
                panteon.TipoPanteonId = seccion.IdTipoPanteon;
                try
                {
                    int idParcela = await _parcelaRepository.Registrar(panteon);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosActuales(int parcelaId)
        {
            return await _parcelaBD.ListaHistorialDifuntosActuales(parcelaId);
        }

        public async Task<DTO_Parcelas_Encabezado> EncabezadoParcela(int parcelaId)
        {
            return await _parcelaBD.EncabezadoParcela(parcelaId);
        }

        public async Task<List<DTO_Historial_Parcelas>> ListaHistorialDifuntosHistoricos(int parcelaId)
        {
            return await _parcelaBD.ListaHistorialDifuntosHistoricos(parcelaId);
        }

        public async Task<List<DTO_Parcela_Tramites>> ListaParcelasTramites(int parcelaId)
        {
            return await _parcelaBD.ListaParcelasTramites(parcelaId);
        }
    }
}
