using BarcoAzul.Api.Informes.PDFs;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Almacen;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Utilidades;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Almacen
{
    public class bSalidaAlmacen : bComun
    {
        public bSalidaAlmacen(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Salida Almacén", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(SalidaAlmacenDTO model)
        {
            try
            {
                dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());

                var salidaAlmacen = Mapping.Mapper.Map<oSalidaAlmacen>(model);
                salidaAlmacen.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                salidaAlmacen.TipoDocumentoId = model.TipoDocumentoId = dSalidaAlmacen.TipoDocumentoId;
                salidaAlmacen.Numero = model.Numero = await dSalidaAlmacen.GetNuevoNumero(salidaAlmacen.EmpresaId, salidaAlmacen.Serie);
                salidaAlmacen.UsuarioId = _datosUsuario.Id;
                salidaAlmacen.ProcesarDatos();
                salidaAlmacen.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dSalidaAlmacen.Registrar(salidaAlmacen);

                    dSalidaAlmacenDetalle dSalidaAlmacenDetalle = new(GetConnectionString());
                    await dSalidaAlmacenDetalle.Registrar(salidaAlmacen.Detalles);

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

        public async Task<bool> Modificar(SalidaAlmacenDTO model)
        {
            try
            {
                var salidaAlmacen = Mapping.Mapper.Map<oSalidaAlmacen>(model);

                salidaAlmacen.UsuarioId = _datosUsuario.Id;
                salidaAlmacen.ProcesarDatos();
                salidaAlmacen.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());
                    await dSalidaAlmacen.Modificar(salidaAlmacen);

                    dSalidaAlmacenDetalle dSalidaAlmacenDetalle = new(GetConnectionString());
                    await dSalidaAlmacenDetalle.Modificar(salidaAlmacen.Detalles);

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
                    dAbonoVenta dAbonoVenta = new(GetConnectionString());
                    await dAbonoVenta.Eliminar(id);

                    dSalidaAlmacenDetalle dSalidaAlmacenDetalle = new(GetConnectionString());
                    await dSalidaAlmacenDetalle.EliminarDeSalidaAlmacen(id);

                    dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());
                    await dSalidaAlmacen.Eliminar(id);

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
                    dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());
                    await dSalidaAlmacen.Anular(id);

                    dSalidaAlmacenDetalle dSalidaAlmacenDetalle = new(GetConnectionString());
                    await dSalidaAlmacenDetalle.AnularDeSalidaAlmacen(id);

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
                var salidaAlmacen = await GetPorId(id, true);
                var rptPath = RptPath.RptSalidaArticulosPath;

                var pdfSalidaAlmacen = new PDFSalidaAlmacen(salidaAlmacen, _configuracionGlobal, rptPath);

                return ($"{salidaAlmacen.TipoDocumentoId}-{salidaAlmacen.Serie}-{int.Parse(salidaAlmacen.Numero)}.pdf", pdfSalidaAlmacen.Generar());
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return (string.Empty, null);
            }
        }

        public async Task<bool> Cerrar(string id)
        {
            try
            {
                dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());
                await dSalidaAlmacen.ActualizarEstadoCancelado(id, true);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<oSalidaAlmacen> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());
                var salidaAlmacen = await dSalidaAlmacen.GetPorId(id);

                dSalidaAlmacenDetalle dSalidaAlmacenDetalle = new(GetConnectionString());
                salidaAlmacen.Detalles = (await dSalidaAlmacenDetalle.ListarPorSalidaAlmacen(salidaAlmacen.Id)).ToList();

                if (incluirReferencias)
                {
                    salidaAlmacen.Cliente = await new dCliente(GetConnectionString()).GetPorId(salidaAlmacen.ClienteId);
                    salidaAlmacen.Moneda = dMoneda.GetPorId(salidaAlmacen.MonedaId);
                    salidaAlmacen.Personal = await new dPersonal(GetConnectionString()).GetPorId(salidaAlmacen.PersonalId);
                }

                return salidaAlmacen;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vSalidaAlmacen>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string numeroDocumento, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dSalidaAlmacen dSalidaAlmacen = new(GetConnectionString());
                return await dSalidaAlmacen.Listar(fechaInicio.Value, fechaFin.Value, numeroDocumento ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dSalidaAlmacen(GetConnectionString()).Existe(id);

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id) => await new dSalidaAlmacen(GetConnectionString()).IsBloqueado(id);

        public async Task<bool> StockSuficiente(SalidaAlmacenDTO salidaAlmacen)
        {
            var detallesAValidar = salidaAlmacen.Detalles.Select(x => new oArticuloValidarStock
            {
                Id = x.LineaId + x.SubLineaId + x.ArticuloId,
                Descripcion = x.Descripcion,
                StockSolicitado = x.Cantidad,
                IsIngreso = false,
                DocumentoVentaCompraId = Comun.IsVentaIdValido(salidaAlmacen.Id) ? salidaAlmacen.Id : string.Empty
            });

            return await StockSuficiente(detallesAValidar);
        }

        public async Task<object> FormularioTablas()
        {
            var monedas = dMoneda.ListarTodos();
            var personal = await new dPersonal(GetConnectionString()).ListarTodos();
            var serie = await new dSalidaAlmacen(GetConnectionString()).GetSerie(_configuracionGlobal.EmpresaId);
            var motivos = await new dMotivoTraslado(GetConnectionString()).ListarTodos();
            return new
            {
                monedas,
                personal,
                motivos,
                serie
            };
        }
    }
}
