

using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.Otros;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Utilidades.Extensiones;
using BarcoAzul.Api.Logica.Empresa;

namespace BarcoAzulApi.Configuracion
{
    public class AuthorizeActionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _menuId;
        private readonly IEnumerable<Enum> _permisos;

        public AuthorizeActionFilter(string menuId, UsuarioPermiso permiso)
        {
            _menuId = menuId;
            _permisos = permiso.GetFlags();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var datosUsuario = context.HttpContext.RequestServices.GetService<oDatosUsuario>();

            if (datosUsuario.TipoUsuarioId == Constantes.TipoUsuarioAdministrador)
                return;

            var bUsuarioPermiso = context.HttpContext.RequestServices.GetService<bUsuarioPermiso>();

            bool accionPermitida = false;

            foreach (var permiso in _permisos)
            {
                accionPermitida = await bUsuarioPermiso.IsPermitido(datosUsuario.Id, _menuId, (UsuarioPermiso)permiso);

                if (accionPermitida)
                    break;
            }

            if (!accionPermitida)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}
