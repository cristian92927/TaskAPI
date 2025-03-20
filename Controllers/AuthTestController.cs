using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskAPI.Models;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {
        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "Este endpoint es público y no requiere autenticación" });
        }

        [HttpGet("secured")]
        [Authorize]
        public IActionResult SecuredEndpoint()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            return Ok(new
            {
                message = "¡Autenticación exitosa!",
                username = username,
                email = email,
                claims = User.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList()
            });
        }

        [HttpGet("debug")]
        [AllowAnonymous]
        public IActionResult DebugAuthHeader()
        {
            // Obtener todos los encabezados
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            // Verificar específicamente el encabezado Authorization
            string authHeader = Request.Headers["Authorization"].ToString();
            bool hasBearer = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

            return Ok(new
            {
                authorizationHeader = authHeader,
                hasBearer = hasBearer,
                bearerToken = hasBearer ? authHeader.Substring(7) : null,
                allHeaders = headers
            });
        }
    }
}
