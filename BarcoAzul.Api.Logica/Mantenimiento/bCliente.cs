using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bCliente : bComun
    {
        public bCliente(oDatosUsuario datosUsuario, IConnectionManager connectionManager) : base(connectionManager, origen: "Cliente", datosUsuario) { }

        public async Task<bool> Registrar(ClienteDTO model)
        {
            try
            {
                var cliente = Mapping.Mapper.Map<oCliente>(model);

                cliente.ProcesarDatos();
                cliente.UsuarioId = _datosUsuario.Id;

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dCliente dCliente = new(GetConnectionString());
                    cliente.Id = model.Id = await dCliente.GetNuevoId();

                    await dCliente.Registrar(cliente);

                    var clienteDireccion = new oClienteDireccion
                    {
                        ClienteId = cliente.Id,
                        Direccion = cliente.DireccionPrincipal,
                        DepartamentoId = cliente.DepartamentoId,
                        ProvinciaId = cliente.ProvinciaId,
                        DistritoId = cliente.DistritoId,
                        Comentario = "DIRECCION PRINCIPAL",
                        IsActivo = true,
                        TipoDireccionId = "01"
                    };

                    dClienteDireccion dClienteDireccion = new(GetConnectionString());
                    cliente.DireccionPrincipalId = await dClienteDireccion.Registrar(clienteDireccion);

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

        public async Task<bool> Modificar(ClienteDTO model)
        {
            try
            {
                var cliente = Mapping.Mapper.Map<oCliente>(model);

                cliente.ProcesarDatos();
                cliente.UsuarioId = _datosUsuario.Id;

                dCliente dCliente = new(GetConnectionString());

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dCliente.Modificar(cliente);

                    var clienteDireccion = new oClienteDireccion
                    {
                        ClienteId = cliente.Id,
                        Direccion = cliente.DireccionPrincipal,
                        DepartamentoId = cliente.DepartamentoId,
                        ProvinciaId = cliente.ProvinciaId,
                        DistritoId = cliente.DistritoId,
                        Comentario = "DIRECCION PRINCIPAL",
                        IsActivo = true,
                        TipoDireccionId = "01"
                    };

                    dClienteDireccion dClienteDireccion = new(GetConnectionString());
                    clienteDireccion.Id = await dClienteDireccion.GetDireccionPrincipalId(cliente.Id) ?? -1; //No actualizar ningún registro

                    await dClienteDireccion.Modificar(clienteDireccion);

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

        public async Task<bool> Eliminar(string id)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dClienteDireccion dClienteDireccion = new(GetConnectionString());
                    await dClienteDireccion.EliminarDeCliente(id);

                    dClienteContacto dClienteContacto = new(GetConnectionString());
                    await dClienteContacto.EliminarDeCliente(id);

                    dClientePersonal dClientePersonal = new(GetConnectionString());
                    await dClientePersonal.EliminarDeCliente(id);

                    dCliente dCliente = new(GetConnectionString());
                    await dCliente.Eliminar(id);

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

        public async Task<oCliente> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dCliente dCliente = new(GetConnectionString());
                var cliente = await dCliente.GetPorId(id);

                dClienteDireccion dClienteDireccion = new(GetConnectionString());
                cliente.DireccionPrincipalId = await dClienteDireccion.GetDireccionPrincipalId(cliente.Id);

                if (incluirReferencias)
                {
                    cliente.TipoDocumentoIdentidad = await new dTipoDocumentoIdentidad(GetConnectionString()).GetPorId(cliente.TipoDocumentoIdentidadId);

                    if (!string.IsNullOrWhiteSpace(cliente.DepartamentoId))
                        cliente.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(cliente.DepartamentoId);

                    if (!string.IsNullOrWhiteSpace(cliente.ProvinciaId))
                        cliente.Provincia = await new dProvincia(GetConnectionString()).GetPorId(cliente.DepartamentoId + cliente.ProvinciaId);

                    if (!string.IsNullOrWhiteSpace(cliente.DistritoId))
                        cliente.Distrito = await new dDistrito(GetConnectionString()).GetPorId(cliente.DepartamentoId + cliente.ProvinciaId + cliente.DistritoId);

                    cliente.Direcciones = await dClienteDireccion.ListarPorCliente(cliente.Id);
                    cliente.Contactos = await new dClienteContacto(GetConnectionString()).ListarPorCliente(cliente.Id);
                    cliente.Personal = await new dClientePersonal(GetConnectionString()).ListarPorCliente(cliente.Id);
                }

                return cliente;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vCliente>> Listar(string numeroDocumentoIdentidad, string nombre, oPaginacion paginacion)
        {
            try
            {
                dCliente dCliente = new(GetConnectionString());
                return await dCliente.Listar(numeroDocumentoIdentidad ?? string.Empty, nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dCliente(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad) => await new dCliente(GetConnectionString()).DatosRepetidos(id, numeroDocumentoIdentidad);

        public async Task<object> FormularioTablas()
        {
            var tiposDocumentoIdentidad = await new dTipoDocumentoIdentidad(GetConnectionString()).ListarTodos();
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();
            var zonas = await new dZona(GetConnectionString()).ListarTodos();
            var tiposVenta = dTipoVentaCompra.ListarTodos();
            var tiposCobro = await new dTipoCobroPago(GetConnectionString()).ListarTodos();

            return new
            {
                tiposDocumentoIdentidad,
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos),
                zonas,
                tiposVenta,
                tiposCobro
            };
        }
    }
}
