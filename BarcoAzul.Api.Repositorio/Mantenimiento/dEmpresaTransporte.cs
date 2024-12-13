
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dEmpresaTransporte : dComun
    {
        public dEmpresaTransporte(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oEmpresaTransporte empresaTransporte)
        {
            string query = @"   INSERT INTO EmpresaTransporte (Conf_Codigo, Tran_Codigo, Tran_RazonSocial, Tran_Ruc, Tran_Telefono, Tran_Celular, Tran_Correo, Tran_Direccion, 
                                Dep_Codigo, Pro_Codigo, Dis_Codigo, Tran_Observacion, Tran_FechaReg, Usu_Codigo) 
                                VALUES (@EmpresaId, @EmpresaTransporteId, @Nombre, @NumeroDocumentoIdentidad, @Telefono, @Celular, @CorreoElectronico, @Direccion,
                                @DepartamentoId, @ProvinciaId, @DistritoId, @Observacion, GETDATE(), @UsuarioId)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, empresaTransporte);
            }
        }

        public async Task Modificar(oEmpresaTransporte empresaTransporte)
        {
            string query = @"   UPDATE EmpresaTransporte SET Tran_RazonSocial = @Nombre, Tran_Ruc = @NumeroDocumentoIdentidad, Tran_Telefono = @Telefono, Tran_Celular = @Celular,
                                Tran_Correo = @CorreoElectronico, Tran_Direccion = @Direccion, Dep_Codigo = @DepartamentoId, Pro_Codigo = @ProvinciaId, Dis_Codigo = @DistritoId,
                                Tran_Observacion = @Observacion, Tran_FechaMod = GETDATE(), Usu_Codigo = @UsuarioId WHERE Conf_Codigo = @EmpresaId AND Tran_Codigo = @EmpresaTransporteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, empresaTransporte);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = "DELETE EmpresaTransporte WHERE Conf_Codigo = @empresaId AND Tran_Codigo = @empresaTransporteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    empresaTransporteId = new DbString { Value = splitId.EmpresaTransporteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oEmpresaTransporte> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT
                                    Conf_Codigo AS EmpresaId,
                                    Tran_Codigo AS EmpresaTransporteId,
                                    Tran_RazonSocial AS Nombre,
                                    Tran_Ruc AS NumeroDocumentoIdentidad,
                                    Tran_Telefono AS Telefono,
                                    Tran_Celular AS Celular,
                                    Tran_Correo AS CorreoElectronico,
                                    Tran_Direccion AS Direccion,
                                    Dep_Codigo AS DepartamentoId,
                                    Pro_Codigo AS ProvinciaId,
                                    Dis_Codigo AS DistritoId,
                                    Tran_Observacion AS Observacion
                                FROM
                                    EmpresaTransporte
                                WHERE
                                    Conf_Codigo = @empresaId
                                    AND Tran_Codigo = @empresaTransporteId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstAsync<oEmpresaTransporte>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    empresaTransporteId = new DbString { Value = splitId.EmpresaTransporteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
            }
        }

        public async Task<IEnumerable<vEmpresaTransporte>> ListarTodos()
        {
            string query = @"   SELECT
                                    Conf_Codigo AS EmpresaId,
                                    Tran_Codigo AS EmpresaTransporteId,
                                    Tran_Ruc AS NumeroDocumentoIdentidad,
                                    Tran_RazonSocial AS Nombre,
                                    Tran_Direccion AS Direccion,
                                    Tran_Telefono AS Telefono
                                FROM
                                    EmpresaTransporte
                                ORDER BY
                                    Tran_RazonSocial";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<vEmpresaTransporte>(query);
            }
        }

        public async Task<oPagina<vEmpresaTransporte>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = $@"  SELECT
                                    Conf_Codigo AS EmpresaId,
                                    Tran_Codigo AS EmpresaTransporteId,
                                    Tran_Ruc AS NumeroDocumentoIdentidad,
                                    Tran_RazonSocial AS Nombre,
                                    Tran_Direccion AS Direccion,
                                    Tran_Telefono AS Telefono
                                FROM
                                    EmpresaTransporte
                                WHERE
                                    Tran_RazonSocial LIKE '%' + @nombre + '%'
                                ORDER BY
                                    Tran_RazonSocial
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vEmpresaTransporte> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<vEmpresaTransporte>
                    {
                        Data = await result.ReadAsync<vEmpresaTransporte>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = @"SELECT COUNT(Conf_Codigo) FROM EmpresaTransporte WHERE Conf_Codigo = @empresaId AND Tran_Codigo = @empresaTransporteId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    empresaTransporteId = new DbString { Value = splitId.EmpresaTransporteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad)
        {
            var splitId = id is null ? null : SplitId(id);

            string query = @$"   SELECT
                                    COUNT(Conf_Codigo)
                                FROM
                                    EmpresaTransporte
                                WHERE 
                                    {(id is null ? string.Empty : "NOT(Conf_Codigo = @empresaId AND Tran_Codigo = @empresaTransporteId) AND ")}
                                    Tran_Ruc = @numeroDocumentoIdentidad";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId?.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    empresaTransporteId = new DbString { Value = splitId?.EmpresaTransporteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumentoIdentidad
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId(string empresaId) => await GetNuevoId("SELECT MAX(Tran_Codigo) FROM EmpresaTransporte WHERE Conf_Codigo = @empresaId AND LEN(Tran_Codigo) = 6", new { empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 } }, "000000");

        public static oSplitEmpresaTransporteId SplitId(string id) => new(id);
        #endregion
    }
}
