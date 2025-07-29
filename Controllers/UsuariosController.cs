using CemSys2.Business;
using CemSys2.Interface;
using CemSys2.Models;
using CemSys2.ViewModel;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace CemSys2.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IRepositoryBusiness<Usuario> _usuarioRepositoryBusiness;
        private readonly IRepositoryBusiness<RolesUsuario> _tipoUsuarioRepositoryBusiness;
        private const int CANTIDAD_POR_PAGINA = 20;

        public UsuariosController(IRepositoryBusiness<Usuario> usuarioRepositoryBusiness, IRepositoryBusiness<RolesUsuario> tipoUsuarioRepositoryBusiness)
        {
            _usuarioRepositoryBusiness = usuarioRepositoryBusiness;
            _tipoUsuarioRepositoryBusiness = tipoUsuarioRepositoryBusiness;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            if (HttpContext.Session.GetInt32("idUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }


            // Validar parámetros
            if (pagina < 1) pagina = 1;

            UsuariosViewModel viewModel = new();

            await ListarUsuarios( viewModel, pagina);

            return View(viewModel);
        }

        private async Task ListarUsuarios(UsuariosViewModel viewModel, int pagina = 1)
        {
            try
            {
                await CargarCombo(viewModel);

                // Definir filtro una sola vez
                Expression<Func<Usuario, bool>> filtro = s => s.Visibilidad == true;
                //Func<IQueryable<Seccione>, IOrderedQueryable<Seccione>> orderBy = q => q.OrderByDescending(s => s.Id);

                // Obtener total de registros
                int totalRegistros = await _usuarioRepositoryBusiness.ContarTotalAsync(filtro);
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)CANTIDAD_POR_PAGINA);

                // Ajustar página si es mayor al total
                if (pagina > totalPaginas && totalPaginas > 0)
                    pagina = totalPaginas;

                viewModel.ListaUsuarios = await _usuarioRepositoryBusiness.ObtenerPaginadoAsync(pagina, CANTIDAD_POR_PAGINA, filtro);
                viewModel.PaginaActual = pagina;
                viewModel.TotalPaginas = totalPaginas;
                viewModel.TotalRegistros = totalRegistros;
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
        }

        private async Task CargarCombo(UsuariosViewModel model)
        {
            try
            {
                model.ListaRolesUsuarios = await _tipoUsuarioRepositoryBusiness.EmitirListado();
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
            }
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }


        [HttpPost]
        public async Task<IActionResult> Registrar(UsuariosViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.EsEdicion) // Es una edición
                {
                    // Cargar el usuario existente para obtener la contraseña actual si no se cambió
                    var usuarioExistente = await _usuarioRepositoryBusiness.Consultar(model.Id.Value);
                    usuarioExistente.Nombre = model.Nombre;
                    usuarioExistente.Correo = model.Correo;
                    usuarioExistente.Usuario1 = model.NombreUsuario; // Asumiendo que Usuario1 es el nombre de usuario
                    usuarioExistente.Visibilidad = model.Visibilidad ?? true;
                    usuarioExistente.Rol = model.Rol.Value;

                    if (!string.IsNullOrEmpty(model.Clave))
                    {
                        usuarioExistente.Clave = model.Clave;
                        usuarioExistente.Clave = HashPassword(model.Clave); // Hashear la nueva contraseña
                    }

                    int modificacion = await _usuarioRepositoryBusiness.Modificar(usuarioExistente);
                    TempData["MensajeExito"] = "Usuario actualizado correctamente.";
                    model.EsEdicion = false; // Resetear el estado de edición
                }
                else // Es un registro nuevo
                {
                    Usuario usuario = new Usuario
                    {
                        Id = model.Id ?? 0, // Si es nuevo, Id será 0
                        Nombre = model.Nombre,
                        Correo = model.Correo,
                        Usuario1 = model.NombreUsuario,
                        Visibilidad = model.Visibilidad ?? true,
                        Rol = model.Rol.Value
                    };

                    usuario.Clave = HashPassword(model.Clave!); //  hashear la contraseña al crear
                    await _usuarioRepositoryBusiness.Registrar(usuario);
                    TempData["MensajeExito"] = "Usuario registrado correctamente.";
                }
                return RedirectToAction("Index");
            }

            await ListarUsuarios(model); // Vuelve a cargar listas si hay errores de validación
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(UsuariosViewModel model)
        {
            try
            {
                Usuario modelo = await _usuarioRepositoryBusiness.Consultar(model.Id.Value);
                modelo.Visibilidad = false;
                await _usuarioRepositoryBusiness.Modificar(modelo);
                TempData["MensajeExito"] = "Usuario eliminado correctamente";
            }
            catch (Exception ex)
            {
                model.MensajeError = ex.Message;
                return RedirectToAction(model.Redirigir);
            }
            return RedirectToAction(model.Redirigir);
        }

        [HttpGet]
        public async Task<IActionResult> Modificar(UsuariosViewModel model)
        {
            Usuario modelo = await _usuarioRepositoryBusiness.Consultar(model.Id.Value);
            model.Id = modelo.Id;
            model.Nombre = modelo.Nombre;
            model.Correo = modelo.Correo;
            model.NombreUsuario = modelo.Usuario1;
            model.Visibilidad = modelo.Visibilidad;
            model.Rol = modelo.Rol;
            await ListarUsuarios(model);
            model.EsEdicion = true;
            ModelState.Clear();

            return View("Index", model);
        }

    }
}
