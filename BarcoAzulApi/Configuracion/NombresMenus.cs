using System.Reflection;

namespace BarcoAzulApi.Configuracion
{
    public class NombresMenus
    {
        #region Área Empresa
        public const string EmpresaConfiguracion = "EmpresaConfiguracion";
        #endregion

        #region Área Venta
        public const string NotaPedido = "NotaPedido";
        public const string DocumentoVenta = "DocumentoVenta";
        public const string Retencion = "Retencion";
        public const string GuiaRemision = "GuiaRemision";
        public const string LetraCambioVenta = "LetraCambioVenta";
        public const string BloquearVenta = "BloquearVenta";
        #endregion

        #region Área Compra
        public const string OrdenCompra = "OrdenCompra";
        public const string DocumentoCompra = "DocumentoCompra";
        public const string FacturaNegociable = "FacturaNegociable";
        public const string LetraCambioCompra = "LetraCambioCompra";
        public const string CEF = "CEF";
        public const string Cheque = "Cheque";
        public const string GuiaCompra = "GuiaCompra";
        public const string BloquearCompra = "BloquearCompra";
        #endregion

        #region Área Almacén
        public const string MovimientoArticulo = "MovimientoArticulo";
        public const string CuadreStock = "CuadreStock";
        public const string EntradaAlmacen = "EntradaAlmacen";
        public const string SalidaAlmacen = "SalidaAlmacen";
        public const string SalidaCilindros = "SalidaCilindros";
        public const string EntradaCilindros = "EntradaCilindros";
        #endregion

        #region Área Mantenimiento
        public const string Usuario = "Usuario";
        public const string Articulo = "Articulo";
        public const string Cliente = "Cliente";
        public const string Proveedor = "Proveedor";
        public const string TipoCambio = "TipoCambio";
        public const string Linea = "Linea";
        public const string SubLinea = "SubLinea";
        public const string Marca = "Marca";
        public const string UnidadMedida = "UnidadMedida";
        public const string EntidadBancaria = "EntidadBancaria";
        public const string CuentaCorriente = "CuentaCorriente";
        public const string Departamento = "Departamento";
        public const string Provincia = "Provincia";
        public const string Distrito = "Distrito";
        public const string Cargo = "Cargo";
        public const string Personal = "Personal";
        public const string TipoCobroPago = "TipoCobroPago";
        public const string EmpresaTransporte = "EmpresaTransporte";
        public const string Vehiculo = "Vehiculo";
        public const string Conductor = "Conductor";
        public const string Correlativo = "Correlativo";
        #endregion

        #region Área Finanzas
        public const string BloquearMovimientoBancario = "BloquearMovimientoBancario";
        public const string MovimientoBancario = "MovimientoBancario";
        public const string CuentaPorPagar = "CuentaPorPagar";
        public const string CuentaPorCobrar = "CuentaPorCobrar";
        public const string PlanillaCobro = "PlanillaCobro";
        #endregion

        #region Área Tesorería
        public const string BloquearReciboEgreso = "BloquearReciboEgreso";
        #endregion

        #region Área Servicios
        public const string ConsultaTipoCambio = "ConsultaTipoCambio";
        public const string ConsultaRucDni = "ConsultaRucDni";
        #endregion

        #region Área Informes

        #region SubÁrea Sistema
        public const string RptClientes = nameof(RptClientes);
        public const string RptProveedores = nameof(RptProveedores);
        public const string RptPersonal = nameof(RptPersonal);
        public const string RptPersonalCliente = nameof(RptPersonalCliente);
        #endregion

        #region SubÁrea Artículos
        public const string RptTomaInventario = nameof(RptTomaInventario);
        public const string RptStockValorizado = nameof(RptStockValorizado);
        public const string RptListadoCostos = nameof(RptListadoCostos);
        public const string RptMovimientoArticulo = nameof(RptMovimientoArticulo);
        public const string RptKardexMarca = nameof(RptKardexMarca);
        #endregion

        #region SubÁrea Finanzas
        public const string RptCuentaBancaria = nameof(RptCuentaBancaria);
        public const string RptIngresoBancarioTienda = nameof(RptIngresoBancarioTienda);
        public const string RptIngresoEgreso = nameof(RptIngresoEgreso);
        public const string RptPagoPendienteRealizado = nameof(RptPagoPendienteRealizado);
        #endregion

