using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Compra;
using BarcoAzul.Api.Repositorio.Empresa;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Utilidades;
using BarcoAzul.Api.Utilidades.Extensiones;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Compra
{
    public class bDocumentoCompra : bComun
    {
        public bDocumentoCompra(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Documento de Compra", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(DocumentoCompraDTO model)
        {
            try
            {
                var documentoCompra = Mapping.Mapper.Map<oDocumentoCompra>(model);

                documentoCompra.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                documentoCompra.ClienteId = model.ClienteId = _configuracionGlobal.DefaultClienteId;

                if (await Existe(documentoCompra.Id))
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el ID ingresado."));
                    return false;
                }

                documentoCompra.UsuarioId = _datosUsuario.Id;
                documentoCompra.PersonalId = _configuracionGlobal.DefaultPersonalId;
                documentoCompra.ProcesarDatos();
                documentoCompra.CompletarDatosDetalles();
                documentoCompra.CompletarDatosOrdenesCompraRelacionadas();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
                    await dDocumentoCompra.Registrar(documentoCompra);

                    dDocumentoCompraDetalle dDocumentoCompraDetalle = new(GetConnectionString());
                    await dDocumentoCompraDetalle.Registrar(documentoCompra.Detalles);

                    if (documentoCompra.OrdenesCompraRelacionadas is not null && documentoCompra.OrdenesCompraRelacionadas.Count > 0)
                    {
                        dDocumentoCompraOrdenCompraRelacionada dDocumentoCompraOrdenCompraRelacionada = new(GetConnectionString());
                        await dDocumentoCompraOrdenCompraRelacionada.Registrar(documentoCompra.OrdenesCompraRelacionadas);
                    }

                    decimal montoAbonado = documentoCompra.TipoCompraId == "CO" ? documentoCompra.Total : 0;
                    await AbonarCompra(documentoCompra, montoAbonado);

                    if (documentoCompra.TipoDocumentoId == "07")
                        await AbonarConNotaCredito(documentoCompra, documentoCompra.Total);

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

        public async Task<bool> Modificar(DocumentoCompraDTO model)
        {
            try
            {
                var documentoCompra = Mapping.Mapper.Map<oDocumentoCompra>(model);

                documentoCompra.UsuarioId = _datosUsuario.Id;
                documentoCompra.ProcesarDatos();
                documentoCompra.CompletarDatosDetalles();
                documentoCompra.CompletarDatosOrdenesCompraRelacionadas();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
                    await dDocumentoCompra.Modificar(documentoCompra);

                    dDocumentoCompraDetalle dDocumentoCompraDetalle = new(GetConnectionString());
                    await dDocumentoCompraDetalle.Modificar(documentoCompra.Detalles);

                    dDocumentoCompraOrdenCompraRelacionada dDocumentoCompraOrdenCompraRelacionada = new(GetConnectionString());
                    await dDocumentoCompraOrdenCompraRelacionada.Modificar(documentoCompra.Id, documentoCompra.OrdenesCompraRelacionadas);

                    //if (documentoCompra.TipoDocumentoId == "01")
                    //{
                    //    dOrdenCompra dOrdenCompra = new(GetConnectionString());
                    //    await dOrdenCompra.ActualizarCantidadPendiente(documentoCompra.Id, Operacion.Disminuir);
                    //    await dOrdenCompra.ActualizarCantidadPendiente(documentoCompra.Id, Operacion.Aumentar);
                    //}

                    decimal montoAbonado = documentoCompra.TipoCompraId == "CO" ? documentoCompra.Total : 0;
                    await AbonarCompra(documentoCompra, montoAbonado);

                    if (documentoCompra.TipoCompraId == "CO")
                        await dDocumentoCompra.ActualizarCampoTipoCompraId(documentoCompra.Id);

                    if (documentoCompra.TipoDocumentoId == "07")
                        await AbonarConNotaCredito(documentoCompra, documentoCompra.Total);

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
                var splitId = dDocumentoCompra.SplitId(id);

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
                    dAbonoCompra dAbonoCompra = new(GetConnectionString());

                    await dDocumentoCompra.DeshacerCompra(id);

                    if (splitId.TipoDocumentoId == "07")
                    {
                        var (documentoReferenciaId, abonoId) = await dDocumentoCompra.GetDatosDocumentoReferencia(id);

                        if (abonoId.HasValue && abonoId != 0)
                        {
                            await dAbonoCompra.Eliminar(documentoReferenciaId, abonoId);
                        }
                    }

                    dOrdenCompra dOrdenCompra = new(GetConnectionString());
                    //await dOrdenCompra.ActualizarCantidadPendiente(id, Operacion.Disminuir);

                    await dDocumentoCompra.ActualizarEstadoOrdenCompraRelacionado(id);

                    await dAbonoCompra.Eliminar(id);

                    dDocumentoCompraOrdenCompraRelacionada dDocumentoCompraOrdenCompraRelacionada = new(GetConnectionString());
                    await dDocumentoCompraOrdenCompraRelacionada.EliminarDeDocumentoCompra(id);

                    dDocumentoCompraDetalle dDocumentoCompraDetalle = new(GetConnectionString());
                    await dDocumentoCompraDetalle.EliminarDeDocumentoCompra(id);

                    await dDocumentoCompra.Eliminar(id);

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

        public async Task<oDocumentoCompra> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
                var documentoCompra = await dDocumentoCompra.GetPorId(id);

                dDocumentoCompraDetalle dDocumentoCompraDetalle = new(GetConnectionString());
                documentoCompra.Detalles = (await dDocumentoCompraDetalle.ListarPorDocumentoCompra(id)).ToList();

                dDocumentoCompraOrdenCompraRelacionada dDocumentoCompraOrdenCompraRelacionada = new(GetConnectionString());
                documentoCompra.OrdenesCompraRelacionadas = (await dDocumentoCompraOrdenCompraRelacionada.ListarPorDocumentoCompra(id)).ToList();

                if (incluirReferencias)
                {
                    documentoCompra.TipoDocumento = await new dTipoDocumento(GetConnectionString()).GetPorId(documentoCompra.TipoDocumentoId);
                    documentoCompra.Proveedor = await new dProveedor(GetConnectionString()).GetPorId(documentoCompra.ProveedorId);
                    documentoCompra.TipoCompra = dTipoVentaCompra.GetPorId(documentoCompra.TipoCompraId);
                    documentoCompra.TipoPago = await new dTipoCobroPago(GetConnectionString()).GetPorId(documentoCompra.TipoPagoId);
                    documentoCompra.Moneda = dMoneda.GetPorId(documentoCompra.MonedaId);

                    if (!string.IsNullOrWhiteSpace(documentoCompra.CuentaCorrienteId))
                        documentoCompra.CuentaCorriente = await new dCuentaCorriente(GetConnectionString()).GetPorId(documentoCompra.CuentaCorrienteId);

                    if (documentoCompra.TipoDocumentoId == "07" || documentoCompra.TipoDocumentoId == "08")
                        documentoCompra.MotivoNota = await new dMotivoNota(GetConnectionString()).GetPorId(documentoCompra.TipoDocumentoId, documentoCompra.MotivoNotaId);
                }

                return documentoCompra;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vDocumentoCompra>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
                return await dDocumentoCompra.Listar(GetTiposDocumentoPermitidos(), fechaInicio.Value, fechaFin.Value, proveedorNombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oPagina<vDocumentoCompraPendiente>> ListarPendientes(DateTime? fechaInicio, DateTime? fechaFin, string proveedorId, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
                return await dDocumentoCompra.ListarPendientes(fechaInicio.Value, fechaFin.Value, proveedorId, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dDocumentoCompra(GetConnectionString()).Existe(id);

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id) => await new dDocumentoCompra(GetConnectionString()).IsBloqueado(id);

        public async Task<object> FormularioTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(new string[] { "01", "02", "03", "04", "07", "08", "12", "14", "20", "RS", "TA", "55", "RR" });
            var tiposCompra = dTipoVentaCompra.ListarTodos();
            var tiposPago = await new dTipoCobroPago(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var porcentajesIGV = await new dEmpresaIGV(GetConnectionString()).ListarTodos();
            var motivosNota = await new dMotivoNota(GetConnectionString()).ListarTodos();
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();
            var porcentajesPercepcion = await new dEmpresaPercepcion(GetConnectionString()).ListarTodos();
            return new
            {
                tiposDocumento,
                tiposCompra,
                tiposPago,
                monedas,
                porcentajesIGV,
                porcentajesPercepcion,
                motivosNota,
                cuentasCorrientes
            };
        }

        private async Task AbonarCompra(oDocumentoCompra documentoCompra, decimal montoAbonado)
        {
            dAbonoCompra dAbonoCompra = new(GetConnectionString());
            await dAbonoCompra.Eliminar(documentoCompra.Id);

            if (montoAbonado > 0)
            {
                var montoPEN = documentoCompra.MonedaId == "S" ? montoAbonado : decimal.Round(decimal.Multiply(montoAbonado, documentoCompra.TipoCambio), 2, MidpointRounding.AwayFromZero);
                var montoUSD = documentoCompra.MonedaId == "S" ? decimal.Round(decimal.Divide(montoAbonado, documentoCompra.TipoCambio), 2, MidpointRounding.AwayFromZero) : montoAbonado;

                var abonoCompra = new oAbonoCompra
                {
                    EmpresaId = documentoCompra.EmpresaId,
                    ProveedorId = documentoCompra.ProveedorId,
                    TipoDocumentoId = documentoCompra.TipoDocumentoId,
                    Serie = documentoCompra.Serie,
                    Numero = documentoCompra.Numero,
                    AbonoId = 1,
                    Fecha = documentoCompra.FechaEmision,
                    MonedaId = documentoCompra.MonedaId,
                    TipoCambio = documentoCompra.TipoCambio,
                    Monto = montoAbonado,
                    MontoPEN = montoPEN,
                    MontoUSD = montoUSD,
                    DocumentoCompraId = documentoCompra.Id.Mid(8, 16),
                    UsuarioId = documentoCompra.UsuarioId,
                    TipoPagoId = documentoCompra.TipoPagoId,
                    CuentaCorrienteId = documentoCompra.CuentaCorrienteId?.Mid(2, 2),
                    NumeroOperacion = documentoCompra.NumeroOperacion,
                    Concepto = "AL CONTADO",
                    IsBloqueado = false
                };

                await dAbonoCompra.Registrar(abonoCompra);
            }
        }

        private async Task AbonarConNotaCredito(oDocumentoCompra documentoCompra, decimal montoAbonado)
        {
            dDocumentoCompra dDocumentoCompra = new(GetConnectionString());
            dAbonoCompra dAbonoCompra = new(GetConnectionString());

            var (documentoReferenciaId, documentoReferenciaAbonoId) = await dDocumentoCompra.GetDatosDocumentoReferencia(documentoCompra.Id);
            documentoReferenciaId = Comun.IsCompraIdValido(documentoReferenciaId) ? documentoReferenciaId : documentoReferenciaId + _configuracionGlobal.DefaultClienteId;

            await dAbonoCompra.Eliminar(documentoReferenciaId, documentoReferenciaAbonoId);

            if (montoAbonado > 0 && documentoCompra.Abonar)
            {
                var montoPEN = documentoCompra.MonedaId == "S" ? montoAbonado : decimal.Round(decimal.Multiply(montoAbonado, documentoCompra.TipoCambio), 2, MidpointRounding.AwayFromZero);
                var montoUSD = documentoCompra.MonedaId == "S" ? decimal.Round(decimal.Divide(montoAbonado, documentoCompra.TipoCambio), 2, MidpointRounding.AwayFromZero) : montoAbonado;
                var abonoId = await dAbonoCompra.GetNuevoId(documentoReferenciaId);

                var abonoCompra = new oAbonoCompra
                {
                    EmpresaId = documentoCompra.DocumentoReferenciaId.Mid(0, 2),
                    ProveedorId = documentoCompra.DocumentoReferenciaId.Mid(2, 6),
                    TipoDocumentoId = documentoCompra.DocumentoReferenciaId.Mid(8, 2),
                    Serie = documentoCompra.DocumentoReferenciaId.Mid(10, 4),
                    Numero = documentoCompra.DocumentoReferenciaId.Mid(14, 10),
                    AbonoId = abonoId,
                    Fecha = documentoCompra.FechaEmision,
                    MonedaId = documentoCompra.MonedaId,
                    TipoCambio = documentoCompra.TipoCambio,
                    Monto = montoAbonado,
                    MontoPEN = montoPEN,
                    MontoUSD = montoUSD,
                    DocumentoCompraId = documentoCompra.Id.Mid(8, 16),
                    UsuarioId = documentoCompra.UsuarioId,
                    TipoPagoId = documentoCompra.TipoPagoId,
                    CuentaCorrienteId = documentoCompra.CuentaCorrienteId?.Mid(2, 2),
                    NumeroOperacion = documentoCompra.NumeroOperacion,
                    Concepto = $"PAGO CON NOTA DE CREDITO N° {documentoCompra.Serie}-{documentoCompra.Numero.Right(8)}",
                    IsBloqueado = true
                };

                await dAbonoCompra.Registrar(abonoCompra);
                await dDocumentoCompra.ActualizarCampoDocumentoReferenciaAbonoId(documentoCompra.Id, abonoId);
            }
            else
            {
                await dDocumentoCompra.ActualizarCampoDocumentoReferenciaAbonoId(documentoCompra.Id, 0);
            }
        }

        private static string[] GetTiposDocumentoPermitidos() => new string[] { "01", "02", "03", "04", "07", "08", "12", "14", "NV", "PR", "CR", "CV" };
    }
}
