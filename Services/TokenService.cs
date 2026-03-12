using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gimapi.Dto.UsuarioDtos;
using Microsoft.IdentityModel.Tokens;

namespace Gimapi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(UsuarioDTO usuario)
        {
            // 1. Definición de los Claims (Información del usuario en el token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Role, usuario.RolNombre) // Crucial para [Authorize(Roles = "Admin")]
            };

          // 2. Obtención de la clave desde appsettings.json 
            var secretKey = _config["Jwt:Key"]
                ?? throw new Exception("La clave JWT no está configurada en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Creación del cuerpo del Token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), // Tiempo de vida del token
                signingCredentials: creds
            );

            // 4. Generación de la cadena final (Token serializado)
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}