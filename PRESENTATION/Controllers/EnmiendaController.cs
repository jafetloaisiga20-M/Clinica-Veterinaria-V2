using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRESENTATION.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EnmiendaController : ControllerBase
    {
        private readonly EnmiendaMedicaServices _service;

        public EnmiendaController(EnmiendaMedicaServices service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();
            return Ok(lista);
        }

        [HttpGet("expediente/{idExpediente}")]
        public async Task<IActionResult> PorExpediente(int idExpediente)
        {
            var lista = await _service.Listar();
            return Ok(lista.Where(e => e.ID_EXPEDIENTE == idExpediente));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(EnmiendaDTOs dto)
        {
            if (dto.ID_EXPEDIENTE <= 0)
                return BadRequest(new { msj = "El expediente es obligatorio." });
            if (string.IsNullOrWhiteSpace(dto.NOTA_ADICIONAL))
                return BadRequest(new { msj = "La nota adicional es obligatoria." });

            await _service.Nueva(dto);
            return StatusCode(201, "Enmienda agregada (no editable).");
        }

        // No se permite editar ni eliminar: las enmiendas son anexas e inalterables.
    }
}