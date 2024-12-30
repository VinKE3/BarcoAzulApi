using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Gerencia
{
    public class dCompraPorArticulo : dComun
    {
        public dCompraPorArticulo(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oCompraPorArticulo>> GetRegistros(oParamCompraPorArticulo parametros)
        {
            string query;

            if (parametros.TipoReporte == "AC")
            {
                query = $@"     SELECT 
	                                Codigo AS ArticuloId,
	                                Descripcion AS ArticuloDescripcion, 
	                                Unidad AS UnidadMedidaAbreviatura,
	                                (SELECT TOP 1 Dcom_Precio 
	                                FROM Detalle_Compra DC 
	                                WHERE DC.DCom_Precio > 0 AND DC.Lin_Codigo + DC.SubL_codigo + DC.Art_Codigo = Codigo ORDER BY dcom_fecha DESC) AS PrecioUnitario,
	                                SUM(CASE TDoc_Codigo WHEN '07' THEN (Cantidad) * (-1) ELSE Cantidad END) AS Cantidad
                                FROM 
	                                v_lst_compraarticulo
                                WHERE 
	                                Art_CtrlStock = 'S'
	                                AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND TDoc_Codigo IN ('00', '01', '03', '12', 'RC', '07', '08', 'NV', 'PR', 'CR', 'CV', 'QC')
                                GROUP BY 
	                                Codigo,
	                                Descripcion,
	                                Unidad";
            }
            else
            {
                query = $@"     SELECT 
	                                SubLinea AS SubLineaDescripcion,
	                                Descripcion AS ArticuloDescripcion,
	                                Fecha AS FechaEmision,
	                                Documento AS NumeroDocumento,
	                                Proveedor AS ProveedorNombre,
	                                Ruc AS ProveedorNumeroDocumentoIdentidad,
	                                Unidad AS UnidadMedidaAbreviatura,
	                                (CASE TDoc_Codigo WHEN '07' THEN (Cantidad) * (-1) ELSE Cantidad END) AS Cantidad
                                FROM 
	                                v_lst_compraarticulo
                                WHERE 
	                                Art_CtrlStock = 'S'
	                                AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND TDoc_Codigo IN {(parametros.TipoReporte == "CA" ? "('00', '01', '03', '12', 'RC', '07', '08', 'NV', 'PR', 'CR', 'CV', 'QC')" : "('EN')")}";
            }

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oCompraPorArticulo>(query, new
                {
                    fechaInicio = parametros.FechaInicio,
                    fechaFin = parametros.FechaFin
                });
            }
        }
    }
}
