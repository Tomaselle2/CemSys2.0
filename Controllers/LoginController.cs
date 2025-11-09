using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using CemSys2.ViewModel.Login;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepositoryBusiness<Usuario> _usuarioRepositoryBusiness;
        private readonly IConfiguration _configuration;

        public LoginController(IRepositoryBusiness<Usuario> usuarioRepositoryBusiness, IConfiguration configuration)
        {
            _usuarioRepositoryBusiness = usuarioRepositoryBusiness;
            _configuration = configuration;
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
                if ((model.NombreUsuario == usuario.Usuario1 || model.NombreUsuario == usuario.Correo)
                   && VerifyPassword(model.Clave, usuario.Clave!))
                {
                    HttpContext.Session.SetString("nombreUsuario", usuario.Nombre);
                    HttpContext.Session.SetInt32("Rol", usuario.Rol);
                    HttpContext.Session.SetInt32("idUsuario", usuario.Id);
                    HttpContext.Session.SetString("IsAuthenticated", "true");

                    return RedirectToAction("Index", "Home");
                }

                //usuario fijo
                if (model.NombreUsuario == "tomaselle2" && model.Clave == "1234")
                {
                    HttpContext.Session.SetString("nombreUsuario", "Admin Temporal");
                    HttpContext.Session.SetInt32("Rol", (int)RolUsuario.Encargado);
                    HttpContext.Session.SetInt32("idUsuario", 999);
                    HttpContext.Session.SetString("IsAuthenticated", "true");

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

        [HttpGet]
        public IActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarPassword(CorreoRecuperacionVM viewModel)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                string EmailOrigen = emailSettings["SenderEmail"];
                string Password = emailSettings["SenderPassword"];
                string SmtpServer = emailSettings["SmtpServer"];
                int Port = int.Parse(emailSettings["Port"]);

                using (MailMessage oMailMessage = new MailMessage(EmailOrigen, viewModel.correo,
                       "Recuperar Contraseña", "<p>Mensaje de prueba</p>"))
                {
                    oMailMessage.IsBodyHtml = true;

                    using (SmtpClient oSmtpClient = new SmtpClient(SmtpServer, Port))
                    {
                        oSmtpClient.Credentials = new NetworkCredential(EmailOrigen, Password);
                        oSmtpClient.EnableSsl = true;
                        oSmtpClient.UseDefaultCredentials = false;

                        await oSmtpClient.SendMailAsync(oMailMessage);
                    }
                }

                ViewBag.Mensaje = "Correo enviado correctamente";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
