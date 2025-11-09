using CemSys2.DTO;
using CemSys2.Models;   

namespace CemSys2.Interface.Usuario
{
    public interface IUsuarioBD
    {
        public void ModificarUsuario(CemSys2.Models.Usuario usuario);
        Task<CemSys2.Models.Usuario> ConsultarUsuario(int id);
        Task<CemSys2.Models.Usuario> ObtenerUsuarioPorCorreo(string correo);
    }
}
