using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Logica.Empresa;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BarcoAzulApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SesionController : GlobalController
    {
        private readonly bSesion _bSesion;
        private readonly IConfiguration _configuration;

        public SesionController(bSesion bSesion, IConfiguration configuration)
        {
            _bSesion = bSesion;
            _configuration = configuration;
        }

        [HttpPost(nameof(Iniciar))]
        public async Task<IActionResult> Iniciar(oSesionUsuario sesionUsuario)
        {
            if (ModelState.IsValid)
            {
                if (await _bSesion.UsuarioValido(sesionUsuario))
                {
                    var token = await GenerarToken(_bSesion.Usuario);
                    return Ok(token);
                }

                AgregarMensajes(_bSesion.Mensajes);
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPost(nameof(ActualizarToken))]
        public async Task<IActionResult> ActualizarToken(oToken model)
        {
            if (ModelState.IsValid)
            {
                var principal = GetPrincipalFromExpiredToken(model.Token);

                if (principal is null)
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"El token de acceso o de actualización es inválido."));
                    return BadRequest(GenerarRespuesta(false));
                }

                string usuarioId = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var refreshTokenActual = await _bSesion.GetRefreshTokenPorUsuarioId(usuarioId);

                if (refreshTokenActual is null || refreshTokenActual.Token != model.RefreshToken || refreshTokenActual.Expiracion <= DateTime.UtcNow)
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"El token de acceso o de actualización es inválido."));
                    return BadRequest(GenerarRespuesta(false));
                }

                bUsuario bUsuario = new(null, _bSesion.ConnectionManager);
                var usuario = await bUsuario.GetPorId(usuarioId);

                var token = await GenerarToken(usuario);
                return Ok(token);

            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(nameof(RevocarToken))]
        public async Task<IActionResult> RevocarToken()
        {
            string usuarioId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            await _bSesion.RevocarRefreshToken(usuarioId);

            return Ok(GenerarRespuesta(true));
        }

        private async Task<oRespuesta> GenerarToken(oUsuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                new Claim(ClaimTypes.Name, usuario.Nick),
                new Claim(ClaimTypes.Role, usuario.TipoUsuarioId),
                new Claim(nameof(usuario.PersonalId), usuario.PersonalId ?? string.Empty),
                new Claim(nameof(usuario.HabilitarAfectarStock), usuario.HabilitarAfectarStock.ToString())
            };

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:TokenValidityInMinutes"]));

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            var refreshToken = GenerarRefreshToken();

            await _bSesion.ActualizarDatosRefreshToken(new oRefreshToken
            {
                UsuarioId = usuario.Id,
                Token = refreshToken,
                Expiracion = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:RefreshTokenValidityInMinutes"]))
            });

            return new oRespuesta
            {
                Success = true,
                Data = new oRespuestaToken
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                    Expiration = securityToken.ValidTo,
                    RefreshToken = refreshToken
                }
            };
        }

        private string GenerarRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                Mensajes.Add(new oMensaje(MensajeTipo.Error, "Token inválido."));
                return null;
            }

            return principal;

        }
    }
}