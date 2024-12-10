using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.Atributos;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Empresa;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Repositorio.Configuracion;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Empresa
{
    public class bUsuario : bComun, ILogicaService
    {
        public bUsuario(oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Usuario", datosUsuario) { }

        public async Task<bool> Registrar(UsuarioRegistrarDTO model)
        {
            try
            {
                var usuario = Mapping.Mapper.Map<oUsuario>(model);

                usuario.ProcesarDatos();
                usuario.UsuarioAutorizadorId = _datosUsuario.Id;

                dUsuario dUsuario = new(GetConnectionString());
                usuario.Id = model.Id = await dUsuario.GetNuevoId();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dUsuario.Registrar(usuario);
                    await dUsuario.EncriptarClave(usuario.Id);

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

        public async Task<bool> Modificar(UsuarioModificarDTO model)
        {
            try
            {
                var usuario = Mapping.Mapper.Map<oUsuario>(model);

                usuario.ProcesarDatos();
                usuario.UsuarioAutorizadorId = _datosUsuario.Id;

                dUsuario dUsuario = new(GetConnectionString());
                await dUsuario.Modificar(usuario);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<bool> Eliminar(string id)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dUsuarioPermiso dUsuarioPermiso = new(GetConnectionString());
                    await dUsuarioPermiso.EliminarDeCliente(id);

                    dUsuario dUsuario = new(GetConnectionString());
                    await dUsuario.Eliminar(id);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oUsuario> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dUsuario dUsuario = new(GetConnectionString());
                var usuario = await dUsuario.GetPorId(id);

                if (incluirReferencias)
                {
                    usuario.TipoUsuario = dTipoUsuario.GetPorId(usuario.TipoUsuarioId);

                    if (!string.IsNullOrWhiteSpace(usuario.PersonalId))
                        usuario.Personal = await new dPersonal(GetConnectionString()).GetPorId(usuario.PersonalId);
                }

                return usuario;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vUsuario>> Listar(string nick, oPaginacion paginacion)
        {
            try
            {
                dUsuario dUsuario = new(GetConnectionString());
                return await dUsuario.Listar(nick ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dUsuario(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string nick) => await new dUsuario(GetConnectionString()).DatosRepetidos(id, nick);

        public async Task<bool> CambiarClave(oUsuarioCambiarClave usuarioCambiarClave)
        {
            try
            {
                usuarioCambiarClave.UsuarioId = _datosUsuario.Id;

                //Validar
                dSesion dSesion = new(GetConnectionString());
                string contraseniaAnterior = await dSesion.ProcesarClave(dSesion.AccionClave.Desencriptar, usuarioCambiarClave.UsuarioId);

                if (usuarioCambiarClave.ClaveAnterior != contraseniaAnterior)
                    throw new MensajeException(new oMensaje(MensajeTipo.Error, "La clave anterior es incorrecta."));

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dUsuario dUsuario = new(GetConnectionString());
                    await dUsuario.CambiarClave(usuarioCambiarClave);
                    await dUsuario.EncriptarClave(usuarioCambiarClave.UsuarioId);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<object> FormularioTablas()
        {
            var personal = await new dPersonal(GetConnectionString()).ListarTodos();

            return new
            {
                personal
            };
        }
    }
}
