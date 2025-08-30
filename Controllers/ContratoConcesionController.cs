using CemSys2.DTO.Concesiones;
using CemSys2.Interface.Concesiones;
using CemSys2.ViewModel.ConcesionesViewModel;
using CemSys2.ViewModel.ContratoViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys2.Controllers
{
    public class ContratoConcesionController : Controller
    {
        private readonly IConcesionesBusiness _concesionesBusiness;

        public ContratoConcesionController(IConcesionesBusiness concesionesBusiness)
        {
            _concesionesBusiness = concesionesBusiness;
        }

        public async Task<IActionResult> Index()
        {
            IndexConcesionesVM viewModel= new IndexConcesionesVM();
            try
            {
                viewModel.ListaParcelasSinContrato = await _concesionesBusiness.ListaParcelasSinContrato();
            }
            catch (Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ContratoConcesion(int parcelaId)
        {
            GenerarContratoVM viewModel = new GenerarContratoVM();

            try
            {
                //metodo que recibe el parcelaId y buscar los difuntos en esa parcela
                viewModel.DifuntosEnParcela = await _concesionesBusiness.ListaDifuntosPorParcela(parcelaId);

            }catch(Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }

            return View(viewModel);
        }

       
    }
}
