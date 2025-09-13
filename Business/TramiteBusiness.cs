using CemSys2.Interface.Tramite;
using CemSys2.Models;

namespace CemSys2.Business
{
    public class TramiteBusiness : ITramiteBusiness
    {
        private readonly ITramiteBD _tramiteBD;

        public TramiteBusiness(ITramiteBD tramiteBD)
        {
            _tramiteBD = tramiteBD;
        }

        public Task<Tramite> ConsultarTramite(int idTramite)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RegistrarTramite(Tramite tramite)
        {
            return await _tramiteBD.RegistrarTramite(tramite);
        }
    }
}
