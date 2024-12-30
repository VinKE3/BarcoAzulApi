using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dProveedor : dComun
    {
        public dProveedor(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oProveedor proveedor)
        {
            string query = @"   INSERT INTO Proveedor (Prov_Codigo, Pro_Ruc, Pro_RazonSocial, Pro_Telefono, Pro_Correo, Pro_Direccion, Dep_Codigo, Pro_Codigo, Dis_Codigo, 
                                Car_Codigo, Ban_Codigo, Pro_Credito_Sol, Pro_Credito_Dol, Pro_FechaReg, Usu_Codigo, Pro_TipoDoc, 
                                Pro_Concar, Pro_Telefax, Pro_Condicion, Pro_Estado, Pro_Observacion)
                                VALUES (@Id, @NumeroDocumentoIdentidad, @Nombre, @Telefono, @CorreoElectronico, @DireccionPrincipal, @DepartamentoId, @ProvinciaId, @DistritoId,
                                1, 1, 0, 0, GETDATE(), @UsuarioId, @TipoDocumentoIdentidadId, 
                                'N', @Celular, @Condicion, @Estado, @Observacion)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, proveedor);
            }
        }

        public async Task Modificar(oProveedor proveedor)
        {
            string query = @"   UPDATE 
                                    Proveedor
                                SET
                                    Pro_TipoDoc = @TipoDocumentoIdentidadId,
                                    Pro_Ruc = @NumeroDocumentoIdentidad,
                                    Pro_RazonSocial = @Nombre,
                                    Pro_Telefono = @Telefono,
                                    Pro_Correo = @CorreoElectronico,
                                    Pro_Direccion = @DireccionPrincipal,
                                    Dep_Codigo = @DepartamentoId,
                                    Pro_Codigo = @ProvinciaId,
                                    Dis_Codigo = @DistritoId,
                                    Usu_Codigo = @UsuarioId,
                                    Pro_Telefax = @Celular,
                                    Pro_Condicion = @Condicion,
                                    Pro_Estado = @Estado,
                                    Pro_Observacion = @Observacion
                                WHERE 
                                    Prov_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, proveedor);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Proveedor WHERE Prov_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oProveedor> GetPorId(string id)
        {
            string query = @"   SELECT
                                    Prov_Codigo AS Id,
                                    Pro_TipoDoc AS TipoDocumentoIdentidadId,
                                    RTRIM(Pro_Ruc) AS NumeroDocumentoIdentidad,
                                    Pro_RazonSocial AS Nombre,
                                    Pro_Telefono AS Telefono,
                                    Pro_Correo AS CorreoElectronico,
                                    Pro_Direccion AS DireccionPrincipal,
                                    Dep_Codigo AS DepartamentoId,
                                    Pro_Codigo AS ProvinciaId,
                                    Dis_Codigo AS DistritoId,
                                    Pro_Telefax AS Celular,
                                    Pro_Condicion AS Condicion,
                                    Pro_Estado AS Estado,
                                    Pro_Observacion AS Observacion
                                FROM 
                                    Proveedor
                                WHERE 
                                    Prov_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oProveedor>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<oPagina<vProveedor>> Listar(string numeroDocumentoIdentidad, string nombre, oPaginacion paginacion)
        {
            string query = $@"  SELECT
	                                P.Codigo AS Id,
	                                P.Razon_Social AS Nombre,
	                                P.Ruc AS NumeroDocumentoIdentidad,
	                                P.Telefono,
                                    P.Direccion AS Direccion,
	                                P.TeleFax AS Celular,
	                                B.Ban_Nombre AS EntidadBancariaNombre,
	                                P.Numero AS NumeroCuentaBancaria,
	                                P.Moneda AS MonedaId
                                FROM 
	                                v_lst_proveedor P
	                                LEFT JOIN Entidad_Bancaria B ON P.Banco = B.Ban_Codigo
                                WHERE   
                                    Ruc LIKE '%' + @numeroDocumentoIdentidad + '%' 
                                    AND Razon_Social LIKE '%' + @nombre + '%'
                                ORDER BY
                                    Codigo
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vProveedor> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    numeroDocumentoIdentidad,
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 100 }
                }))
                {
                    pagina = new oPagina<vProveedor>
                    {
                        Data = await result.ReadAsync<vProveedor>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Prov_Codigo) FROM Proveedor WHERE Prov_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad)
        {
            string query = $"SELECT COUNT(Prov_Codigo) FROM Proveedor WHERE {(id is null ? string.Empty : "Prov_Codigo <> @id AND")} Pro_Ruc = @numeroDocumentoIdentidad";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumentoIdentidad
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Prov_Codigo) FROM Proveedor", null, "00000#");
        #endregion
    }
}
