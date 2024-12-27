using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dCorrelativo : dComun
    {
        public dCorrelativo(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oCorrelativo correlativo)
        {
            string query = @"   INSERT INTO Susursal_Documento (Conf_Codigo, Suc_Codigo, TDoc_Codigo, ADoc_UltSerie, ADoc_UltNro) 
                                VALUES (@EmpresaId, '01', @TipoDocumentoId, @Serie, @Numero)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    correlativo.EmpresaId,
                    correlativo.TipoDocumentoId,
                    correlativo.Serie,
                    Numero = correlativo.Numero.ToString("0000000000")
                });
            }
        }

        public async Task Modificar(oCorrelativo correlativo)
        {
            string query = "UPDATE Susursal_Documento SET ADoc_UltNro = @Numero WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND ADoc_UltSerie = @Serie";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    Numero = correlativo.Numero.ToString("0000000000"),
                    correlativo.EmpresaId,
                    correlativo.TipoDocumentoId,
                    correlativo.Serie
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oCorrelativo> GetPorId(string empresaId, string tipoDocumentoId, string serie)
        {
            string query = @"   SELECT 
	                                SD.Conf_Codigo AS EmpresaId,
	                                SD.TDoc_Codigo AS TipoDocumentoId,
	                                TD.TDoc_Nombre AS TipoDocumentoDescripcion,
	                                SD.ADoc_UltSerie AS Serie,
	                                SD.ADoc_UltNro AS Numero
                                FROM 
	                                Susursal_Documento SD
	                                LEFT JOIN Tipo_Documento TD ON SD.TDoc_Codigo = TD.TDoc_Codigo
                                WHERE
                                    SD.Conf_Codigo = @empresaId AND SD.TDoc_Codigo = @tipoDocumentoId AND SD.ADoc_UltSerie = @serie";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCorrelativo>(query, new
                {
                    empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = serie, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });
            }
        }

        public async Task<IEnumerable<oCorrelativo>> ListarTodos(string[] tiposDocumento = null)
        {
            string query = @"   SELECT 
	                                SD.Conf_Codigo AS EmpresaId,
	                                SD.TDoc_Codigo AS TipoDocumentoId,
	                                TD.TDoc_Nombre AS TipoDocumentoDescripcion,
	                                SD.ADoc_UltSerie AS Serie,
	                                SD.ADoc_UltNro AS Numero
                                FROM 
	                                Susursal_Documento SD
	                                LEFT JOIN Tipo_Documento TD ON SD.TDoc_Codigo = TD.TDoc_Codigo";

            if (tiposDocumento != null)
                query += $" WHERE SD.TDoc_Codigo IN ({string.Join(", ", tiposDocumento.Select(x => $"'{x}'"))})";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oCorrelativo>(query);
            }
        }

        public async Task<oPagina<oCorrelativo>> Listar(string[] tiposDocumento, oPaginacion paginacion)
        {
            string query = @"   SELECT 
	                                SD.Conf_Codigo AS EmpresaId,
	                                SD.TDoc_Codigo AS TipoDocumentoId,
	                                TD.TDoc_Nombre AS TipoDocumentoDescripcion,
	                                SD.ADoc_UltSerie AS Serie,
	                                SD.ADoc_UltNro AS Numero
                                FROM 
	                                Susursal_Documento SD
	                                LEFT JOIN Tipo_Documento TD ON SD.TDoc_Codigo = TD.TDoc_Codigo
                                WHERE
                                    ISNUMERIC(SD.ADoc_UltNro) = 1";

            if (tiposDocumento is not null)
                query += $" AND SD.TDoc_Codigo IN ({string.Join(", ", tiposDocumento.Select(x => $"'{x}'"))})";

            query += $" ORDER BY SD.TDoc_Codigo {GetPaginacionQuery(paginacion)}";
            query += GetCountQuery(query);

            oPagina<oCorrelativo> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query))
                {
                    pagina = new oPagina<oCorrelativo>
                    {
                        Data = await result.ReadAsync<oCorrelativo>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string empresaId, string tipoDocumentoId, string serie)
        {
            string query = "SELECT COUNT(*) FROM Susursal_Documento WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND ADoc_UltSerie = @serie";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = serie, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });

                return existe > 0;
            }
        }
        #endregion
    }
}
