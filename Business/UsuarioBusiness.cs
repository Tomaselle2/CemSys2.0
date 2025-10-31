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
                var usuarioExistente = await _unitOfWork._usuarioBD.ConsultarUsuario(idUsuario);

                if (string.IsNullOrEmpty(nuevaPass))
                    throw new ValidationException("La contraseña nueva no puede estar vacía.");

                if (string.IsNullOrEmpty(antiguaPass))
                    throw new ValidationException("La contraseña actual no puede estar vacía.");

                if (usuarioExistente.Clave != HashPassword(antiguaPass))
                    throw new ValidationException("La contraseña actual es incorrecta");

                usuarioExistente.Clave = HashPassword(nuevaPass);
                _unitOfWork._usuarioBD.ModificarUsuario(usuarioExistente);

            });
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
    }
}
