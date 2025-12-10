using CemSys2.DTO.SeccionesGraficas;
using CemSys2.Models;
using Microsoft.AspNetCore.Mvc;

namespace CemSys2.Interface.SeccionesGraficas
{
    public interface ISeccionesGraficasData
    {
        Task<DatosSeccionDto> ObtenerDatosSeccionAsync(int seccionId);
    }
}
