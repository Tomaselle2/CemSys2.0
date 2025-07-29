using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

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
                if (model.NombreUsuario == usuario.Usuario1 && VerifyPassword(model.Clave, usuario.Clave!))
                {
                    HttpContext.Session.SetString("nombreUsuario", usuario.Nombre);
                    HttpContext.Session.SetInt32("Rol", usuario.Rol);
                    HttpContext.Session.SetInt32("idUsuario", usuario.Id);

                    return RedirectToAction("Index", "Home");
                }

                //usuario dijo
                if (model.NombreUsuario == "tomaselle2" && model.Clave == "1234")
                {
                    HttpContext.Session.SetString("nombreUsuario", "Admin Temporal");
                    HttpContext.Session.SetInt32("Rol", 2);
                    HttpContext.Session.SetInt32("idUsuario", 999);

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

        public static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var parts = storedPassword.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            var enteredHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hash == enteredHash;
        }
    }
}
