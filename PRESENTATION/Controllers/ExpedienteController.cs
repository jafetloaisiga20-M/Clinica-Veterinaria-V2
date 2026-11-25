using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRESENTATION.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpedienteController : ControllerBase
    {
        private readonly ExpedienteMedicoServices _service;

        public ExpedienteController(ExpedienteMedicoServices service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();
            return Ok(lista);
        }

        [HttpGet("paciente/{idPaciente}")]
        public async Task<IActionResult> PorPaciente(int idPaciente)
        {
            var lista = await _service.Listar();
            return Ok(lista.Where(e => e.ID_PACIENTE == idPaciente));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ExpedienteMedicoDTOs dto)
        {
            if (dto.ID_PACIENTE <= 0)
                return BadRequest(new { msj = "El paciente es obligatorio." });
            if (string.IsNullOrWhiteSpace(dto.DIAGNOSTICO))
                return BadRequest(new { msj = "El diagnostico es obligatorio." });

            await _service.Nuevo(dto);
            return StatusCode(201, "Expediente registrado (inmutable).");
        }

        // No se permite editar ni eliminar: el expediente es inalterable.
    }
}