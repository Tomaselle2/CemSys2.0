using CemSys2.Models;

namespace CemSys2.Interface.Tramite
{
    public interface ITramiteBD
    {
        Task<int> RegistrarTramite(CemSys2.Models.Tramite tramite);
        Task<CemSys2.Models.Tramite> ConsultarTramite(int idTramite);
    }
}
