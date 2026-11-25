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
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [AllowAnonymous]
        [HttpPost("NuevaCita")]
        public async Task<IActionResult> NuevoCitas(CitasDTOs citasDTOs)
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                  return BadRequest(new {msj ="papi nada llore mejor"});
                }
                var id = await _services.Nuevacita(citasDTOs);
                return StatusCode(201, new { id, msj = "Cita agregada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { msj = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
