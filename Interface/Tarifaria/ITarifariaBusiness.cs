using CemSys2.Models;

namespace CemSys2.Interface.Tarifaria
{
    public interface ITarifariaBusiness
    {
        Task<int> RegistrarTarifaria(CemSys2.Models.Tarifaria modelo);
        Task<int> ModificarTarifaria(CemSys2.Models.Tarifaria modelo);
        Task<List<CemSys2.Models.Tarifaria>> EmitirListadoTarifaria();
        Task<CemSys2.Models.Tarifaria> ConsultarTarifaria(int id);

        Task<int> RegistrarTipoConceptoTarifaria(TiposConceptoTarifarium modelo);

        Task<int> RegistrarConceptoTarifaria(ConceptosTarifaria modelo);
        Task<List<ConceptosTarifaria>> EmitirListadoConceptoTarifaria();
        Task<ConceptosTarifaria> ConsultarConceptoTarifaria(int id);
        Task<int> ModificarConceptoTarifaria(ConceptosTarifaria modelo);

        Task<List<AniosConcesion>> EmitirListadoAniosConcesion();

        Task<int> RegistrarPrecioTarifaria(PreciosTarifaria modelo);
        Task<List<PreciosTarifaria>> EmitirListadoPrecioTarifaria(PreciosTarifaria modelo);
        Task<PreciosTarifaria> ConsultarPrecioTarifaria(int id);
        Task<int> ModificarPrecioTarifaria(PreciosTarifaria modelo);
    }
}
