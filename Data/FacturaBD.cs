using CemSys2.Interface.Facturas;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Data
{
    public class FacturaBD : IFacturasBD
    {
        private AppDbContext _context;

        public FacturaBD(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<int> RegistrarConceptoFactura(ConceptosFactura concepto)
        {
            _context.ConceptosFacturas.Add(concepto);
            await _context.SaveChangesAsync();
            return concepto.Id;
        }

        public async Task<int> RegistrarFactura(Factura factura) //devuelve el id de la factura
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();
            return factura.Id;
        }

        public async Task<Factura> ConsultarFacturaPorTramiteId(int idTramite)
        {
            return await _context.Facturas
                .FirstAsync(f => f.TramiteId == idTramite);
        }

        public async Task<List<ConceptosFactura>> ListaConceptosFacturaPorFactura(int idFactura)
        {
            return await _context.ConceptosFacturas
                .Include(c => c.ConceptoTarifaria)
                .Where(c => c.FacturaId == idFactura)
                .ToListAsync();
        }
    }
}
