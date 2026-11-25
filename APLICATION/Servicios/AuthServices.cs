using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APLICATION.Servicios
{
    public class AuthServices
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IRolesRepository _roles;
        private readonly IConfiguration _config;

        public AuthServices(IUsuarioRepository usuarios, IRolesRepository roles, IConfiguration config)
        {
            _usuarios = usuarios;
            _roles = roles;
            _config = config;
        }

        public async Task<string?> LoginAsync(LoginDTOs dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return null;

            var usuarios = await _usuarios.ListarUsuarioAsync();
            var user = usuarios.FirstOrDefault(u =>
                u.NOMBRE_USUARIO == dto.Username && u.ESTADO == "Activo");
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PASSWORD_HASH))
                return null;

            var roles = await _roles.ListarRolesAsync();
            var rol = roles.FirstOrDefault(r => r.ID_ROL == user.ID_ROL);
            var rolNombre = rol?.NOMBRE_ROL ?? "Recepcion";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["Jwt:Key"] ?? "dev-secret-key-change-in-production-32chars-minimum!!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.NOMBRE_USUARIO ?? ""),
                new Claim(ClaimTypes.Role, rolNombre),
                new Claim("sucursal", user.ID_SUCURSAL.ToString()),
                new Claim("usuario", user.ID_USUARIO.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "SaludAnimal",
                audience: _config["Jwt:Audience"] ?? "SaludAnimal",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}