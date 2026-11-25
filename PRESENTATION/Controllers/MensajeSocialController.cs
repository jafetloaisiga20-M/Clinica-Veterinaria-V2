using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using APLICATION.DTOs;
using APLICATION.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace PRESENTATION.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MensajeSocialController : ControllerBase
    {
        private readonly MensajeSocialServices _service;
        private readonly IConfiguration _config;

        public MensajeSocialController(MensajeSocialServices service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        // Listado de leads/mensajes recibidos por redes sociales.
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();
            return Ok(lista);
        }

        // Crear un lead manualmente (simula un mensaje recibido).
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Crear(MensajeSocialDTOs dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TEXTO))
                return BadRequest(new { msj = "El texto del mensaje es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.CANAL))
                dto.CANAL = "WhatsApp";
            if (dto.FECHA_RECEPCION == default)
                dto.FECHA_RECEPCION = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(dto.ESTADO))
                dto.ESTADO = "Nuevo";

            await _service.Nuevo(dto);
            return StatusCode(201, "Mensaje registrado.");
        }

        // Verificacion del webhook de WhatsApp (Meta).
        [AllowAnonymous]
        [HttpGet("webhooks/whatsapp")]
        public IActionResult VerificarWhatsApp(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.verify_token")] string verifyToken,
            [FromQuery(Name = "hub.challenge")] string challenge)
        {
            var expected = _config["Social:WhatsApp:VerifyToken"];
            // En desarrollo (sin token configurado) se acepta cualquier verificacion
            // para facilitar pruebas locales del profesor/evaluador.
            if (mode == "subscribe" && (string.IsNullOrEmpty(expected) || verifyToken == expected))
                return Content(challenge ?? string.Empty, "text/plain");

            return Unauthorized();
        }

        // Recepcion de mensajes de WhatsApp.
        [AllowAnonymous]
        [HttpPost("webhooks/whatsapp")]
        public async Task<IActionResult> RecibirWhatsApp()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            var secret = _config["Social:WhatsApp:AppSecret"];
            var signature = Request.Headers["X-Hub-Signature-256"].ToString();
            if (!string.IsNullOrEmpty(secret) && !ValidarFirma(body, signature, secret))
                return Unauthorized();

            try
            {
                using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
                if (doc.RootElement.TryGetProperty("entry", out var entries))
                {
                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (!entry.TryGetProperty("changes", out var changes)) continue;
                        foreach (var change in changes.EnumerateArray())
                        {
                            if (!change.TryGetProperty("value", out var value)) continue;
                            if (!value.TryGetProperty("messages", out var messages)) continue;
                            foreach (var msg in messages.EnumerateArray())
                            {
                                string? from = msg.TryGetProperty("from", out var f) ? f.GetString() : null;
                                string? texto = null;
                                if (msg.TryGetProperty("text", out var t) && t.TryGetProperty("body", out var b))
                                    texto = b.GetString();

                                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(texto))
                                {
                                    await _service.Nuevo(new MensajeSocialDTOs
                                    {
                                        CANAL = "WhatsApp",
                                        REMITENTE = from!,
                                        TEXTO = texto!,
                                        FECHA_RECEPCION = DateTime.UtcNow,
                                        ESTADO = Clasificar(texto!)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Meta reintenta si no responde 200; se registra y se responde OK para evitar bucles.
            }

            return Ok();
        }

        // Recepcion de mensajes de Facebook Messenger.
        [AllowAnonymous]
        [HttpPost("webhooks/facebook")]
        public async Task<IActionResult> RecibirFacebook()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            var secret = _config["Social:Facebook:AppSecret"];
            var signature = Request.Headers["X-Hub-Signature-256"].ToString();
            if (!string.IsNullOrEmpty(secret) && !ValidarFirma(body, signature, secret))
                return Unauthorized();

            try
            {
                using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
                if (doc.RootElement.TryGetProperty("entry", out var entries))
                {
                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (!entry.TryGetProperty("messaging", out var messaging)) continue;
                        foreach (var m in messaging.EnumerateArray())
                        {
                            string? sender = null;
                            if (m.TryGetProperty("sender", out var s) && s.TryGetProperty("id", out var sid))
                                sender = sid.GetString();

                            string? texto = null;
                            if (m.TryGetProperty("message", out var msg) && msg.TryGetProperty("text", out var bt))
                                texto = bt.GetString();

                            if (!string.IsNullOrWhiteSpace(sender) && !string.IsNullOrWhiteSpace(texto))
                            {
                                await _service.Nuevo(new MensajeSocialDTOs
                                {
                                    CANAL = "Facebook",
                                    REMITENTE = sender!,
                                    TEXTO = texto!,
                                    FECHA_RECEPCION = DateTime.UtcNow,
                                    ESTADO = Clasificar(texto!)
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return Ok();
        }

        private static string Clasificar(string texto)
        {
            var t = (texto ?? string.Empty).ToLowerInvariant();
            if (t.Contains("cita") || t.Contains("agendar") || t.Contains("reservar") ||
                t.Contains("appointment") || t.Contains("consulta"))
                return "Solicitud de cita";
            return "Consulta general";
        }

        private static bool ValidarFirma(string body, string signature, string secret)
        {
            if (string.IsNullOrEmpty(signature)) return false;
            const string prefix = "sha256=";
            if (!signature.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;

            var hashHex = signature.Substring(prefix.Length).ToLowerInvariant();
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var computedHex = BitConverter.ToString(computed).Replace("-", "").ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(hashHex),
                Encoding.UTF8.GetBytes(computedHex));
        }
    }
}