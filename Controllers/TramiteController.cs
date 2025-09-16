using CemSys2.Interface.Tramite;
using CemSys2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CemSys2.Enumerable;
using CemSys2.Interface.Concesiones;

namespace CemSys2.Controllers
{
    public class TramiteController : Controller
    {
        private ITramiteBusiness _tramiteBusiness;
        private IConcesionesBusiness _concesionesBusiness;

        public TramiteController(ITramiteBusiness tramiteBusiness, IConcesionesBusiness concesionesBusiness)
        {
            _tramiteBusiness = tramiteBusiness;
            _concesionesBusiness = concesionesBusiness;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IrATramite(int tramiteId)
        {
            try
            {
               var tramite = await _tramiteBusiness.ConsultarTramite(tramiteId);

                switch (tramite.TipoTramiteId)
                {
                    case (int)TipotamiteEmun.Introduccion: 
                        return RedirectToAction("ResumenIntroduccion", "Introduccion", new { tramiteId = tramite.Id });

                    case (int)TipotamiteEmun.ContratoDeConcesion:
                       var contrato = await _concesionesBusiness.ConsultarContratoConcesion(tramiteId);
                        return RedirectToAction("ContratoIniciado", "ContratoConcesion", new { nroConcesion = contrato.Concesion, parcelaId = contrato.ParcelaId });

                    default:
                        return BadRequest($"Tipo de trámite no soportado: {tramite.TipoTramiteId}");
                }

            } catch (Exception ex) {

               return BadRequest("algo fallo" + ex.Message);
            }    
        }
    }
}
