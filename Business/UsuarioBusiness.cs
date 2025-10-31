using CemSys2.DTO;
using CemSys2.Interface;
using CemSys2.Interface.Usuario;
using System.ComponentModel.DataAnnotations;
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

        public Task ModificarContrasenia(int idUsuario, string nuevaPass, string antiguaPass)
        {
            throw new NotImplementedException();
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
