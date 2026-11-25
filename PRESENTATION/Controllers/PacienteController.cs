using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRESENTATION.Controllers
{
    [Authorize]
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
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [AllowAnonymous]
        [HttpPost("NuevaCita")]
        public async Task<IActionResult> NuevoPaciente(PacienteDTOs dTOs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "papi nada llore mejor" });
                }
                var id = await _services.NuevoPaciente(dTOs);
                return StatusCode(201, new { id, msj = "Paciente agregado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
