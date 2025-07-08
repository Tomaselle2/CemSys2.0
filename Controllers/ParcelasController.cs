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
    }
}
