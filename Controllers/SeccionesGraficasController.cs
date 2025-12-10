using CemSys2.Interface.SeccionesGraficas;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CemSys2.Controllers
{
    public class SeccionesGraficasController : Controller
    {
        private readonly ISeccionesGraficasBusiness _seccionesGraficasBusiness;

        public SeccionesGraficasController(ISeccionesGraficasBusiness seccionesGraficasBusiness)
        {
            _seccionesGraficasBusiness = seccionesGraficasBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> VistaSeccionGrafica(int id)
        {
            var datos = await _seccionesGraficasBusiness.ObtenerDatosSeccionAsync(id);

            if (datos == null)
            {
                return NotFound();
            }

            // Serializar a JSON para JavaScript en la vista
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // Mantiene los nombres de propiedades
                WriteIndented = true // JSON formateado (opcional)
            };

            ViewBag.DatosJson = JsonSerializer.Serialize(datos, jsonOptions);
            ViewBag.SeccionNombre = datos.seccion.nombre;

            return View();
        }

    }
}
