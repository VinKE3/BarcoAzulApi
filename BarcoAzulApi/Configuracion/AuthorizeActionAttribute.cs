using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzulApi.Configuracion
{
    public class AuthorizeActionAttribute : TypeFilterAttribute
    {
        public AuthorizeActionAttribute(string menuId, UsuarioPermiso permiso) : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { menuId, permiso };
        }
    }
}
