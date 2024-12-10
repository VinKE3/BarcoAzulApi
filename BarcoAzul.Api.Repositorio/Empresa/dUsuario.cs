using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Configuracion;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dUsuario : dComun
    {
        public dUsuario(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oUsuario usuario)
        {
            string query = @"   INSERT INTO Usuario (Usu_Codigo, Usu_Nick, Usu_Clave, Usu_TUsuario, Usu_Observ, Usu_Activo, Usu_ValidaStock, Per_Codigo, Usu_FechaReg, Usu_Autoriza)
                                VALUES (@Id, @Nick, @Clave, 'NO', @Observacion, @IsActivo, @HabilitarAfectarStock, @PersonalId, GETDATE(), @UsuarioAutorizadorId)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    usuario.Id,
                    usuario.Nick,
                    usuario.Clave,
                    usuario.Observacion,
                    HabilitarAfectarStock = usuario.HabilitarAfectarStock ? "S" : "N",
                    IsActivo = usuario.IsActivo ? "S" : "N",
                    usuario.PersonalId,
                    usuario.UsuarioAutorizadorId
                });
            }
        }

        public async Task Modificar(oUsuario usuario)
        {
            string query = @"   UPDATE Usuario SET Usu_Observ = @Observacion, Usu_Activo = @IsActivo, Usu_ValidaStock = @HabilitarAfectarStock,
                                Usu_FechaMod = GETDATE(), Per_Codigo = @PersonalId, Usu_Autoriza = @UsuarioAutorizadorId WHERE Usu_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    usuario.Observacion,
                    IsActivo = usuario.IsActivo ? "S" : "N",
                    HabilitarAfectarStock = usuario.HabilitarAfectarStock ? "S" : "N",
                    usuario.PersonalId,
                    usuario.UsuarioAutorizadorId,
                    Id = new DbString { Value = usuario.Id, IsAnsi = true, IsFixedLength = false, Length = 3 }
                });
            }
        }

        public async Task Eliminar(string id)
        {
            string query = string.Empty;

            using (var db = GetConnection())
            {
                query = "DELETE Acceso_Usuario WHERE Usu_Codigo = @id";
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 } });

                query = "DELETE Ingreso_Sistema WHERE Usu_Codigo = @id";
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 } });

                query = "DELETE Menu_Acceso WHERE Usu_Codigo = @id";
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 } });

                query = "DELETE Usuario WHERE Usu_Codigo = @id";
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oUsuario> GetPorId(string id)
        {
            string query = @"   SELECT 
                                    Usu_Codigo AS Id,
                                    Usu_Nick AS Nick,
                                    Usu_TUsuario AS TipoUsuarioId,
                                    Usu_Observ AS Observacion,
                                    CAST(CASE WHEN Usu_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
                                    CAST(CASE WHEN Usu_ValidaStock = 'S' THEN 1 ELSE 0 END AS BIT) AS HabilitarAfectarStock,
                                    Per_Codigo AS PersonalId
                                FROM 
                                    Usuario 
                                WHERE 
                                    Usu_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oUsuario>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 } });
            }
        }

        public async Task<oPagina<vUsuario>> Listar(string nick, oPaginacion paginacion)
        {
            string query = $@"   SELECT
                                    Usu_Codigo AS Id,
                                    Usu_Nick AS Nick,
	                                Usu_TUsuario AS TipoUsuarioDescripcion,
                                    Usu_FechaReg AS FechaInicio,
                                    Usu_FechaMod AS FechaModificacion,
                                    CAST(CASE WHEN Usu_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo
                                FROM 
                                    Usuario
                                WHERE
                                    Usu_Nick LIKE '%' + @nick + '%'
                                ORDER BY
                                    Usu_Codigo
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vUsuario> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nick = new DbString { Value = nick, IsAnsi = true, IsFixedLength = false, Length = 20 } }))
                {
                    pagina = new oPagina<vUsuario>
                    {
                        Data = await result.ReadAsync<vUsuario>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = "SELECT COUNT(Usu_Codigo) FROM Usuario WHERE Usu_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string nick)
        {
            string query = $@"SELECT COUNT(Usu_Codigo) FROM Usuario WHERE {(id is null ? string.Empty : "Usu_Codigo <> @id AND")} Usu_Nick = @nick";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 },
                    nick = new DbString { Value = nick, IsAnsi = true, IsFixedLength = false, Length = 20 }
                });
                return existe > 0;
            }
        }

        public async Task EncriptarClave(string id)
        {
            var claveEncriptada = await new dSesion(_connectionString).ProcesarClave(dSesion.AccionClave.Encriptar, id);

            string query = @"UPDATE Usuario SET Usu_Clave = @claveDesencriptada WHERE Usu_Codigo = @id";

            using (var db = GetConnection())
            {
                db.Execute(query, new
                {
                    claveDesencriptada = new DbString { Value = claveEncriptada, IsAnsi = true, IsFixedLength = false, Length = 20 },
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 3 },
                });
            }
        }

        public async Task CambiarClave(oUsuarioCambiarClave usuarioCambiarClave)
        {
            string query = @"UPDATE Usuario SET Usu_Clave = @ClaveNueva WHERE Usu_Codigo = @UsuarioId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, usuarioCambiarClave);
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Usu_Codigo) FROM Usuario", null, "000");
        #endregion
    }
}
