using CemSys2.DTO;
using CemSys2.Interface.Usuario;
using CemSys2.Models;

namespace CemSys2.Data
{
    public class UsuarioBD : IUsuarioBD
    {
        private readonly AppDbContext _context;

        public UsuarioBD(AppDbContext appDbContext) {
            _context = appDbContext;
        }

        public void ModificarUsuario(Usuario usuario)
        {
            _context.Update(usuario);
        }

        public async Task<Usuario> ConsultarUsuario (int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}
