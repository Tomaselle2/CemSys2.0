using CemSys2.DTO.Introduccion;
using CemSys2.DTO.Reportes;
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
        private readonly IRepositoryDB<EmpresaFunebre> _empresaFunebreBD;

        public IntroduccionBD(AppDbContext context, IRepositoryDB<EstadoDifunto> estadoDifuntoBD, IRepositoryDB<TipoParcela> tipoParcelaBD, IRepositoryDB<EmpresaFunebre> empresaFunebreBD)
        {
            _context = context;
            _estadoDifuntoBD = estadoDifuntoBD;
            _tipoParcelaBD = tipoParcelaBD;
            _empresaFunebreBD = empresaFunebreBD;
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

        public async Task<List<DTO_parcelaIntroduccion>> ListaParcelas(int idSeccion, int estadoDifuntoId)
        {
            var tipoParcela = await _context.Secciones
                .Where(s => s.Id == idSeccion)
                .Select(s => s.TipoParcela)
                .FirstOrDefaultAsync();

            var parcelasQuery = _context.Parcelas
                .Where(p => p.Seccion == idSeccion && p.Visibilidad == true);

            if (estadoDifuntoId == 1)
            {
                if (tipoParcela == 1)
                {
                    parcelasQuery = parcelasQuery.Where(p => p.CantidadDifuntos == 0);
                }
                // si tipoParcela != 1 no aplicamos más filtros
            }
            else
            {
                if (tipoParcela == 1 || tipoParcela == 2)
                {
                    parcelasQuery = parcelasQuery.Where(p => p.CantidadDifuntos < 5);
                }
                else if (tipoParcela == 3)
                {
                    parcelasQuery = parcelasQuery.Where(p => p.CantidadDifuntos < 20);
                }
            }

            return await parcelasQuery
                .Select(p => new DTO_parcelaIntroduccion
                {
                    Id = p.Id,
                    NroParcela = p.NroParcela,
                    NroFila = p.NroFila,
                    SeccionId = p.Seccion,
                    CantidadDifuntos = p.CantidadDifuntos
                }).ToListAsync();
        }

        public async Task<List<EmpresaFunebre>> ListaEmpresasFunebres()
        {
            return await _empresaFunebreBD.EmitirListado();
        }

        public async Task<List<DTO_UsuarioIntroduccion>> ListaEmpleados()
        {
            return await _context.Usuarios
                .Where(u => u.Visibilidad == true) 
                .Select(u => new DTO_UsuarioIntroduccion
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                }).ToListAsync();
        }

        public async Task<int> RegistrarEmpresaSepelio(EmpresaFunebre model)
        {
            return await _empresaFunebreBD.Registrar(model);
        }

        public async Task<Persona?> ConsultarDifunto(string dni)
        {
           return await _context.Personas.Where(p => p.Visibilidad == true && p.Dni == dni).FirstOrDefaultAsync();
        }

        public async Task<int> RegistrarIntroduccionCompleta(ActaDefuncion actaDefuncion, Persona difunto, int empleadoId, int empresaSepelioId, int ParcelaId, DateTime fechaIngreso)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Registrar Acta de Defunción
                _context.ActaDefuncions.Add(actaDefuncion);
                await _context.SaveChangesAsync();

                var estadoDifunto = await _context.EstadoDifuntos
                    .Where(x => x.Id == difunto.EstadoDifunto)
                    .Select(x => x.Estado)
                    .FirstOrDefaultAsync();

                // Asignar el acta al difunto
                difunto.ActaDefuncion = actaDefuncion.Id;
                _context.Personas.Add(difunto);
                await _context.SaveChangesAsync();


                int tipoTramiteId = 1; // fijo
                int estadoTramiteId = 1; // estado inicial, "Registrado"

                // Registrar Trámite
                Tramite tramite = new Tramite
                {
                    Id = await ObtenerProximoIdTramite(),
                    TipoTramiteId = tipoTramiteId,
                    FechaCreacion = DateTime.Now,
                    Usuario = empleadoId, 
                    Visibilidad = true,
                    EstadoActualId = estadoTramiteId
                };

                _context.Tramites.Add(tramite);
                await _context.SaveChangesAsync();


                // Registrar Historial Estado Trámite
                HistorialEstadoTramite historial = new HistorialEstadoTramite
                {
                    TramiteId = tramite.Id,
                    EstadoTramiteId = estadoTramiteId,
                    Fecha = DateTime.Now
                };
                _context.HistorialEstadoTramites.Add(historial);

                // Registrar Introducción
                Introduccione introduccion = new Introduccione
                {
                    IdTramite = tramite.Id,
                    Visibilidad = true,
                    FechaIngreso = fechaIngreso,
                    Empleado = tramite.Usuario,
                    EmpresaFunebre = empresaSepelioId, 
                    ParcelaId = ParcelaId, 
                    DifuntoId = difunto.IdPersona,
                    EstadoDifunto = estadoDifunto,
                    IntroduccionNueva = true,
                    FechaRetiro = null
                };
                _context.Introducciones.Add(introduccion);

                Parcela? parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == ParcelaId);
                if(parcela != null)
                    parcela.CantidadDifuntos++; //suma uno a cantidad de difuntos

                await _context.SaveChangesAsync();
                
                // Registrar ParcelaDifuntos
                ParcelaDifunto parcelaDifunto = new ParcelaDifunto
                {
                    ParcelaId = introduccion.ParcelaId,
                    DifuntoId = difunto.IdPersona,
                    FechaIngreso = DateTime.Now,
                    EstadoActual = true,
                    TramiteIngresoId = tramite.Id
                };
                _context.ParcelaDifuntos.Add(parcelaDifunto);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return tramite.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<int> ObtenerProximoIdTramite()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }

        public async Task<(List<Introduccione> introducciones, int totalRegistros)> ListadoIntroducciones(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int registrosPorPagina = 10, int pagina = 1)
        {
            var query = _context.Introducciones
             .Include(d => d.Difunto)
             .Include(p => p.Parcela)
             .ThenInclude(s => s.SeccionNavigation)
             .AsQueryable();

            // Aplicar filtros de fecha si existen
            if (fechaDesde.HasValue)
            {
                query = query.Where(x => x.FechaIngreso >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                // Añadir un día para incluir todo el día hasta
                query = query.Where(x => x.FechaIngreso < fechaHasta.Value.AddDays(1));
            }

            var totalRegistros = await query.CountAsync();

            var introducciones = await query
                .OrderByDescending(x => x.IdTramite)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            return (introducciones, totalRegistros);
        }





        //reportes
        public async Task<List<Introduccione>> ReporteIntroducciones(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = _context.Introducciones
                .Include(i => i.Parcela)
                    .ThenInclude(p => p.SeccionNavigation)
                        .ThenInclude(s => s.TipoParcelaNavigation)
                    .Include(e => e.EmpresaFunebreNavigation)
                    .Include(em => em.EmpleadoNavigation)
                        .Where(i => i.FechaIngreso.HasValue);

            if(desde.HasValue && hasta.HasValue)
            {
                query = query.Where(i => i.FechaIngreso >= desde.Value && i.FechaIngreso <= hasta);
            }

            return await query.ToListAsync();
        }


    }
}
