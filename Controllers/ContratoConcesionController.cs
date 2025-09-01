using CemSys2.DTO.Concesiones;
using CemSys2.Interface.Concesiones;
using CemSys2.Interface.Introduccion;
using CemSys2.Models;
using CemSys2.ViewModel.ConcesionesViewModel;
using CemSys2.ViewModel.ContratoViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static CemSys2.Controllers.IntroduccionController;

namespace CemSys2.Controllers
{
    public class ContratoConcesionController : Controller
    {
        private readonly IConcesionesBusiness _concesionesBusiness;
        private readonly IIntroduccionBusiness _introduccionBusiness;


        public ContratoConcesionController(IConcesionesBusiness concesionesBusiness, IIntroduccionBusiness introduccionBusiness)
        {
            _concesionesBusiness = concesionesBusiness;
            _introduccionBusiness = introduccionBusiness;
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

                //metodo que recibe el parcelaId y buscar los datos de la parcela
                viewModel.DatosParcela = await _concesionesBusiness.DatosParcela(parcelaId);

            }
            catch(Exception ex)
            {
                viewModel.MensajeError = ex.Message;
            }

            return View(viewModel);
        }

        // Método para buscar contribuyente (AJAX)
        [HttpPost]
        public async Task<IActionResult> BuscarContribuyente([FromBody] BuscarContribuyenteRequest request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo))
                {
                    return Json(new { success = false, message = "DNI y sexo son obligatorios" });
                }

                Persona contribuyente = await _introduccionBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);

                if (contribuyente != null)
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = new
                        {
                            id = contribuyente.IdPersona,
                            nombre = contribuyente.Nombre,
                            apellido = contribuyente.Apellido,
                            dni = request.Dni, // Usar el DNI del request para mantener consistencia
                            sexo = contribuyente.Sexo,
                            celular = contribuyente.Celular,
                            correo = contribuyente.Correo,
                            domicilio = contribuyente.Domicilio,

                        }
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = (object)null,
                        dni = request.Dni, // Devolver el DNI aunque no se encuentre el contribuyente
                        sexo = request.Sexo,

                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Método para registrar nuevo contribuyente (AJAX)
        [HttpPost]
        public async Task<IActionResult> RegistrarContribuyente([FromBody] RegistrarContribuyenteRequest request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo) ||
                    string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Apellido) || string.IsNullOrEmpty(request.Domicilio))
                {
                    return Json(new { success = false, message = "Todos los campos son obligatorios" });
                }

                // Validar que no exista ya
                Persona contribuyenteExistente = await _introduccionBusiness.BuscarContribuyente(request.Dni.ToString(), request.Sexo);
                if (contribuyenteExistente != null)
                {
                    return Json(new { success = false, message = "El titular ya existe en el sistema" });
                }

                // Crear nuevo titular
                var nuevoTitular = new Persona
                {
                    Dni = request.Dni.ToString(),
                    Nombre = request.Nombre.Trim(),
                    Apellido = request.Apellido.Trim(),
                    Sexo = request.Sexo,
                    Celular = request.Celular?.Trim(),
                    Correo = request.Correo?.Trim(),
                    Domicilio = request.Domicilio.Trim(),
                };

                // Guardar en base de datos
                var TitularCreado = await _concesionesBusiness.RegistrarTitular(nuevoTitular);

                return Json(new
                {
                    success = true,
                    contribuyente = new
                    {
                        id = TitularCreado.IdPersona,
                        nombre = TitularCreado.Nombre,
                        apellido = TitularCreado.Apellido,
                        dni = request.Dni, // Usar el DNI del request
                        sexo = TitularCreado.Sexo,
                        celular = TitularCreado.Celular,
                        correo = TitularCreado.Correo,
                        domicilio = TitularCreado.Domicilio
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Clase para los requests AJAX
        public class BuscarContribuyenteRequest
        {
            public int? Dni { get; set; }
            public string Sexo { get; set; }
        }

        // Clase para los requests AJAX
        public class RegistrarContribuyenteRequest
        {
            public int? Dni { get; set; }
            public string Sexo { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Domicilio { get; set; }
            public string? Celular { get; set; }
            public string? Correo { get; set; }
        }


    }
}
