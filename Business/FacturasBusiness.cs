using CemSys2.DTO.Concesiones;
using CemSys2.DTO.Factura;
using CemSys2.Enumerable;
using CemSys2.Interface.Facturas;
using CemSys2.Models;
using System.ComponentModel.DataAnnotations;

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

        //verifica el detalle de la factura para generar la factura
        public void VerificarDetalleFactura(DTO_VerificarDetalleFactura DTO_verificarDetalleFactura)
        {
            if(DTO_verificarDetalleFactura.Contribuyente == 0 || DTO_verificarDetalleFactura.Contribuyente == null) //si no hay contribuyente seleccionado
                throw new ValidationException("Debe seleccionar un titular para la factura");
            
            if (DTO_verificarDetalleFactura.DetallesFactura.Count == 0) //si no hay conceptos seleccionados
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

                // Mapear el tipo MIME --quitar de aqui
                string mimeType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".pdf" => "application/pdf",
                    _ => "application/octet-stream"
                };
            }

            
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
    }
}
