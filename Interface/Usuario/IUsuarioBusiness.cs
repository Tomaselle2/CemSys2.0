using CemSys2.DTO;

namespace CemSys2.Interface.Usuario
{
    public interface IUsuarioBusiness
    {
        public Task ModificarUsuario(DTO_Usuario usuario);
        public Task ModificarContrasenia(int idUsuario, string nuevaPass, string antiguaPass);
    }

}
