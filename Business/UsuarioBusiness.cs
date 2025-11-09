using CemSys2.Data;
using CemSys2.DTO;
using CemSys2.Interface;
using CemSys2.Interface.Usuario;
using CemSys2.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CemSys2.Business
{
    public class UsuarioBusiness : IUsuarioBusiness
    {
        public readonly IUnitOfWork _unitOfWork;

        public UsuarioBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ModificarContrasenia(int idUsuario, string nuevaPass, string antiguaPass)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await ModificarContraseniaInternal(idUsuario, nuevaPass, antiguaPass);

            });
        }

        private async Task ModificarContraseniaInternal(int idUsuario, string nuevaPass, string antiguaPass)
        {
            // Ahora puedes debuggear paso a paso esta función
            var usuarioExistente = await _unitOfWork._usuarioBD.ConsultarUsuario(idUsuario);

            if (string.IsNullOrEmpty(nuevaPass))
                throw new ValidationException("La contraseña nueva no puede estar vacía.");

            if (string.IsNullOrEmpty(antiguaPass))
                throw new ValidationException("La contraseña actual no puede estar vacía.");

            if (!VerifyPassword(antiguaPass, usuarioExistente.Clave))
                throw new ValidationException("La contraseña actual es incorrecta");

            if (!IsPasswordStrong(nuevaPass))
            {
                throw new ValidationException("La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.");
            }

            if(nuevaPass.Equals(antiguaPass))
                throw new ValidationException("La contraseña nueva debe ser diferente de la anterior");

            usuarioExistente.Clave = HashPassword(nuevaPass);
            _unitOfWork._usuarioBD.ModificarUsuario(usuarioExistente);
        }

        private async Task ReemplazarContraseniaInternal(int idUsuario, string nuevaPass)
        {
            // Ahora puedes debuggear paso a paso esta función
            var usuarioExistente = await _unitOfWork._usuarioBD.ConsultarUsuario(idUsuario);

            if (string.IsNullOrEmpty(nuevaPass))
                throw new ValidationException("La contraseña nueva no puede estar vacía.");


            if (!IsPasswordStrong(nuevaPass))
            {
                throw new ValidationException("La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.");
            }

            usuarioExistente.Clave = HashPassword(nuevaPass);
            _unitOfWork._usuarioBD.ModificarUsuario(usuarioExistente);
        }


        private bool IsPasswordStrong(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch));
        }

        private static string HashPassword(string password)
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

        // Método de verificación compatible con tu HashPassword
        private bool VerifyPassword(string plainPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            try
            {
                // Extraer salt y hash almacenado
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedSubHash = parts[1];

                // Calcular hash de la contraseña proporcionada con el mismo salt
                var hashedInput = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: plainPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                // Comparación segura
                return SecureCompare(hashedInput, storedSubHash);
            }
            catch
            {
                return false;
            }
        }

        // Método de comparación segura
        private bool SecureCompare(string a, string b)
        {
            if (a == null || b == null)
                return false;

            // Comparación de tiempo constante
            int minLength = Math.Min(a.Length, b.Length);
            int maxLength = Math.Max(a.Length, b.Length);

            bool result = (a.Length == b.Length);

            for (int i = 0; i < minLength; i++)
            {
                result &= (a[i] == b[i]);
            }

            return result;
        }

        public async Task ModificarUsuario(DTO_Usuario usuario)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var usuarioExistente = await _unitOfWork._usuarioBD.ConsultarUsuario(usuario.Id);
                if (usuarioExistente == null)
                {
                    throw new Exception("Usuario no encontrado.");
                }

                if(string.IsNullOrEmpty(usuario.Nombre))
                    throw new ValidationException("El nombre no puede estar vacío.");

                if (string.IsNullOrEmpty(usuario.Usuario1))
                    throw new ValidationException("El nombre de usuario no puede estar vacío.");

                if (string.IsNullOrEmpty(usuario.Correo))
                    throw new ValidationException("El correo no puede estar vacío.");

                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.Usuario1 = usuario.Usuario1;
                usuarioExistente.Visibilidad = usuario.Visibilidad;
                usuarioExistente.Rol = usuario.Rol;
                _unitOfWork._usuarioBD.ModificarUsuario(usuarioExistente);
            });
        }

        public Task<Usuario> ObtenerUsuarioPorCorreo(string correo)
        {
            return _unitOfWork._usuarioBD.ObtenerUsuarioPorCorreo(correo);
        }

        public async Task ReemplazarContrasenia(int idUsuario, string nuevaPass)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await ReemplazarContraseniaInternal(idUsuario, nuevaPass);

            });
        }
    }
}
