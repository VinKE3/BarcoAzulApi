using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dCompraPorProveedor : dComun
    {
        public dCompraPorProveedor(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oCompraPorProveedor>> GetRegistros(oParamCompraPorProveedor parametros)
        {
            string query;

            if (parametros.TipoReporte == "S")
            {
                query = @$"	SELECT
								Proveedor AS ProveedorNombre,
								Ruc AS ProveedorNumeroDocumentoIdentidad,
								Documento AS NumeroDocumento,
								Fecha AS FechaEmision,
								Vencimiento AS FechaVencimiento,
								Personal AS PersonalNombreCompleto,
								(CASE WHEN TipoDoc = '07' THEN Total * (-1) ELSE Total END) AS Total
							FROM 
								v_lst_compra
							WHERE
								Moneda = @monedaId
								{(string.IsNullOrWhiteSpace(parametros.ProveedorId) ? string.Empty : "AND Prov_Codigo = @proveedorId")}
								AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
								AND TipoDoc IN ('01', '03', '07', '08')";
            }
            else
            {
                query = $@"	SELECT 
								C.Proveedor AS ProveedorNombre,
								C.Ruc AS ProveedorNumeroDocumentoIdentidad,
								C.Documento AS NumeroDocumento,
								C.Fecha AS FechaEmision,
								C.Vencimiento AS FechaVencimiento,
								C.Personal AS PersonalNombreCompleto,
								(CASE WHEN TipoDoc = '07' THEN Total * (-1) ELSE Total END) AS Total,
								D.Com_Item AS Item,	
								D.DCom_Descripcion AS ArticuloDescripcion,
								(SELECT Uni_Nombre FROM Unidad_Medida U WHERE U.Uni_Codigo = D.Uni_Codigo) AS UnidadMedidaDescripcion,
								D.DCom_Cantidad AS Cantidad,
								D.DCom_Precio AS PrecioUnitario,
								D.DCom_Importe AS Importe
							FROM 
								v_lst_compra C
								INNER JOIN Detalle_Compra D ON D.Conf_Codigo + D.Prov_Codigo + D.TDoc_Codigo + D.Com_Serie + D.Com_Numero = LEFT(C.Codigo, 24)
							WHERE 
								C.Moneda = @monedaId
								{(string.IsNullOrWhiteSpace(parametros.ProveedorId) ? string.Empty : "AND C.Prov_Codigo = @proveedorId")}
								AND (C.Fecha BETWEEN @fechaInicio AND @fechaFin)
								AND C.TipoDoc IN ('01', '03', '07', '08')
							ORDER BY
								C.Documento,
								D.Com_Item ASC";
            }

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oCompraPorProveedor>(query, new
                {
                    monedaId = new DbString { Value = parametros.MonedaId, IsAnsi = true, IsFixedLength = true, Length = 1 },
                    proveedorId = new DbString { Value = parametros.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    fechaInicio = parametros.FechaInicio,
                    fechaFin = parametros.FechaFin
                });
            }
        }
    }
}
