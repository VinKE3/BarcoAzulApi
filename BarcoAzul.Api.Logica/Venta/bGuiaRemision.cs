using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Informes.PDFs;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Repositorio.Venta;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Utilidades;
using MCWebAPI.Modelos.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Venta
{
    public class bGuiaRemision : bComun
    {
        public bGuiaRemision(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Guía de Remisión", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(GuiaRemisionDTO model)
        {
            try
            {
                dGuiaRemision dGuiaRemision = new(GetConnectionString());

                var guiaRemision = Mapping.Mapper.Map<oGuiaRemision>(model);

                guiaRemision.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                guiaRemision.TipoDocumentoId = model.TipoDocumentoId = dGuiaRemision.TipoDocumentoId;
                guiaRemision.Numero = model.Numero = await dGuiaRemision.GetNuevoNumero(guiaRemision.EmpresaId, guiaRemision.Serie);
                guiaRemision.UsuarioId = _datosUsuario.Id;
                guiaRemision.TipoCambio = (await new dTipoCambio(GetConnectionString()).GetPorId(guiaRemision.FechaEmision))?.PrecioVenta ?? 0;
                guiaRemision.PorcentajeIGV = _configuracionGlobal.DefaultPorcentajeIgv;

                guiaRemision.ProcesarDatos();
                guiaRemision.CompletarDatosDetalles();
                guiaRemision.CompletarDatosDocumentosRelacionados();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dGuiaRemision.Registrar(guiaRemision);

                    dGuiaRemisionDetalle dGuiaRemisionDetalle = new(GetConnectionString());
                    await dGuiaRemisionDetalle.Registrar(guiaRemision.Detalles);

                    if (guiaRemision.DocumentosRelacionados is not null && guiaRemision.DocumentosRelacionados.Any())
                    {
                        dGuiaRemisionDocumentoRelacionado dGuiaRemisionDocumentoRelacionado = new(GetConnectionString());
                        await dGuiaRemisionDocumentoRelacionado.Registrar(guiaRemision.DocumentosRelacionados);
                    }

                    await dGuiaRemision.ActualizarCantidadPendiente(guiaRemision.Id, Operacion.Aumentar);

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

        public async Task<bool> Modificar(GuiaRemisionDTO model)
        {
            try
            {
                dGuiaRemision dGuiaRemision = new(GetConnectionString());

                var guiaRemision = Mapping.Mapper.Map<oGuiaRemision>(model);

                guiaRemision.UsuarioId = _datosUsuario.Id;
                guiaRemision.TipoCambio = (await new dTipoCambio(GetConnectionString()).GetPorId(guiaRemision.FechaEmision))?.PrecioVenta ?? 0;
                guiaRemision.PorcentajeIGV = _configuracionGlobal.DefaultPorcentajeIgv;

                guiaRemision.ProcesarDatos();
                guiaRemision.CompletarDatosDetalles();
                guiaRemision.CompletarDatosDocumentosRelacionados();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dGuiaRemision.Modificar(guiaRemision);

                    dGuiaRemisionDetalle dGuiaRemisionDetalle = new(GetConnectionString());
                    await dGuiaRemisionDetalle.Modificar(guiaRemision.Detalles);

                    dGuiaRemisionDocumentoRelacionado dGuiaRemisionDocumentoRelacionado = new(GetConnectionString());
                    await dGuiaRemisionDocumentoRelacionado.Modificar(guiaRemision.Id, guiaRemision.DocumentosRelacionados);

                    await dGuiaRemision.ActualizarCantidadPendiente(guiaRemision.Id, Operacion.Disminuir);
                    await dGuiaRemision.ActualizarCantidadPendiente(guiaRemision.Id, Operacion.Aumentar);

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
                    dGuiaRemision dGuiaRemision = new(GetConnectionString());

                    await new dDocumentoVenta(GetConnectionString()).DeshacerVenta(id);
                    await dGuiaRemision.ActualizarEstadoDocumentoRelacionado(id);

                    dAbonoVenta dAbonoVenta = new(GetConnectionString());
                    await dAbonoVenta.Eliminar(id);

                    dGuiaRemisionDocumentoRelacionado dGuiaRemisionDocumentoRelacionado = new(GetConnectionString());
                    await dGuiaRemisionDocumentoRelacionado.EliminarDeGuiaRemision(id);

                    dGuiaRemisionDetalle dGuiaRemisionDetalle = new(GetConnectionString());
                    await dGuiaRemisionDetalle.EliminarDeGuiaRemision(id);

                    await dGuiaRemision.Eliminar(id);

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

        public async Task<bool> Anular(string id)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dGuiaRemision dGuiaRemision = new(GetConnectionString());

                    await new dDocumentoVenta(GetConnectionString()).DeshacerVenta(id);
                    await dGuiaRemision.ActualizarEstadoDocumentoRelacionado(id);

                    await dGuiaRemision.Anular(id);

                    dGuiaRemisionDetalle dGuiaRemisionDetalle = new(GetConnectionString());
                    await dGuiaRemisionDetalle.AnularDeGuiaRemision(id);

                    dGuiaRemisionDocumentoRelacionado dGuiaRemisionDocumentoRelacionado = new(GetConnectionString());
                    await dGuiaRemisionDocumentoRelacionado.EliminarDeGuiaRemision(id);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Anular);
                return false;
            }
        }

        public async Task<(string Nombre, byte[] Archivo)> Imprimir(string id)
        {
            try
            {
                var guiaRemision = await GetPorId(id, true);
                var rptPath = RptPath.RptGuiaRemisionPath;

                var pdf = new PDFGuiaRemision(guiaRemision, _configuracionGlobal, rptPath);

                return ($"{guiaRemision.TipoDocumentoId}-{guiaRemision.Serie}-{int.Parse(guiaRemision.Numero)}.pdf", pdf.Generar());
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return (string.Empty, null);
            }
        }

        public async Task<oGuiaRemision> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dGuiaRemision dGuiaRemision = new(GetConnectionString());
                var guiaRemision = await dGuiaRemision.GetPorId(id);

                dGuiaRemisionDetalle dGuiaRemisionDetalle = new(GetConnectionString());
                guiaRemision.Detalles = (await dGuiaRemisionDetalle.ListarPorGuiaRemision(guiaRemision.Id)).ToList();

                dGuiaRemisionDocumentoRelacionado dGuiaRemisionDocumentoRelacionado = new(GetConnectionString());
                guiaRemision.DocumentosRelacionados = (await dGuiaRemisionDocumentoRelacionado.ListarPorGuiaRemision(guiaRemision.Id)).ToList();

                if (incluirReferencias)
                {
                    guiaRemision.Cliente = await new dCliente(GetConnectionString()).GetPorId(guiaRemision.ClienteId);
                    guiaRemision.Personal = await new dPersonal(GetConnectionString()).GetPorId(guiaRemision.PersonalId);

                    if (!string.IsNullOrWhiteSpace(guiaRemision.EmpresaTransporteId))
                        guiaRemision.EmpresaTransporte = await new dEmpresaTransporte(GetConnectionString()).GetPorId(guiaRemision.EmpresaTransporteId);

                    if (!string.IsNullOrWhiteSpace(guiaRemision.ConductorId))
                        guiaRemision.Conductor = await new dConductor(GetConnectionString()).GetPorId(guiaRemision.ConductorId);

                    if (!string.IsNullOrWhiteSpace(guiaRemision.VehiculoId))
                        guiaRemision.Vehiculo = await new dVehiculo(GetConnectionString()).GetPorId(guiaRemision.VehiculoId);

                    guiaRemision.MotivoTraslado = await new dMotivoTraslado(GetConnectionString()).GetPorId(guiaRemision.MotivoTrasladoId);
                    guiaRemision.Moneda = dMoneda.GetPorId(guiaRemision.MonedaId);
                }

                return guiaRemision;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vGuiaRemision>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, string serie, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dGuiaRemision dGuiaRemision = new(GetConnectionString());
                return await dGuiaRemision.Listar(fechaInicio.Value, fechaFin.Value, clienteNombre ?? string.Empty, serie ?? string.Empty, _datosUsuario.PersonalId, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dGuiaRemision(GetConnectionString()).Existe(id);

        public async Task<bool> IsBloqueado(string id) => await new dGuiaRemision(GetConnectionString()).IsBloqueado(id);

        public async Task<bool> IsAnulado(string id) => await new dGuiaRemision(GetConnectionString()).IsAnulado(id);

        public async Task<bool> StockSuficiente(GuiaRemisionDTO guiaRemision)
        {
            var detallesAValidar = guiaRemision.Detalles.Select(x => new oArticuloValidarStock
            {
                Id = x.LineaId + x.SubLineaId + x.ArticuloId,
                Descripcion = x.Descripcion,
                StockSolicitado = x.Cantidad,
                IsIngreso = false,
                DocumentoVentaCompraId = Comun.IsVentaIdValido(guiaRemision.Id) ? guiaRemision.Id : string.Empty
            });

            return await StockSuficiente(detallesAValidar);
        }

        public async Task<object> FormularioTablas()
        {
            var series = Mapping.Mapper.Map<IEnumerable<oTipoDocumentoSerie>>(await new dCorrelativo(GetConnectionString()).ListarTodos(new string[] { dGuiaRemision.TipoDocumentoId }));
            var vendedores = await new dPersonal(GetConnectionString()).ListarTodos();
            var empresasTransporte = await new dEmpresaTransporte(GetConnectionString()).ListarTodos();
            var conductores = await new dConductor(GetConnectionString()).ListarTodos();
            var vehiculos = await new dVehiculo(GetConnectionString()).ListarTodos();
            var motivosTraslado = await new dMotivoTraslado(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var tipos = new[]
            {
                new { Texto = "+", Valor = "+" },
                new { Texto = "-", Valor = "-" },
            };

            return new
            {
                series,
                vendedores,
                empresasTransporte,
                conductores,
                vehiculos,
                motivosTraslado,
                monedas,
                tipos
            };
        }
    }
}
