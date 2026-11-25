using System.Threading.Tasks;
using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace PRESENTATION.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthServices _auth;

        public AuthController(AuthServices auth)
        {
            _auth = auth;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { msj = "Datos invalidos." });

            var token = await _auth.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { msj = "Usuario o contrasena incorrectos." });

            return Ok(new { token });
        }
    }
}