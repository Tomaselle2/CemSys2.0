using CemSys2.DTO.Introduccion;
using CemSys2.Interface;
using CemSys2.Interface.Introduccion;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Data
{
    public class IntroduccionBD : IIntroduccionBD
    {
        private readonly AppDbContext _context;
        private readonly IRepositoryDB<EstadoDifunto> _estadoDifuntoBD;
        private readonly IRepositoryDB<TipoParcela> _tipoParcelaBD;
        public IntroduccionBD(AppDbContext context, IRepositoryDB<EstadoDifunto> estadoDifuntoBD, IRepositoryDB<TipoParcela> tipoParcelaBD)
        {
            _context = context;
            _estadoDifuntoBD = estadoDifuntoBD;
            _tipoParcelaBD = tipoParcelaBD;
        }


        public Task<int> RegistrarActaDefuncion(ActaDefuncion model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarDifunto(Persona model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarHistorialEstadoTramite(HistorialEstadoTramite model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarIntroduccion(Introduccione model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarParcelaDifunto(ParcelaDifunto model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarTramite(Tramite model)
        {
            throw new NotImplementedException();
        }



        //Lista para combos
        public async Task<List<EstadoDifunto>> ListaEstadoDifunto()
        {
            return await _estadoDifuntoBD.EmitirListado();
        }

        public async Task<List<TipoParcela>> ListaTipoParcela()
        {
            return await _tipoParcelaBD.EmitirListado();
        }

        public async Task<List<DTO_SeccionIntroduccion>> ListaSecciones(int idTipoParcela)
        {
            return await _context.Secciones
                .Where(s => s.TipoParcela == idTipoParcela && s.Visibilidad == true)
                .Select(s => new DTO_SeccionIntroduccion
                {
                    Id = s.Id,
                    Nombre = s.Nombre
                }).ToListAsync();
        }

        public async Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion)
        {
            return await _context.Parcelas
                .Where(p => p.Seccion == idSeccion && p.Visibilidad == true)
                .Select(p => new DTO_parcelaIntroduccion
                {
                    Id = p.Id,
                    NroParcela = p.NroParcela,
                    NroFila = p.NroFila,
                    SeccionId = p.Seccion,
                    CantidadDifuntos = p.CantidadDifuntos
                }).ToListAsync();
        }
    }
}
