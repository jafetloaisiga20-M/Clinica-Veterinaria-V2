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
    public class ClienteController : ControllerBase
    {
        public readonly ClienteServices _clienteServices;

        public ClienteController(ClienteServices clienteServices)
        {
            _clienteServices = clienteServices;

        }


        [HttpGet]
        public async Task<IActionResult> ListarClietes()
        {
            try
            {
                var li= await _clienteServices.ListarClietes();
                return Ok(li);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [AllowAnonymous]
        [HttpPost("NuevoCliente")]

        public async Task<IActionResult> Nuevocliente(ClienteDTOs clienteDTOs)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(new { msj= "esa mrd no es valida"});
                }
                var id = await _clienteServices.NuevoCliente(clienteDTOs);
                return StatusCode(201, new { id, msj = "Cliente agregado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
