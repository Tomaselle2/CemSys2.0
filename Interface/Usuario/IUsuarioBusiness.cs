using CemSys2.DTO;

namespace CemSys2.Interface.Usuario
{
    public interface IUsuarioBusiness
    {
        public Task ModificarUsuario(DTO_Usuario usuario);
        public Task ModificarContrasenia(int idUsuario, string nuevaPass, string antiguaPass);
        public Task ReemplazarContrasenia(int idUsuario, string nuevaPass);
        Task<CemSys2.Models.Usuario> ObtenerUsuarioPorCorreo(string correo);

    }

}
