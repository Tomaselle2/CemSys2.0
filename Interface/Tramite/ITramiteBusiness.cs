namespace CemSys2.Interface.Tramite
{
    public interface ITramiteBusiness
    {
        Task<int> RegistrarTramite(CemSys2.Models.Tramite tramite);
        Task<CemSys2.Models.Tramite> ConsultarTramite(int idTramite);
    }
}
