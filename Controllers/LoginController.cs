using CemSys2.Enumerable;
using CemSys2.Interface;
using CemSys2.Interface.Usuario;
using CemSys2.Models;
using CemSys2.ViewModel;
using CemSys2.ViewModel.Login;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepositoryBusiness<Usuario> _usuarioRepositoryBusiness;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioBusiness _usuarioBusiness;


        public LoginController(IRepositoryBusiness<Usuario> usuarioRepositoryBusiness, IConfiguration configuration, IUsuarioBusiness usuarioBusiness)
        {
            _usuarioRepositoryBusiness = usuarioRepositoryBusiness;
            _configuration = configuration;
            _usuarioBusiness = usuarioBusiness;
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
        public IActionResult EnviarCorreoRecuperacion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarCorreoRecuperacion(CorreoRecuperacionVM viewModel)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                string EmailOrigen = emailSettings["SenderEmail"];
                string Password = emailSettings["SenderPassword"];
                string SmtpServer = emailSettings["SmtpServer"];
                int Port = int.Parse(emailSettings["Port"]);

                // Construimos la URL absoluta para el botón
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                string actionUrl = $"{baseUrl}/Login/RecuperarClave";

                // --- cuerpo del correo HTML ---
                string bodyHtml = $@"
                    <div style='font-family: Arial, sans-serif; color: #333; text-align: center;'>
                        <h2>Recuperación de Contraseña</h2>
                        <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
            
                        <img src='https://raw.githubusercontent.com/Tomaselle2/CemSys2.0/main/wwwroot/fotos/cemsysss.png' alt='Logo' style='max-width:200px; margin:20px auto; display:block;' />

                        <p>Haz clic en el siguiente botón para restablecer tu contraseña:</p>
            
                        <form method='get' action='{actionUrl}' style='display:inline-block; margin-top:20px;'>
                            <input type='hidden' name='correo' value='{viewModel.correo.Trim()}' />
                            <button type='submit' 
                                    style='background-color:#007bff; color:white; border:none; padding:12px 24px;
                                           border-radius:6px; cursor:pointer; font-size:16px; text-decoration:none;'>
                                Restablecer Contraseña
                            </button>
                        </form>

                        <p style='margin-top:40px; font-size:13px; color:#666;'>
                            Si no solicitaste este cambio, puedes ignorar este mensaje.
                        </p>
                    </div>";

                using (MailMessage oMailMessage = new MailMessage(EmailOrigen, viewModel.correo.Trim(),
                       "Recuperar Contraseña", bodyHtml))
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

                TempData["CorreoStatus"] = "success";
                TempData["CorreoMensaje"] = "Correo enviado correctamente. Revisa tu bandeja de entrada.";
            }
            catch (Exception ex)
            {
                TempData["CorreoStatus"] = "error";
                TempData["CorreoMensaje"] = $"No se pudo enviar el correo: {ex.Message}";
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult RecuperarClave(string correo)
        {
            CambiarClaveLoginVM viewModel = new CambiarClaveLoginVM
            {
                Correo = correo
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarClave(CambiarClaveLoginVM model)
        {
            try
            {
                Usuario usuario = await _usuarioBusiness.ObtenerUsuarioPorCorreo(model.Correo!);


                await _usuarioBusiness.ReemplazarContrasenia(usuario.Id, model.ClaveNueva!);

                // Mensaje de éxito en TempData
                TempData["CorreoStatus"] = "success";
                TempData["CorreoMensaje"] = "Recuperaste tu clave exitosamente"; 
                return RedirectToAction("Index", "Login");
            }
            catch (ValidationException ex)
            {
                // Mensaje de error de validación
                TempData["SweetAlertType"] = "warning";
                TempData["SweetAlertTitle"] = "Validación";
                TempData["SweetAlertMessage"] = ex.Message;
                return View("RecuperarClave", new {correo = model.Correo});
            }
            catch (Exception ex)
            {
                // Mensaje de error general
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "No se pudo cambiar la contraseña: " + ex.Message;
                return View("RecuperarClave", new { correo = model.Correo });
            }

        }
    }
}
