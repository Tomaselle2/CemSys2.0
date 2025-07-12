using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys2.Business
{
    public class TarifariaBusiness : ITarifariaBusiness
    {
        private readonly ITarifariaBD _tarifariaBD;
        public TarifariaBusiness(ITarifariaBD tarifariaBD)
        {
            _tarifariaBD = tarifariaBD;
        }

        //tarifaria------------------------
        public async Task RegistrarTarifaria(Tarifaria modelo)
        {
            try
            {
                await _tarifariaBD.RegistrarTarifaria(modelo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la tarifaria", ex);
            }
        }

        public async Task<int> ModificarTarifaria(Tarifaria modelo)
        {
            try
            {
                return await _tarifariaBD.ModificarTarifaria(modelo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la tarifaria", ex);
            }
        }

        public async Task<List<Tarifaria>> EmitirListadoTarifaria()
        {
            try
            {
                return await _tarifariaBD.EmitirListadoTarifaria();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al emitir listado de las tarifarias", ex);
            }
        }

        public async Task<Tarifaria> ConsultarTarifaria(int id)
        {
            try
            {
                return await _tarifariaBD.ConsultarTarifaria(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la tarifaria", ex);
            }
        }
        //tarifaria------------------------


        //precios tarifaria------------------------
        public async Task<List<PreciosTarifaria>> ConsultarPrecioTarifaria(int id)
        {
            return await _tarifariaBD.ConsultarPrecioTarifaria(id);
        }
        //precios tarifaria------------------------


        public Task<ConceptosTarifaria> ConsultarConceptoTarifaria(int id)
        {
            throw new NotImplementedException();
        }



        public Task<List<AniosConcesion>> EmitirListadoAniosConcesion()
        {
            throw new NotImplementedException();
        }

        public Task<List<ConceptosTarifaria>> EmitirListadoConceptoTarifaria()
        {
            throw new NotImplementedException();
        }

        public Task<List<PreciosTarifaria>> EmitirListadoPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

        public Task<int> ModificarConceptoTarifaria(ConceptosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

        public Task<int> ModificarPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }


        public Task<int> RegistrarConceptoTarifaria(ConceptosTarifaria modelo)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegistrarPrecioTarifaria(PreciosTarifaria modelo)
        {
            throw new NotImplementedException();
        }


        public Task<int> RegistrarTipoConceptoTarifaria(TiposConceptoTarifarium modelo)
        {
            throw new NotImplementedException();
        }

 
    }
}
