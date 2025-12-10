using CemSys2.DTO.SeccionesGraficas;
using CemSys2.Interface.SeccionesGraficas;
using CemSys2.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;


namespace CemSys2.Data
{
    public class SeccionesGraficasData : ISeccionesGraficasData
    {
        readonly AppDbContext _context;
        public SeccionesGraficasData(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DatosSeccionDto> ObtenerDatosSeccionAsync(int seccionId)
        {
            // Obtener la sección
            var seccion = await _context.Secciones
                .Where(s => s.Id == seccionId)
                .Select(s => new SeccionDto
                {
                    id = s.Id,
                    nombre = s.Nombre,
                    visibilidad = s.Visibilidad,
                    filas = s.Filas,
                    nroParcelas = s.NroParcelas,
                    tipoNumeracionParcelas = s.TipoNumeracionParcela,
                    tipoParcela = s.TipoParcela
                })
                .FirstOrDefaultAsync();

            if (seccion == null)
            {
                return null; // O lanzar excepción
            }

            // Obtener las parcelas de la sección
            var parcelas = await _context.Parcelas
                .Where(p => p.Seccion == seccionId)
                .OrderBy(p => p.NroFila)
                .ThenBy(p => p.NroParcela)
                .Select(p => new ParcelaDto
                {
                    id = p.Id,
                    visibilidad = p.Visibilidad,
                    nroParcela = p.NroParcela,
                    nroFila = p.NroFila,
                    cantidadDifuntos = p.CantidadDifuntos,
                    seccionId = p.Seccion,
                    tipoNichoId = p.TipoNicho,
                    tipoPanteonId = p.TipoPanteonId,
                    nombrePanteon = p.NombrePanteon
                })
                .ToListAsync();

            // Obtener todos los difuntos de las parcelas de esta sección
            var difuntos = await _context.ParcelaDifuntos
                .Where(pd => pd.Parcela.Seccion == seccionId
                          && pd.EstadoActual == true
                          && pd.FechaRetiro == null)
                .Select(pd => new DifuntoDto
                {
                    id = pd.Difunto.IdPersona,
                    nombre = pd.Difunto.Nombre,
                    apellido = pd.Difunto.Apellido,
                    dni = pd.Difunto.Dni,
                    visibilidad = pd.Difunto.Visibilidad,
                    fechaNacimeinto = pd.Difunto.FechaNacimiento != null
                        ? pd.Difunto.FechaNacimiento.Value.ToString("dd/MM/yyyy")
                        : null,
                    fechaDefuncion = pd.Difunto.FechaDefuncion != null
                        ? pd.Difunto.FechaDefuncion.Value.ToString("dd/MM/yyyy")
                        : null,
                    estadoDifuntoId = pd.Difunto.EstadoDifunto,
                    sexo = pd.Difunto.Sexo,
                    parcelaId = pd.ParcelaId
                })
                .ToListAsync();

            // Construir y retornar el DTO completo
            return new DatosSeccionDto
            {
                seccion = seccion,
                parcelas = parcelas,
                difuntos = difuntos
            };
        }
    
    }
}
