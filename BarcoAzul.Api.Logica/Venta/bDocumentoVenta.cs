using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Empresa;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Repositorio.Venta;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Utilidades.Extensiones;
using BarcoAzul.Api.Utilidades;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Venta
{
    public class bDocumentoVenta : bComun
    {
        public bDocumentoVenta(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Documento de Venta", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(DocumentoVentaDTO model)
        {
            try
            {
                dDocumentoVenta dDocumentoVenta = new(GetConnectionString());

                var documentoVenta = Mapping.Mapper.Map<oDocumentoVenta>(model);

                documentoVenta.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                documentoVenta.Numero = model.Numero = await dDocumentoVenta.GetNuevoNumero(documentoVenta.EmpresaId, documentoVenta.TipoDocumentoId, documentoVenta.Serie);
                documentoVenta.UsuarioId = _datosUsuario.Id;

                documentoVenta.ProcesarDatos();
                documentoVenta.CompletarDatosDetalles();
                documentoVenta.CompletarDatosCuotas();
                documentoVenta.CompletarDatosAnticipos();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dDocumentoVenta.Registrar(documentoVenta);

                    dDocumentoVentaDetalle dDocumentoVentaDetalle = new(GetConnectionString());
                    await dDocumentoVentaDetalle.Registrar(documentoVenta.Detalles);

                    if (documentoVenta.TipoVentaId == "CR" && documentoVenta.Cuotas is not null)
                    {
                        dDocumentoVentaCuota dDocumentoVentaCuota = new(GetConnectionString());
                        await dDocumentoVentaCuota.Registrar(documentoVenta.Cuotas);
                    }

                    if (documentoVenta.Anticipos is not null)
                    {
                        dDocumentoVentaAnticipo dDocumentoVentaAnticipo = new(GetConnectionString());
                        await dDocumentoVentaAnticipo.Registrar(documentoVenta.Anticipos);
                    }

                    //:TODO CONSULTAR A JOSEPH
                    //if (!string.IsNullOrWhiteSpace(documentoVenta.CotizacionId))
                    //{
                    //    var numeroDocumento = $"{Comun.GetTipoDocumentoAbreviatura(documentoVenta.TipoDocumentoId)}-{documentoVenta.Serie}-{documentoVenta.Numero.Right(8)}";
                    //    numeroDocumento = documentoVenta.TipoDocumentoId == "01" || documentoVenta.TipoDocumentoId == "03" ? numeroDocumento : numeroDocumento.Mid(3);

                    //    dCotizacion dCotizacion = new(GetConnectionString());
                    //    await dCotizacion.ActualizarDocumentoVentaRelacionado(documentoVenta.CotizacionId, documentoVenta.Id, numeroDocumento);
                    //}

                    decimal montoAbonado = documentoVenta.TipoVentaId == "CO" && documentoVenta.TipoCobroId != "CP" ? documentoVenta.Total - documentoVenta.MontoRetencion : 0;
                    await AbonarVenta(documentoVenta, montoAbonado);

                    if (documentoVenta.TipoDocumentoId == "07")
                        await AbonarConNotaCredito(documentoVenta, documentoVenta.Total);

                    //if (documentoVenta.TipoDocumentoId == "08")
                    //    await ActualizarLetraCambio(documentoVenta);

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

        public async Task<bool> Modificar(DocumentoVentaDTO model)
        {
            try
            {
                dDocumentoVenta dDocumentoVenta = new(GetConnectionString());

                var documentoVenta = Mapping.Mapper.Map<oDocumentoVenta>(model);

                documentoVenta.UsuarioId = _datosUsuario.Id;
                documentoVenta.ProcesarDatos();
                documentoVenta.CompletarDatosDetalles();
                documentoVenta.CompletarDatosCuotas();
                documentoVenta.CompletarDatosAnticipos();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dDocumentoVenta.Modificar(documentoVenta);

                    dDocumentoVentaDetalle dDocumentoVentaDetalle = new(GetConnectionString());
                    await dDocumentoVentaDetalle.Modificar(documentoVenta.Detalles);

                    dDocumentoVentaCuota dDocumentoVentaCuota = new(GetConnectionString());
                    await dDocumentoVentaCuota.Modificar(documentoVenta.Id, documentoVenta.Cuotas);

                    dDocumentoVentaAnticipo dDocumentoVentaAnticipo = new(GetConnectionString());
                    await dDocumentoVentaAnticipo.Modificar(documentoVenta.Id, documentoVenta.Anticipos);

                    decimal montoAbonado = documentoVenta.TipoVentaId == "CO" && documentoVenta.TipoCobroId != "CP" ? documentoVenta.Total - documentoVenta.MontoRetencion : 0;
                    await AbonarVenta(documentoVenta, montoAbonado);

                    if (documentoVenta.TipoDocumentoId == "07")
                        await AbonarConNotaCredito(documentoVenta, documentoVenta.Total);

                    //TODO: Función Actualizar Letra cuando el tipo de documento es 08

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
                    dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                    await dDocumentoVenta.Eliminar(id);

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
                    dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                    await dDocumentoVenta.Anular(id);

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

        //:TODO CONSULTAR A JOSEPH
        //public async Task<(string Nombre, byte[] Archivo)> Imprimir(string id)
        //{
        //    try
        //    {
        //        dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
        //        var documentoVenta = await dDocumentoVenta.GetPorId(id);

        //        if (documentoVenta.IsAnulado)
        //        {
        //            Mensajes.Add(new oMensaje(MensajeTipo.Error, $"{_origen}: El PDF no está disponible cuando el registro está anulado."));
        //            return (string.Empty, null);
        //        }

        //        dCPE dCPE = new(GetConnectionString());

        //        var cpe = await dCPE.Get(documentoVenta.EmpresaId, documentoVenta.TipoDocumentoId, documentoVenta.Serie, documentoVenta.Numero);
        //        var rptPath = RptPath.RptCPEGREPath;

        //        var pdfCPE = new PDFCPE(cpe, _configuracionGlobal.EmpresaNumeroDocumentoIdentidad, rptPath);

        //        return ($"{cpe.tipoDocumento}-{cpe.serie}-{cpe.numero}.pdf", pdfCPE.Generar());
        //    }
        //    catch (Exception ex)
        //    {
        //        ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
        //        return (string.Empty, null);
        //    }
        //}

        public async Task<bool> Enviar(oEnviarDocumentoVenta model)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                    await dDocumentoVenta.Enviar(model);

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

        public async Task<oDocumentoVenta> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                var documentoVenta = await dDocumentoVenta.GetPorId(id);

                dDocumentoVentaDetalle dDocumentoVentaDetalle = new(GetConnectionString());
                documentoVenta.Detalles = (await dDocumentoVentaDetalle.ListarPorDocumentoVenta(documentoVenta.Id)).ToList();

                dDocumentoVentaCuota dDocumentoVentaCuota = new(GetConnectionString());
                documentoVenta.Cuotas = (await dDocumentoVentaCuota.ListarPorDocumentoVenta(documentoVenta.Id)).ToList();

                dDocumentoVentaAnticipo dDocumentoVentaAnticipo = new(GetConnectionString());
                documentoVenta.Anticipos = (await dDocumentoVentaAnticipo.ListarPorDocumentoVenta(documentoVenta.Id)).ToList();

                if (incluirReferencias)
                {
                    documentoVenta.Cliente = await new dCliente(GetConnectionString()).GetPorId(documentoVenta.ClienteId);
                    documentoVenta.TipoDocumento = await new dTipoDocumento(GetConnectionString()).GetPorId(documentoVenta.TipoDocumentoId);
                    documentoVenta.Personal = await new dPersonal(GetConnectionString()).GetPorId(documentoVenta.PersonalId);
                    documentoVenta.Moneda = dMoneda.GetPorId(documentoVenta.MonedaId);
                    documentoVenta.TipoVenta = dTipoVentaCompra.GetPorId(documentoVenta.TipoVentaId);
                    documentoVenta.TipoCobro = await new dTipoCobroPago(GetConnectionString()).GetPorId(documentoVenta.TipoCobroId);

                    if (!string.IsNullOrWhiteSpace(documentoVenta.CuentaCorrienteId))
                        documentoVenta.CuentaCorriente = await new dCuentaCorriente(GetConnectionString()).GetPorId(documentoVenta.CuentaCorrienteId);

                    if (!string.IsNullOrWhiteSpace(documentoVenta.MotivoNotaId))
                        documentoVenta.MotivoNota = await new dMotivoNota(GetConnectionString()).GetPorId(documentoVenta.TipoDocumentoId, documentoVenta.MotivoNotaId);
                }

                return documentoVenta;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vDocumentoVenta>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, bool? isEnviado, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                return await dDocumentoVenta.Listar(GetTiposDocumentoPermitidos(), fechaInicio.Value, fechaFin.Value, clienteNombre ?? string.Empty, isEnviado, _datosUsuario.PersonalId, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oPagina<vDocumentoVentaSimplificado>> ListarSimplificado(string numeroDocumento, oPaginacion paginacion)
        {
            try
            {
                dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                return await dDocumentoVenta.ListarSimplificado(numeroDocumento ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oPagina<vDocumentoVentaAnticipo>> ListarAnticipos(DateTime? fechaInicio, DateTime? fechaFin, string clienteId, string numeroDocumento, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
                return await dDocumentoVenta.ListarAnticipos(fechaInicio.Value, fechaFin.Value, clienteId, numeroDocumento ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dDocumentoVenta(GetConnectionString()).Existe(id);

        public async Task<bool> IsEnviadoSunat(string id) => await new dDocumentoVenta(GetConnectionString()).IsEnviadoSunat(id);

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id) => await new dDocumentoVenta(GetConnectionString()).IsBloqueado(id);

        public async Task<bool> StockSuficiente(DocumentoVentaDTO documentoVenta)
        {
            var detallesAValidar = documentoVenta.Detalles.Select(x => new oArticuloValidarStock
            {
                Id = x.LineaId + x.SubLineaId + x.ArticuloId,
                Descripcion = x.Descripcion,
                StockSolicitado = x.Cantidad,
                IsIngreso = false,
                DocumentoVentaCompraId = Comun.IsVentaIdValido(documentoVenta.Id) ? documentoVenta.Id : string.Empty
            });

            return await StockSuficiente(detallesAValidar);
        }

        public bool CostoDetallesValido(DocumentoVentaDTO model)
        {
            if (model.TipoDocumentoId == "07" || model.TipoDocumentoId == "08")
                return true;

            int articulosConCostoMayor = 0;

            foreach (var detalle in model.Detalles)
            {
                if (detalle.PrecioCompra > detalle.PrecioUnitario)
                {
                    articulosConCostoMayor++;
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"El artículo {detalle.Descripcion} se está vendiendo por un precio menor al costo (Costo: {detalle.PrecioCompra})."));
                }
            }

            return articulosConCostoMayor == 0;
        }

        public async Task<bool> ClienteTieneCreditoDisponible(DocumentoVentaDTO model)
        {
            if (model.TipoVentaId == "CR")
            {
                decimal saldoAnterior = 0;

                if (Comun.IsVentaIdValido(model.Id))
                    saldoAnterior = await new dDocumentoVenta(GetConnectionString()).GetSaldoDocumento(model.Id);

                var (creditoPEN, creditoUSD) = await new dCliente(GetConnectionString()).GetCreditoDisponible(model.ClienteId);
                var moneda = dMoneda.GetPorId(model.MonedaId);
                var saldo = model.Total;
                var credito = (model.MonedaId == "S" ? creditoPEN : creditoUSD) + saldoAnterior;

                if (saldo > credito)
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"El cliente solo tiene disponible un crédito de {moneda.Abreviatura}{credito:#,###,##0.00}"));
                    return false;
                }
            }

            return true;
        }

        public async Task<object> FormularioTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(GetTiposDocumentoPermitidos());
            var series = Mapping.Mapper.Map<IEnumerable<oTipoDocumentoSerie>>(await new dCorrelativo(GetConnectionString()).ListarTodos(GetTiposDocumentoPermitidos()));
            var vendedores = await new dPersonal(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var tiposVenta = dTipoVentaCompra.ListarTodos();
            var tiposCobro = await new dTipoCobroPago(GetConnectionString()).ListarTodos();
            var motivosNota = await new dMotivoNota(GetConnectionString()).ListarTodos();
            var porcentajesIGV = await new dEmpresaIGV(GetConnectionString()).ListarTodos();
            var porcentajesRetencion = await new dEmpresaRetencion(GetConnectionString()).ListarTodos();
            var porcentajesDetraccion = await new dEmpresaDetraccion(GetConnectionString()).ListarTodos();
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();

            return new
            {
                tiposDocumento,
                series,
                vendedores,
                monedas,
                tiposVenta,
                tiposCobro,
                motivosNota,
                porcentajesIGV,
                porcentajesRetencion,
                porcentajesDetraccion,
                cuentasCorrientes
            };
        }

        private async Task AbonarVenta(oDocumentoVenta documentoVenta, decimal montoAbonado)
        {
            dAbonoVenta dAbonoVenta = new(GetConnectionString());
            await dAbonoVenta.Eliminar(documentoVenta.Id);

            if (montoAbonado > 0)
            {
                var montoPEN = documentoVenta.MonedaId == "S" ? montoAbonado : decimal.Round(decimal.Multiply(montoAbonado, documentoVenta.TipoCambio), 2, MidpointRounding.AwayFromZero);
                var montoUSD = documentoVenta.MonedaId == "S" ? decimal.Round(decimal.Divide(montoAbonado, documentoVenta.TipoCambio), 2, MidpointRounding.AwayFromZero) : montoAbonado;

                var abonoVenta = new oAbonoVenta
                {
                    EmpresaId = documentoVenta.EmpresaId,
                    TipoDocumentoId = documentoVenta.TipoDocumentoId,
                    Serie = documentoVenta.Serie,
                    Numero = documentoVenta.Numero,
                    AbonoId = 1,
                    Fecha = documentoVenta.FechaEmision,
                    MonedaId = documentoVenta.MonedaId,
                    TipoCambio = documentoVenta.TipoCambio,
                    Monto = montoAbonado,
                    MontoPEN = montoPEN,
                    MontoUSD = montoUSD,
                    DocumentoVentaId = documentoVenta.Id.Mid(2),
                    UsuarioId = documentoVenta.UsuarioId,
                    TipoCobroId = documentoVenta.TipoCobroId,
                    CuentaCorrienteId = documentoVenta.CuentaCorrienteId?.Mid(2, 2),
                    NumeroOperacion = documentoVenta.NumeroOperacion,
                    Concepto = "CANCELACION DE LA DEUDA",
                    IsBloqueado = false
                };

                await dAbonoVenta.Registrar(abonoVenta);
            }
        }

        private async Task AbonarConNotaCredito(oDocumentoVenta documentoVenta, decimal montoAbonado)
        {
            dDocumentoVenta dDocumentoVenta = new(GetConnectionString());
            dAbonoVenta dAbonoVenta = new(GetConnectionString());

            var (documentoReferenciaId, documentoReferenciaAbonoId) = await dDocumentoVenta.GetDatosDocumentoReferencia(documentoVenta.Id);
            await dAbonoVenta.Eliminar(documentoReferenciaId, documentoReferenciaAbonoId);

            if (montoAbonado > 0 && documentoVenta.Abonar)
            {
                var montoPEN = documentoVenta.MonedaId == "S" ? montoAbonado : decimal.Round(decimal.Multiply(montoAbonado, documentoVenta.TipoCambio), 2, MidpointRounding.AwayFromZero);
                var montoUSD = documentoVenta.MonedaId == "S" ? decimal.Round(decimal.Divide(montoAbonado, documentoVenta.TipoCambio), 2, MidpointRounding.AwayFromZero) : montoAbonado;
                var abonoId = await dAbonoVenta.GetNuevoId(documentoReferenciaId);

                var abonoVenta = new oAbonoVenta
                {
                    EmpresaId = documentoVenta.DocumentoReferenciaId.Mid(0, 2),
                    TipoDocumentoId = documentoVenta.DocumentoReferenciaId.Mid(2, 2),
                    Serie = documentoVenta.DocumentoReferenciaId.Mid(4, 4),
                    Numero = documentoVenta.DocumentoReferenciaId.Mid(8, 10),
                    AbonoId = abonoId,
                    Fecha = documentoVenta.FechaEmision,
                    MonedaId = documentoVenta.MonedaId,
                    TipoCambio = documentoVenta.TipoCambio,
                    Monto = montoAbonado,
                    MontoPEN = montoPEN,
                    MontoUSD = montoUSD,
                    DocumentoVentaId = documentoVenta.DocumentoReferenciaId.Mid(2),
                    UsuarioId = documentoVenta.UsuarioId,
                    TipoCobroId = documentoVenta.TipoCobroId,
                    CuentaCorrienteId = documentoVenta.CuentaCorrienteId?.Mid(2, 2),
                    NumeroOperacion = documentoVenta.NumeroOperacion,
                    Concepto = $"PAGO CON NOTA DE CREDITO N° {documentoVenta.Serie}-{documentoVenta.Numero.Right(8)}",
                    IsBloqueado = true
                };

                await dAbonoVenta.Registrar(abonoVenta);
                await dDocumentoVenta.ActualizarCampoDocumentoReferenciaAbonoId(documentoVenta.Id, abonoId);
            }
            else
            {
                await dDocumentoVenta.ActualizarCampoDocumentoReferenciaAbonoId(documentoVenta.Id, 0);
            }
        }

        //private async Task ActualizarLetraCambio(oDocumentoVenta documentoVenta)
        //{
        //    if (string.IsNullOrWhiteSpace(documentoVenta.LetraId))
        //        return;

        //    dProcesoLetra dProcesoLetra = new(GetConnectionString());

        //    oProcesoLetra procesoLetra = new()
        //    {
        //        Id = await dProcesoLetra.GetNuevoId(),
        //        EstadoLetraId = "N",
        //        DocumentoReferenciaId = documentoVenta.Id,
        //        DocumentoReferenciaNumeroDocumento = documentoVenta.NumeroDocumento,
        //        UsuarioId = _datosUsuario.Id
        //    };

        //    await dProcesoLetra.Registrar(procesoLetra);

        //    dVentaLetra dVentaLetra = new(GetConnectionString());

        //    await dVentaLetra.Registrar(new oVentaLetra
        //    {
        //        Id = procesoLetra.Id,
        //        DocumentoReferenciaId = documentoVenta.Id,
        //        DocumentoReferenciaNumeroDocumento = documentoVenta.NumeroDocumento,
        //        LetraRelacionadaId = documentoVenta.LetraId,
        //        LetraRelacionadaNumeroDocumento = documentoVenta.Letra,
        //        EstadoLetraId = procesoLetra.EstadoLetraId
        //    });

        //    dLetraCambioVenta dLetraCambioVenta = new(GetConnectionString());
        //    await dLetraCambioVenta.ActualizarEstado(documentoVenta.LetraId, "T");
        //}

        private static string[] GetTiposDocumentoPermitidos() => new string[] { "01", "03", "07", "08", "12", "NV" };
    }
}
