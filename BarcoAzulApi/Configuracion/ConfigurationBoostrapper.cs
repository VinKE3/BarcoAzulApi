using BarcoAzul.Api.Logica.Empresa;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;

namespace BarcoAzulApi.Configuracion
{
    public static class ConfigurationBoostrapper
    {
        public static async Task ConfigureCors(this IServiceCollection services)
        {
            var connectionManager = services.BuildServiceProvider().GetRequiredService<IConnectionManager>();
            bOrigen bOrigen = new(connectionManager);
            var origenes = await bOrigen.Listar();

            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(origenes.ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Content-Disposition");
                });
            });
        }

        public static async Task ConfigureDatosEstaticos(this IServiceCollection services)
        {
            var connectionManager = services.BuildServiceProvider().GetRequiredService<IConnectionManager>();
            bConfiguracionGlobal bConfiguracionGlobal = new(connectionManager);
            var configuracionGlobal = await bConfiguracionGlobal.Get();

            services.AddSingleton(configuracionGlobal);
        }

        public static void ConfigureDatosUsuario(this IServiceCollection services)
        {
            services.AddScoped<bUsuarioPermiso>();
            services.AddScoped(x => DatosUsuarioFactory.Get(x.GetService(typeof(IHttpContextAccessor)) as HttpContextAccessor));
        }

        public static async Task RevisarMenus(this IServiceCollection services)
        {
            var bMenu = services.BuildServiceProvider().GetRequiredService<bMenu>();

            var menus = NombresMenus.Listar();
            var menusNuevos = new List<oMenu>();

            foreach (var menu in menus)
            {
                if (!await bMenu.Existe(menu))
                {
                    menusNuevos.Add(new oMenu
                    {
                        Id = menu,
                        Nombre = menu,
                        SistemaAreaId = Constantes.DefaultSistemaArea,
                        IsActivo = true
                    });
                }
            }

            if (menusNuevos.Count > 0)
                await bMenu.Registrar(menusNuevos);
        }
    }
}
