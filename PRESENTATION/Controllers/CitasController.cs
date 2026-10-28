using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRESENTATION.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        public readonly CitasServices _services;

        public CitasController(CitasServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult>ListarCitas()
        {
            try
            {
                var li= await _services.ListarCitas();
                return Ok(li);   
            }
            catch (Exception ex) { 
            
                return StatusCode(500,"entonce q pedo eso esta mal" +ex.Message);
            }
        }

        [HttpPost("Nueva cita")]
        public async Task<IActionResult> NuevoCitas(CitasDTOs citasDTOs)
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                  return BadRequest(new {msj ="papi nada llore mejor"});
                }
                await _services.Nuevacita(citasDTOs);
                return StatusCode(201, "Cita agregada puto");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "nada papito malisimo volve a estudiar");
            }
        }
    }
}
