using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenDebugController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenDebugController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("decode")]
        [AllowAnonymous]
        public IActionResult DecodeToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token no proporcionado");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var claims = jwtToken.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList();
                var headers = jwtToken.Header.Select(h => new { key = h.Key, value = h.Value?.ToString() }).ToList();

                // Verificar si está expirado
                var expiryClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
                bool isExpired = false;
                DateTime? expiryTime = null;

                if (expiryClaim != null)
                {
                    var unixTime = long.Parse(expiryClaim.Value);
                    expiryTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
                    isExpired = expiryTime < DateTime.UtcNow;
                }

                // Verificar el issuer y audience
                var issuerClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
                var audienceClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
                var configIssuer = _configuration["JWT:ValidIssuer"];
                var configAudience = _configuration["JWT:ValidAudience"];

                return Ok(new
                {
                    isValid = !isExpired,
                    isExpired,
                    expiryTime,
                    currentTimeUtc = DateTime.UtcNow,
                    validIssuer = configIssuer == issuerClaim,
                    configIssuer,
                    tokenIssuer = issuerClaim,
                    validAudience = configAudience == audienceClaim,
                    configAudience,
                    tokenAudience = audienceClaim,
                    headers,
                    claims
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al decodificar el token: {ex.Message}");
            }
        }

        [HttpGet("validate")]
        [AllowAnonymous]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token no proporcionado");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                try
                {
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                    return Ok(new
                    {
                        isValid = true,
                        username = principal.Identity?.Name,
                        claims = principal.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList()
                    });
                }
                catch (SecurityTokenExpiredException)
                {
                    return Ok(new { isValid = false, error = "Token expirado" });
                }
                catch (SecurityTokenInvalidAudienceException)
                {
                    return Ok(new { isValid = false, error = "Audience inválido" });
                }
                catch (SecurityTokenInvalidIssuerException)
                {
                    return Ok(new { isValid = false, error = "Issuer inválido" });
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    return Ok(new { isValid = false, error = "Clave de firma no encontrada" });
                }
                catch (Exception ex)
                {
                    return Ok(new { isValid = false, error = $"Error de validación: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error general: {ex.Message}");
            }
        }
    }
}