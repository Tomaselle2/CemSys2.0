using CemSys2.Models;
namespace CemSys2.Interface.Tarifaria
{
    public interface ITarifariaBD
    {
        Task RegistrarTarifaria(CemSys2.Models.Tarifaria modelo);//usando
        Task<int> ModificarTarifaria(CemSys2.Models.Tarifaria modelo);
        Task<List<CemSys2.Models.Tarifaria>> EmitirListadoTarifaria(); //usando
        Task<CemSys2.Models.Tarifaria> ConsultarTarifaria(int id); //usando


        Task<int> RegistrarTipoConceptoTarifaria(TiposConceptoTarifarium modelo);

        Task<int> RegistrarConceptoTarifaria(ConceptosTarifaria modelo); //usando
        Task<List<ConceptosTarifaria>> EmitirListadoConceptoTarifaria();//usando
        Task<ConceptosTarifaria> ConsultarConceptoTarifaria(int id);//usando
        Task<int> ModificarConceptoTarifaria(ConceptosTarifaria modelo);//usando

        Task<List<AniosConcesion>> EmitirListadoAniosConcesion();

        Task<int> RegistrarPrecioTarifaria(PreciosTarifaria modelo);
        Task<List<PreciosTarifaria>> EmitirListadoPrecioTarifaria(PreciosTarifaria modelo);
        Task<List<PreciosTarifaria>> ConsultarPrecioTarifaria(int id);
        Task<int> ModificarPrecioTarifaria(PreciosTarifaria modelo);

    }
}
