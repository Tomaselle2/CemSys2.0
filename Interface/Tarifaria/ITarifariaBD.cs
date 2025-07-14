using CemSys2.DTO;
using CemSys2.Models;
namespace CemSys2.Interface.Tarifaria
{
    public interface ITarifariaBD
    {
        Task RegistrarTarifaria(CemSys2.Models.Tarifaria modelo);
        Task<int> ModificarTarifaria(CemSys2.Models.Tarifaria modelo);
        Task<List<CemSys2.Models.Tarifaria>> EmitirListadoTarifaria(); 
        Task<CemSys2.Models.Tarifaria> ConsultarTarifaria(int id);
        Task<List<PreciosTarifaria>> ConsultarPrecioTarifaria(int id);
        Task RegistrarConceptoTarifaria(ConceptosTarifaria nuevoConcepto); 
        Task ActualizarPreciosTarifaria(List<PrecioActualizarDto> preciosActualizar);

    }
}