        #region SubÁrea Compras
        public const string RptRegistroCompra = nameof(RptRegistroCompra);
        public const string RptCompraArticulo = nameof(RptCompraArticulo);
        public const string RptCompraPorProveedor = nameof(RptCompraPorProveedor);
        public const string RptOrdenesCompra = nameof(RptOrdenesCompra);
        public const string RptCompraLogistica = nameof(RptCompraLogistica);
        public const string RptEntradasAlmacen = nameof(RptEntradasAlmacen);
        public const string RptLetraCambio = nameof(RptLetraCambio);
        public const string RptGuiaCompra = nameof(RptGuiaCompra);
        public const string RptGuiaCompraDetalle = nameof(RptGuiaCompraDetalle);
        public const string RptCuentaPorPagar = nameof(RptCuentaPorPagar);
        #endregion

        #region SubÁrea Ventas
        public const string RptVentaTipoDocumento = nameof(RptVentaTipoDocumento);
        public const string RptVentaTiendaMarca = nameof(RptVentaTiendaMarca);
        public const string RptRegistroVenta = nameof(RptRegistroVenta);
        public const string RptRegistroVentaDetalle = nameof(RptRegistroVentaDetalle);
        public const string RptRegistroVentaUbigeo = nameof(RptRegistroVentaUbigeo);
        public const string RptRegistroSalidaCilindro = nameof(RptRegistroSalidaCilindro);
        public const string RptVentaPorCliente = nameof(RptVentaPorCliente);
        public const string RptVentaPorClienteDocumento = nameof(RptVentaPorClienteDocumento);
        public const string RptVentaPorVendedor = nameof(RptVentaPorVendedor);
        public const string RptVentaPorVendedorDetalle = nameof(RptVentaPorVendedorDetalle);
        public const string RptVentaPorVendedorArticulo = nameof(RptVentaPorVendedorArticulo);
        public const string RptGuiaRemision = nameof(RptGuiaRemision);
        public const string RptGuiaValorizada = nameof(RptGuiaValorizada);
        public const string RptReporteDocumento = nameof(RptReporteDocumento);
        public const string RptInformeCilindro = nameof(RptInformeCilindro);
        #endregion

        #region SubÁrea Cobranzas
        public const string RptLetraCambioVenta = nameof(RptLetraCambioVenta);
        public const string RptCuentaPorCobrar = nameof(RptCuentaPorCobrar);
        public const string RptCuentaPorCobrarVencido = nameof(RptCuentaPorCobrarVencido);
        public const string RptInformeCobranza = nameof(RptInformeCobranza);
        public const string RptInformePlanillaPago = nameof(RptInformePlanillaPago);
        public const string RptInformeControlPlanilla = nameof(RptInformeControlPlanilla);
        public const string RptCobranzaDetallado = nameof(RptCobranzaDetallado);
        #endregion

        #region SubÁrea Gerencia
        public const string RptInformeTienda = nameof(RptInformeTienda);
        public const string RptInformeUtilidad = nameof(RptInformeUtilidad);
        public const string RptInformeCostoProducto = nameof(RptInformeCostoProducto);
        public const string RptCompraPorArticulo = nameof(RptCompraPorArticulo);
        public const string RptVentaPorArticulo = nameof(RptVentaPorArticulo);
        public const string RptVentaPorVendedorCliente = nameof(RptVentaPorVendedorCliente);
        public const string RptVentaPorMarcaArticulo = nameof(RptVentaPorMarcaArticulo);
        public const string RptVentaPorVendedorMes = nameof(RptVentaPorVendedorMes);
        public const string RptVentaPorVendedorMesDia = nameof(RptVentaPorVendedorMesDia);
        public const string RptVentaPorArticuloVendedor = nameof(RptVentaPorArticuloVendedor);
        public const string RptRegistroCompraMes = nameof(RptRegistroCompraMes);
        #endregion

        #region Contables
        public const string RptInformeContable = nameof(RptInformeContable);
        #endregion

        #endregion

        public static IEnumerable<string> Listar()
        {
            var constantes = typeof(NombresMenus).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly);

            foreach (var constante in constantes)
            {
                yield return (string)constante.GetValue(null);
            }
        }
    }
}
