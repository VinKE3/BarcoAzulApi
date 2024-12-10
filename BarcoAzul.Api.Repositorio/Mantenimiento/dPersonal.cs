using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Utilidades.Extensiones;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dPersonal : dComun
    {
        public dPersonal(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oPersonal personal)
        {
            string query = @"   INSERT INTO Personal(Per_Codigo, Per_Dni, Per_Activo, Per_ApePat, Per_ApeMat,
                                Per_Nombres, Dep_Codigo, Pro_Codigo, Dis_Codigo, Per_Direccion, Per_Telefono, Per_Celular,
                                Usu_Codigo, Per_FechaNac, Per_Sexo, Per_EstadoCivil, Per_Correo, Car_Codigo, Per_Observacion,
                                Ban_Codigo, Per_TipoCuentaCte, Per_CteCteMoneda, Per_NroCuentaCte, Per_FechaReg, Per_HoraExtra,
                                Per_Estable, Per_SueldoBasico, Per_SueldoPlani) VALUES
                                (@Id, @NumeroDocumentoIdentidad, @IsActivo, @ApellidoPaterno, @ApellidoMaterno,
                                @Nombres, @DepartamentoId, @ProvinciaId, @DistritoId, @Direccion, @Telefono, @Celular,
                                @UsuarioId, @FechaNacimiento, @SexoId, @EstadoCivilId, @CorreoElectronico, @CargoId, @Observacion,
                                @EntidadBancariaId, @TipoCuentaBancariaId, @MonedaId, @CuentaCorriente, GETDATE(), 'N', 'N', 0, 0)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    personal.Id,
                    personal.NumeroDocumentoIdentidad,
                    IsActivo = personal.IsActivo ? "S" : "N",
                    personal.ApellidoPaterno,
                    personal.ApellidoMaterno,
                    personal.Nombres,
                    personal.DepartamentoId,
                    personal.ProvinciaId,
                    personal.DistritoId,
                    personal.Direccion,
                    personal.Telefono,
                    personal.Celular,
                    personal.UsuarioId,
                    personal.FechaNacimiento,
                    personal.SexoId,
                    personal.EstadoCivilId,
                    personal.CorreoElectronico,
                    personal.CargoId,
                    personal.Observacion,
                    personal.EntidadBancariaId,
                    personal.TipoCuentaBancariaId,
                    personal.MonedaId,
                    personal.CuentaCorriente
                });
            }
        }

        public async Task Modificar(oPersonal personal)
        {
            string query = @"   UPDATE Personal SET Per_Dni = @NumeroDocumentoIdentidad, Per_Activo = @IsActivo, 
                                Per_ApePat = @ApellidoPaterno, Per_ApeMat = @ApellidoMaterno, Per_Nombres = @Nombres, Dep_Codigo = @DepartamentoId,
                                Pro_Codigo = @ProvinciaId, Dis_Codigo = @DistritoId, Per_Direccion = @Direccion, Per_Telefono = @Telefono, 
                                Per_Celular = @Celular, Usu_Codigo = @UsuarioId, Per_FechaNac = @FechaNacimiento, Per_Sexo = @SexoId,
                                Per_EstadoCivil = @EstadoCivilId, Per_Correo = @CorreoElectronico, Car_Codigo = @CargoId, Per_Observacion = @Observacion,
                                Ban_Codigo = @EntidadBancariaId, Per_TipoCuentaCte = @TipoCuentaBancariaId, Per_CteCteMoneda = @MonedaId, 
                                Per_NroCuentaCte = @CuentaCorriente, Per_FechaMod = GETDATE() WHERE Per_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    personal.NumeroDocumentoIdentidad,
                    IsActivo = personal.IsActivo ? "S" : "N",
                    personal.ApellidoPaterno,
                    personal.ApellidoMaterno,
                    personal.Nombres,
                    personal.DepartamentoId,
                    personal.ProvinciaId,
                    personal.DistritoId,
                    personal.Direccion,
                    personal.Telefono,
                    personal.Celular,
                    personal.UsuarioId,
                    personal.FechaNacimiento,
                    personal.SexoId,
                    personal.EstadoCivilId,
                    personal.CorreoElectronico,
                    personal.CargoId,
                    personal.Observacion,
                    personal.EntidadBancariaId,
                    personal.TipoCuentaBancariaId,
                    personal.MonedaId,
                    personal.CuentaCorriente,
                    personal.Id
                });
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Personal WHERE Per_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 8 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oPersonal> GetPorId(string id)
        {
            string query = @"   SELECT
	                                Per_Codigo AS Id,
	                                RTRIM(Per_Dni) AS NumeroDocumentoIdentidad,
                                    CAST(CASE WHEN Per_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
	                                Per_ApePat AS ApellidoPaterno,
	                                Per_ApeMat AS ApellidoMaterno,
	                                Per_Nombres AS Nombres,
	                                Dep_Codigo AS DepartamentoId,
	                                Pro_Codigo AS ProvinciaId,
	                                Dis_Codigo AS DistritoId,
	                                Per_Direccion AS Direccion,
	                                Per_Telefono AS Telefono,
	                                Per_Celular AS Celular,
	                                Per_FechaNac AS FechaNacimiento,
	                                Per_Sexo AS SexoId,
	                                Per_EstadoCivil AS EstadoCivilId,
	                                Per_Correo AS CorreoElectronico,
	                                Car_Codigo AS CargoId,
	                                Per_Observacion AS Observacion,
	                                Ban_Codigo AS EntidadBancariaId,
	                                Per_TipoCuentaCte AS TipoCuentaBancariaId,
	                                Per_CteCteMoneda AS MonedaId,
	                                Per_NroCuentaCte AS CuentaCorriente
                                FROM 
                                    Personal
                                WHERE 
                                    Per_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oPersonal>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 8 } });
            }
        }

        public async Task<IEnumerable<vPersonal>> ListarTodos()
        {
            string query = @"   SELECT
	                                P.Per_Codigo AS Id,
	                                P.Per_ApePat AS ApellidoPaterno,
	                                P.Per_ApeMat AS ApellidoMaterno,
	                                P.Per_Nombres AS Nombres,
	                                RTRIM(P.Per_Dni) AS NumeroDocumentoIdentidad,
	                                C.Car_Nombre AS CargoDescripcion,
	                                CAST(CASE WHEN P.Per_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo
                                FROM 
                                    Personal P
                                    LEFT JOIN Cargo C ON P.Car_Codigo = C.Car_Codigo
                                ORDER BY
                                    P.Per_ApePat, P.Per_ApeMat, P.Per_Nombres"
            ;

            using (var db = GetConnection())
            {
                return await db.QueryAsync<vPersonal>(query);
            }
        }

        public async Task<oPagina<vPersonal>> Listar(string nombreCompleto, oPaginacion paginacion)
        {
            string query = $@"   SELECT
	                                P.Per_Codigo AS Id,
	                                P.Per_ApePat AS ApellidoPaterno,
	                                P.Per_ApeMat AS ApellidoMaterno,
	                                P.Per_Nombres AS Nombres,
	                                RTRIM(P.Per_Dni) AS NumeroDocumentoIdentidad,
	                                C.Car_Nombre AS CargoDescripcion,
	                                CAST(CASE WHEN P.Per_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo
                                FROM 
                                    Personal P
                                    LEFT JOIN Cargo C ON P.Car_Codigo = C.Car_Codigo
                                WHERE 
                                    CONCAT(P.Per_ApePat, ' ', P.Per_ApeMat, ' ', P.Per_Nombres) LIKE '%' + @nombreCompleto + '%'
                                ORDER BY
                                    P.Per_ApePat, P.Per_ApeMat, P.Per_Nombres
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vPersonal> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombreCompleto = new DbString { Value = nombreCompleto, IsAnsi = true, IsFixedLength = false, Length = 100 } }))
                {
                    pagina = new oPagina<vPersonal>
                    {
                        Data = await result.ReadAsync<vPersonal>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = "SELECT COUNT(Per_Codigo) FROM Personal WHERE Per_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 8 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad)
        {
            string query = @$"SELECT COUNT(Per_Codigo) FROM Personal WHERE {(id is null ? string.Empty : "Per_Codigo <> @id AND")} Per_Dni = @numeroDocumentoIdentidad";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 8 },
                    numeroDocumentoIdentidad = new DbString { Value = numeroDocumentoIdentidad, IsAnsi = true, IsFixedLength = true, Length = 9 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId(oPersonal personal)
        {
            var personalId = $"{personal.ApellidoPaterno.Left(2)}{(!string.IsNullOrWhiteSpace(personal.ApellidoMaterno) ? personal.ApellidoMaterno.Left(2) : "01")}{personal.Nombres.Left(2)}";
            var numeracionId = await GetNuevoId("SELECT MAX(RIGHT(Per_Codigo, 2)) FROM Personal WHERE LEFT(Per_Codigo, 6) = @personalId", new { personalId }, "0#");

            return personalId + numeracionId;
        }
        #endregion
    }
}
