using CemSys2.Interface;
using CemSys2.Models;
using Microsoft.AspNetCore.Mvc;
using CemSys2.ViewModel;

namespace CemSys2.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepositoryBusiness<Usuario> _usuarioRepositoryBusiness;
        public LoginController(IRepositoryBusiness<Usuario> usuarioRepositoryBusiness)
        {
            _usuarioRepositoryBusiness = usuarioRepositoryBusiness;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("idUsuario") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            List<Usuario> usuarios = await _usuarioRepositoryBusiness.EmitirListado();

            foreach (var usuario in usuarios)
            {
                if (model.NombreUsuario == usuario.Usuario1 && model.Clave == usuario.Clave)
                {
                    HttpContext.Session.SetString("nombreUsuario", usuario.Nombre);
                    return RedirectToAction("Index", "Home");
                }
            }

            model.MensajeError = "Usuario o contraseña incorrecta";
            model.Clave = ""; // Limpiar la contraseña por seguridad
            return View("Index", model);
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
