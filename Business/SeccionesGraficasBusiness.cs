using CemSys2.DTO;
using CemSys2.DTO.SeccionesGraficas;
using CemSys2.Interface;
using CemSys2.Interface.SeccionesGraficas;
using CemSys2.Models;
using System.Linq.Expressions;

namespace CemSys2.Business
{
    public class SeccionesGraficasBusiness : ISeccionesGraficasBusiness
    {

        private readonly ISeccionesGraficasData _seccionesGraficasData;
        public SeccionesGraficasBusiness(ISeccionesGraficasData seccionesGraficasData)
        {
            _seccionesGraficasData = seccionesGraficasData;
        }

        public async Task<DatosSeccionDto> ObtenerDatosSeccionAsync(int seccionId)
        {
            return await _seccionesGraficasData.ObtenerDatosSeccionAsync(seccionId);
        }
    }
}
