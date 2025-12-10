using CemSys2.DTO.SeccionesGraficas;

namespace CemSys2.Interface.SeccionesGraficas
{
    public interface ISeccionesGraficasBusiness
    {
        Task<DatosSeccionDto> ObtenerDatosSeccionAsync(int seccionId);

    }
}
