using CemSys2.Interface.Introduccion;
using CemSys2.Models;

namespace CemSys2.Data
{
    public class IntroduccionBD : IIntroduccionBD
    {
        public Task<int> RegistrarActaDefuncion(ActaDefuncion model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarDifunto(Persona model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarHistorialEstadoTramite(HistorialEstadoTramite model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarIntroduccion(Introduccione model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarParcelaDifunto(ParcelaDifunto model)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarTramite(Tramite model)
        {
            throw new NotImplementedException();
        }
    }
}
