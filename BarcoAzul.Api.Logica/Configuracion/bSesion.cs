using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Configuracion;
using BarcoAzul.Api.Repositorio.Empresa;

namespace BarcoAzul.Api.Logica.Configuracion
{
    public class bSesion : bComun
    {
        private oUsuario _usuario = null!;
        public oUsuario Usuario { get { return _usuario; } }

        public bSesion(IConnectionManager connectionManager) : base(connectionManager) { }

        public async Task<bool> UsuarioValido(oSesionUsuario sesionUsuario)
        {
            try
            {
                dSesion dSesion = new(GetConnectionString());
                var usuarioValido = await dSesion.UsuarioValido(sesionUsuario);

                dUsuario dUsuario = new(GetConnectionString());
                _usuario = await dUsuario.GetPorId(dSesion.UsuarioId);

                return usuarioValido;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex);
                return false;
            }
        }

        public async Task<oRefreshToken> GetRefreshTokenPorUsuarioId(string usuarioId)
        {
            try
            {
                dSesion dSesion = new(GetConnectionString());
                return await dSesion.GetRefreshTokenPorUsuarioId(usuarioId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex);
                return null;
            }
        }

        public async Task ActualizarDatosRefreshToken(oRefreshToken refreshToken)
        {
            try
            {
                dSesion dSesion = new(GetConnectionString());
                await dSesion.ActualizarDatosRefreshToken(refreshToken);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex);
            }
        }

        public async Task RevocarRefreshToken(string usuarioId)
        {
            try
            {
                dSesion dSesion = new(GetConnectionString());
                await dSesion.RevocarRefreshToken(usuarioId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex);
                throw;
            }
        }
    }
}
