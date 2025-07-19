using CemSys2.DTO;
using CemSys2.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class ParcelasController : Controller
    {
        private readonly IParcelasBusiness _parcelasBusiness;

        public ParcelasController(IParcelasBusiness parcelasBusiness)
        {
            _parcelasBusiness = parcelasBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RegistrarParcelas(DTO_secciones seccion)
        {
            try
            {
                await _parcelasBusiness.RegistrarParcelas(seccion);
                return RedirectToAction(seccion.Redirigir, "Secciones");
            }
            catch (Exception ex)
            {
                return View(seccion.Redirigir, "Secciones");
            }

        }

        //metodo para ver el historial de la parcela recibe un id de la parcela
        //interface de negocio y bd de parcelas
        //capa de datos de parcelas
        //tabla contrato de concesion, parcela difunto -> si fecharetiro es null quiere decir que esta vigente
        //parcela difunto, include a difunto
        //tramites 
        public IActionResult HistorialParcela(int id)
        {
            // Implementar lógica para obtener el historial de la parcela
            // y devolver la vista correspondiente.
            return View();
        }
    }
}
