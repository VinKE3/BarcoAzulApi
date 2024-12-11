using BarcoAzul.Api.Modelos.Otros;
using System.Security.Claims;

namespace BarcoAzulApi.Configuracion
{
    public class DatosUsuarioFactory
    {
        public static oDatosUsuario Get(IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor?.HttpContext?.User?.Claims;

            return claims is null ? null : new oDatosUsuario
            {
                Id = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                Nick = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                TipoUsuarioId = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                PersonalId = claims.FirstOrDefault(x => x.Type == "PersonalId")?.Value
            };
        }
    }
}
