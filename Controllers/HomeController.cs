using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CemSys2.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        var nombre = HttpContext.Session.GetString("nombreUsuario");

        if (nombre == null)
        {
            return RedirectToAction("Index", "Login");
        }


        ViewData["UsuarioLogueado"] = nombre;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
