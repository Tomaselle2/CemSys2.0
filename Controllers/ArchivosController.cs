using CemSys2.Business;
using CemSys2.Interface.Archivos;
using CemSys2.Interface.Introduccion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys2.Controllers
{
    public class ArchivosController : Controller
    {
        private readonly IArchivoBusiness _archivoBusiness;

        public ArchivosController(IArchivoBusiness archivoBusiness)
        {
            _archivoBusiness = archivoBusiness;
        }

        //ver Recibo o archivo
        public async Task<IActionResult> VerArchivo(Guid archivoId)
        {
            var archivo = await _archivoBusiness.ObtenerArchivo(archivoId);

            if (archivo == null || archivo.Contenido == null)
                return NotFound("Archivo no encontrado.");
            string tipo = archivo.TipoArchivo.ToLower();

            if (tipo.StartsWith("image/"))
            {
                // Convertir la imagen a PDF
                archivo.Contenido = PdfHelper.ImagenComoPdf(archivo.Contenido);
                tipo = "application/pdf";
                archivo.NombreArchivo = Path.ChangeExtension(archivo.NombreArchivo, ".pdf");
            }

            // Forzar a que el navegador intente mostrarlo
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{archivo.NombreArchivo}\"";

            return File(archivo.Contenido, tipo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarArchivo(Guid idArchivoActual, int idTramite, string descripcion, string categoriaArchivo, IFormFile? archivoNuevo, string vista, string controlador)
        {
            try
            {
                await _archivoBusiness.EditarArchivo(idArchivoActual, descripcion, categoriaArchivo, archivoNuevo);
                TempData["MensajeExito"] = "Recibo editado con éxito";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("VolverAOrigen", new { tramiteId = idTramite, vista, controlador });
        }

        public IActionResult VolverAOrigen(int tramiteId, string vista, string controlador)
        {
            // origen puede ser "Introduccion", "Concesion", "Renovacion", etc.
            return RedirectToAction(vista, controlador, new { tramiteId });
        }
    }
}
