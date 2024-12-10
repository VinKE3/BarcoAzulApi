using BarcoAzul.Api.Modelos.Atributos;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;
using System.Data;

namespace BarcoAzul.Api.Repositorio.Configuracion
{
    public class dSesion : dComun
    {
        public string UsuarioId { get; private set; }

        public dSesion(string connectionString) : base(connectionString) { }

        public async Task<bool> UsuarioValido(oSesionUsuario sesionUsuario)
        {
            string query = "SELECT Usu_Codigo AS UsuarioId, CAST(CASE WHEN Usu_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo FROM Usuario WHERE Usu_Nick = @usuario";
            dynamic usuario;

            using (var db = GetConnection())
            {
                usuario = await db.QueryFirstOrDefaultAsync(query, new
                {
                    usuario = new DbString { Value = sesionUsuario.Usuario, IsAnsi = true, IsFixedLength = false, Length = 20 }
                });
            }

            if (usuario == null)
            {
                throw new MensajeException(new oMensaje(MensajeTipo.Error, "Usuario y/o clave incorrectas."));
            }
            else if (!Convert.ToBoolean(usuario.IsActivo))
            {
                throw new MensajeException(new oMensaje(MensajeTipo.Error, "El usuario ha sido desactivado."));
            }
            else
            {
                var claveDesencriptada = await ProcesarClave(AccionClave.Desencriptar, usuario.UsuarioId);

                if (sesionUsuario.Clave != claveDesencriptada)
                {
                    throw new MensajeException(new oMensaje(MensajeTipo.Error, "Usuario y/o clave incorrectas."));
                }

                UsuarioId = usuario.UsuarioId;

                return true;
            }
        }

        public async Task<string> ProcesarClave(AccionClave accionClave, string usuarioId)
        {
            using (var db = GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@tipo", accionClave == AccionClave.Encriptar ? "ENC" : "DES", dbType: DbType.AnsiString, size: 3);
                parameters.Add("@Usu_Codigo", usuarioId, dbType: DbType.AnsiString, size: 3);
                parameters.Add("@ClaveDes", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);

                await db.QueryAsync<string>("SP_EncriptarDesencriptar", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<string>("@ClaveDes");
            }
        }

        public async Task<oRefreshToken> GetRefreshTokenPorUsuarioId(string usuarioId)
        {
            string query = "SELECT Usu_Codigo AS UsuarioId, Usu_RefreshToken AS Token, Usu_RefreshTokenExpiration AS Expiracion FROM Usuario WHERE Usu_Codigo = @usuarioId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oRefreshToken>(query, new { usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 } });
            }
        }

        public async Task ActualizarDatosRefreshToken(oRefreshToken refreshToken)
        {
            string query = "UPDATE Usuario SET Usu_RefreshToken = @Token, Usu_RefreshTokenExpiration = @Expiracion WHERE Usu_Codigo = @UsuarioId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    Token = new DbString { Value = refreshToken.Token, IsAnsi = true, IsFixedLength = false, Length = 100 },
                    refreshToken.Expiracion,
                    UsuarioId = new DbString { Value = refreshToken.UsuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 }
                });
            }
        }

        public async Task RevocarRefreshToken(string usuarioId)
        {
            string query = "UPDATE Usuario SET Usu_RefreshToken = NULL, Usu_RefreshTokenExpiration = NULL WHERE Usu_Codigo = @UsuarioId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 } });
            }
        }

        public enum AccionClave
        {
            Encriptar,
            Desencriptar
        }
    }
}
