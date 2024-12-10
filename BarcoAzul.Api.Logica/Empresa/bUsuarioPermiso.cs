using BarcoAzul.Api.Modelos.Atributos;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Empresa;
using BarcoAzul.Api.Repositorio.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Empresa
{
    public class bUsuarioPermiso : bComun, ILogicaService
    {
        public bUsuarioPermiso(IConnectionManager connectionManager) : base(connectionManager, origen: "Usuario - Permisos") { }

        public async Task<bool> Registrar(oUsuarioConfiguracionPermisos usuarioConfiguracionPermisos)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dUsuarioPermiso = new dUsuarioPermiso(GetConnectionString());
                    await dUsuarioPermiso.EliminarDeCliente(usuarioConfiguracionPermisos.UsuarioId);
                    await dUsuarioPermiso.Registrar(usuarioConfiguracionPermisos.UsuarioId, usuarioConfiguracionPermisos.TipoUsuarioId, usuarioConfiguracionPermisos.Permisos);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<oUsuarioConfiguracionPermisos> GetPermisos(string usuarioId)
        {
            try
            {
                var usuario = await new dUsuario(GetConnectionString()).GetPorId(usuarioId);

                if (usuario is null)
                    throw new MensajeException(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un usuario con el ID proporcionado."));

                var permisos = await new dUsuarioPermiso(GetConnectionString()).ListarPorUsuario(usuarioId);

                return new oUsuarioConfiguracionPermisos
                {
                    UsuarioId = usuario.Id,
                    TipoUsuarioId = usuario.TipoUsuarioId,
                    Permisos = permisos.ToList()
                };
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oUsuarioPermiso> GetPorUsuarioYMenu(string usuarioId, string menuId)
        {
            try
            {
                var usuario = await new dUsuario(GetConnectionString()).GetPorId(usuarioId);

                if (usuario is null)
                    throw new MensajeException(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un usuario con el ID proporcionado."));

                if (usuario.TipoUsuarioId == Constantes.TipoUsuarioAdministrador)
                {
                    return new oUsuarioPermiso
                    {
                        UsuarioId = usuarioId,
                        MenuId = menuId,
                        Registrar = true,
                        Modificar = true,
                        Eliminar = true,
                        Consultar = true,
                        Anular = true
                    };
                }

                return await new dUsuarioPermiso(GetConnectionString()).GetPorUsuarioYMenu(usuarioId, menuId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<bool> IsPermitido(string usuarioId, string menuId, UsuarioPermiso permiso) => await new dUsuarioPermiso(GetConnectionString()).IsPermitido(usuarioId, menuId, permiso);

        public static object FormularioTablas()
        {
            var tiposUsuario = dTipoUsuario.ListarTodos();

            return new
            {
                tiposUsuario
            };
        }
    }
}
