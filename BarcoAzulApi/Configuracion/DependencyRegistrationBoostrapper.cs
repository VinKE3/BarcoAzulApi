﻿using BarcoAzul.Api.Logica.Almacen;
using BarcoAzul.Api.Logica.Compra;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Logica.Empresa;
using BarcoAzul.Api.Logica.Finanzas;
using BarcoAzul.Api.Logica.Informes.Articulos;
using BarcoAzul.Api.Logica.Informes.Gerencia;
using BarcoAzul.Api.Logica.Informes.Sistema;
using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Logica.Venta;
using BarcoAzul.Api.Modelos.Interfaces;

namespace BarcoAzulApi.Configuracion
{
    public static class DependencyRegistrationBoostrapper
    {
        public static void RegisterDependencies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            //Lógica
            services.AddScoped<bEmpresa>();
            services.AddScoped<bEmpresa>();
            services.AddScoped<bMenu>();
            services.AddScoped<bUsuario>();
            services.AddScoped<bUsuarioPermiso>();
            services.AddScoped<bBloquearVenta>();
            services.AddScoped<bBloquearCompra>();
            services.AddScoped<bBloquearMovimientoBancario>();
            //services.AddScoped<bBloquearReciboEgreso>();
            services.AddScoped<bArticulo>();
            services.AddScoped<bCliente>();
            services.AddScoped<bClienteDireccion>();
            services.AddScoped<bClienteContacto>();
            services.AddScoped<bClientePersonal>();
            services.AddScoped<bProveedor>();
            services.AddScoped<bProveedorContacto>();
            services.AddScoped<bProveedorCuentaCorriente>();
            services.AddScoped<bTipoCambio>();
            services.AddScoped<bLinea>();
            services.AddScoped<bSubLinea>();
            services.AddScoped<bMarca>();
            services.AddScoped<bUnidadMedida>();
            services.AddScoped<bEntidadBancaria>();
            services.AddScoped<bCuentaCorriente>();
            services.AddScoped<bDepartamento>();
            services.AddScoped<bProvincia>();
            services.AddScoped<bDistrito>();
            services.AddScoped<bCargo>();
            services.AddScoped<bPersonal>();
            //services.AddScoped<bTipoCobroPago>();
            services.AddScoped<bEmpresaTransporte>();
            services.AddScoped<bVehiculo>();
            services.AddScoped<bConductor>();
            services.AddScoped<bCorrelativo>();
            services.AddScoped<bMovimientoArticulo>();
            services.AddScoped<bCuadreStock>();
            //services.AddScoped<bSalidaCilindros>();
            //services.AddScoped<bEntradaCilindros>();
            services.AddScoped<bMovimientoBancario>();
            services.AddScoped<bCuentaPorCobrar>();
            services.AddScoped<bOrdenCompra>();
            services.AddScoped<bDocumentoCompra>();
            //services.AddScoped<bFacturaNegociable>();
            //services.AddScoped<bLetraCambioCompra>();
            //services.AddScoped<bCEF>();
            //services.AddScoped<bCheque>();
            services.AddScoped<bGuiaCompra>();
            services.AddScoped<bCuentaPorPagar>();
            services.AddScoped<bAbonoCompra>();
            services.AddScoped<bNotaPedido>();
            services.AddScoped<bDocumentoVenta>();
            //services.AddScoped<bRetencion>();
            //services.AddScoped<bGuiaRemision>();
            //services.AddScoped<bPlanillaCobro>();
            //services.AddScoped<bLetraCambioVenta>();
            //services.AddScoped<bProcesoLetra>();
            //services.AddScoped<bVentaLetra>();
            services.AddScoped<bEntradaAlmacen>();
            services.AddScoped<bSalidaAlmacen>();
            services.AddScoped<bReporteClientes>();
            services.AddScoped<bReporteProveedores>();
            //services.AddScoped<bReportePersonal>();
            //services.AddScoped<bReportePersonalCliente>();
            //services.AddScoped<bTomaInventario>();
            //services.AddScoped<bStockValorizado>();
            //services.AddScoped<bListadoCostos>();
            services.AddScoped<bMovimientoArticulo>();
            //services.AddScoped<bKardexMarca>();
            services.AddScoped<bSesion>();
            //services.AddScoped<bReporteCuentaBancaria>();
            //services.AddScoped<bRegistroCompra>();
            //services.AddScoped<bReporteCompraArticulo>();
            //services.AddScoped<bCompraPorProveedor>();
            //services.AddScoped<bOrdenesCompra>();
            //services.AddScoped<bReporteCompraLogistica>();
            //services.AddScoped<bEntradasAlmacen>();
            //services.AddScoped<bReporteLetraCambio>();
            //services.AddScoped<bReporteGuiaCompra>();
            //services.AddScoped<bReporteGuiaCompraDetalle>();
            //services.AddScoped<bReporteCuentaPorPagar>();
            //services.AddScoped<bVentaTipoDocumento>();
            //services.AddScoped<bVentaTiendaMarca>();
            //services.AddScoped<bRegistroVenta>();
            //services.AddScoped<bRegistroVentaDetalle>();
            //services.AddScoped<bRegistroVentaUbigeo>();
            //services.AddScoped<bRegistroSalidaCilindro>();
            //services.AddScoped<bVentaPorCliente>();
            //services.AddScoped<bVentaPorClienteDocumento>();
            //services.AddScoped<bVentaPorVendedor>();
            //services.AddScoped<bVentaPorVendedorDetalle>();
            //services.AddScoped<bVentaPorVendedorArticulo>();
            //services.AddScoped<bReporteGuiaRemision>();
            //services.AddScoped<bReporteGuiaValorizada>();
            //services.AddScoped<bReporteDocumento>();
            //services.AddScoped<bInformeCilindro>();
            //services.AddScoped<bReporteLetraCambioVenta>();
            //services.AddScoped<bReporteCuentaPorCobrar>();
            //services.AddScoped<bReporteCuentaPorCobrarVencido>();
            //services.AddScoped<bInformeCobranza>();
            //services.AddScoped<bInformePlanillaPago>();
            //services.AddScoped<bInformeControlPlanilla>();
            //services.AddScoped<bReporteCobranzaDetallado>();
            //services.AddScoped<bReporteIngresoBancarioTienda>();
            //services.AddScoped<bReporteIngresoEgreso>();
            //services.AddScoped<bPagoPendienteRealizado>();
            //services.AddScoped<bInformeTienda>();
            //services.AddScoped<bInformeUtilidad>();
            //services.AddScoped<bInformeCostoProducto>();
            services.AddScoped<bCompraPorArticulo>();
            //services.AddScoped<bVentaPorArticulo>();
            //services.AddScoped<bVentaPorVendedorCliente>();
            //services.AddScoped<bVentaPorMarcaArticulo>();
            //services.AddScoped<bVentaPorVendedorMes>();
            //services.AddScoped<bVentaPorVendedorMesDia>();
            //services.AddScoped<bVentaPorArticuloVendedor>();
            //services.AddScoped<bRegistroCompraMes>();
            //services.AddScoped<bInformeContable>();
            services.AddTransient<ILogicaServiceFactory, LogicaServiceFactory>();
        }
    }
}
