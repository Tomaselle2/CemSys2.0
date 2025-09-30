using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Enumerable;
using CemSys2.Interface.Facturas;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class FacturasBusiness : IFacturaBusiness
    {
        private readonly IFacturasBD _facturasBD;

        public FacturasBusiness(IFacturasBD facturasBD)
        {
            _facturasBD = facturasBD;
        }
        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _facturasBD.ConsultarFacturaPorTramiteId(idTramite);
        }

        public async Task EditarArchivo(Guid archivoId, string descripcion, string categoriaArchivo, IFormFile? nuevoArchivo)
        {
            await _facturasBD.EditarArchivo(archivoId, descripcion, categoriaArchivo, nuevoArchivo);
        }

        public async Task<List<DTO_Archivos_Documentacion>> ListaArchivosTramiteId(int tramiteId)
        {
            return await _facturasBD.ListaArchivosTramiteId(tramiteId);
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
        public List<DTO_ConceptosTarifaria> ListaConceptoTarifariaConPreciosConLogicaNegocio(List<DTO_ConceptosTarifaria> conceptosTarifaria, bool fallecidoEnTirolesa)
        {
            if (fallecidoEnTirolesa == false)
            {
                foreach (var item in conceptosTarifaria)
                {
                    if (item.TipoConceptoTarifariaId == (int)TipoConceptoTarifariaEnum.Contribucion || item.TipoConceptoTarifariaId == (int)TipoConceptoTarifariaEnum.DerechoDeOficina)
                    {
                        item.Precio *= 2; //duplica el precio
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

        public async Task RegistrarArchivo(IFormFile archivo, string mimeType, int tramiteId, string categoriaArchivo, string descripcion)
        {
            await _facturasBD.RegistrarArchivo(archivo, mimeType, tramiteId, categoriaArchivo, descripcion);
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
    }
}
