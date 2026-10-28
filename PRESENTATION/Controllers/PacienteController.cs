using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRESENTATION.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        public readonly PacienteServices _services;

        public PacienteController(PacienteServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPaciente()
        {
            try
            {
                var li = await _services.ListarPaciente();
                return Ok(li);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "entonce q pedo eso esta mal" + ex.Message);
            }
        }

        [HttpPost("Nueva cita")]
        public async Task<IActionResult> NuevoPaciente(PacienteDTOs dTOs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "papi nada llore mejor" });
                }
                await _services.NuevoPaciente(dTOs);
                return StatusCode(201, "Paciente agregada puto");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "nada papito malisimo volve a estudiar");
            }
        }
    }
}
