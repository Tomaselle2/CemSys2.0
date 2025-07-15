using CemSys2.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace CemSys2.Controllers
{
    public class IntroduccionController : Controller
    {
        public IActionResult Index(int pagina = 1)
        {
            IntroduccionIndexVM viewModelIndex = new IntroduccionIndexVM();
            return View(viewModelIndex);
        }

        public IActionResult IntroduccionDifunto()
        {
            return View();
        }
    }
}
