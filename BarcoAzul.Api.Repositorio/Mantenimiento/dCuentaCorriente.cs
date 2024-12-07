using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dCuentaCorriente : dComun
    {
        public dCuentaCorriente(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oCuentaCorriente cuentaCorriente)
        {
            string query = @"   INSERT INTO Cuenta_Corriente (Conf_Codigo, CC_Codigo, CC_CtaCble, CC_TipoCta, Ban_Codigo, CC_Numero, CC_Moneda, 
                                CC_SaldoIni, CC_Ingreso, CC_Egreso, CC_MontoIft, CC_SaldoFin, CC_Observ, CC_FechaReg, Usu_Codigo, CC_PorcItf, cc_contableanexo, cc_contabledesc, CC_IdEmpresa)
                                VALUES
                                (@EmpresaId, @CuentaCorrienteId, '', @TipoCuentaDescripcion, @EntidadBancariaId, @Numero, @MonedaId,
                                0, 0, 0, 0, 0, @Observacion, GETDATE(), @UsuarioId, 0, '', '', '01')";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, cuentaCorriente);
            }
        }

        public async Task Modificar(oCuentaCorriente cuentaCorriente)
        {
            string query = @"   UPDATE Cuenta_Corriente SET CC_TipoCta = @TipoCuentaDescripcion, Ban_Codigo = @EntidadBancariaId, CC_Numero = @Numero, 
                                CC_Moneda = @MonedaId, CC_Observ = @Observacion, CC_FechaMod = GETDATE(), Usu_Codigo = @UsuarioId
                                WHERE Conf_Codigo = @EmpresaId AND CC_Codigo = @CuentaCorrienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, cuentaCorriente);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Cuenta_Corriente WHERE Conf_Codigo = @empresaId AND CC_Codigo = @cuentaCorrienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    cuentaCorrienteId = new DbString { Value = splitId.CuentaCorrienteId, IsAnsi = true, IsFixedLength = false, Length = 2 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oCuentaCorriente> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT
	                                Conf_Codigo AS EmpresaId,
	                                CC_Codigo AS CuentaCorrienteId,
	                                Ban_Codigo AS EntidadBancariaId,
	                                CC_Numero AS Numero,
	                                CC_TipoCta AS TipoCuentaDescripcion,
	                                CC_Moneda AS MonedaId,
	                                CC_Observ AS Observacion
                                FROM
                                    Cuenta_Corriente
                                WHERE
	                                Conf_Codigo = @empresaId 
                                    AND CC_Codigo = @cuentaCorrienteId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCuentaCorriente>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    cuentaCorrienteId = new DbString { Value = splitId.CuentaCorrienteId, IsAnsi = true, IsFixedLength = false, Length = 2 }
                });
            }
        }

        public async Task<IEnumerable<vCuentaCorriente>> ListarTodos()
        {
            string query = @"   SELECT
	                                CC.Conf_Codigo AS EmpresaId,
	                                CC.CC_Codigo AS CuentaCorrienteId,
	                                CC.CC_Numero AS Numero,
	                                EB.Ban_Nombre AS EntidadBancariaNombre,
                                    EB.Ban_Tipo AS EntidadBancariaTipo,
	                                CC.CC_Moneda AS MonedaId,
	                                CC.CC_TipoCta AS TipoCuentaDescripcion,
	                                CC.CC_SaldoFin AS SaldoFinal
                                FROM 
                                    Cuenta_Corriente CC
                                    INNER JOIN Entidad_Bancaria EB ON CC.Ban_Codigo = EB.Ban_Codigo
                                ORDER BY
                                    CC.CC_Numero"
            ;

            using (var db = GetConnection())
            {
                return await db.QueryAsync<vCuentaCorriente>(query);
            }
        }

        public async Task<oPagina<vCuentaCorriente>> Listar(string numero, oPaginacion paginacion)
        {
            string query = @$"  SELECT
	                                CC.Conf_Codigo AS EmpresaId,
	                                CC.CC_Codigo AS CuentaCorrienteId,
	                                CC.CC_Numero AS Numero,
	                                EB.Ban_Nombre AS EntidadBancariaNombre,
                                    EB.Ban_Tipo AS EntidadBancariaTipo,
	                                CC.CC_Moneda AS MonedaId,
	                                CC.CC_TipoCta AS TipoCuentaDescripcion,
	                                CC.CC_SaldoFin AS SaldoFinal
                                FROM 
                                    Cuenta_Corriente CC
                                    INNER JOIN Entidad_Bancaria EB ON CC.Ban_Codigo = EB.Ban_Codigo
                                WHERE 
                                    CC.CC_Numero LIKE '%' + @numero + '%'
                                ORDER BY
                                    CC.CC_Numero
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCuentaCorriente> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { numero = new DbString { Value = numero, IsAnsi = true, IsFixedLength = false, Length = 25 } }))
                {
                    pagina = new oPagina<vCuentaCorriente>
                    {
                        Data = await result.ReadAsync<vCuentaCorriente>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = @"SELECT COUNT(Conf_Codigo) FROM Cuenta_Corriente WHERE Conf_Codigo = @empresaId AND CC_Codigo = @cuentaCorrienteId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    cuentaCorrienteId = new DbString { Value = splitId.CuentaCorrienteId, IsAnsi = true, IsFixedLength = false, Length = 2 }
                });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numero)
        {
            oSplitCuentaCorrienteId splitId = id is null ? null : SplitId(id);

            string query = @$"  SELECT 
                                    COUNT(Conf_Codigo) 
                                FROM 
                                    Cuenta_Corriente 
                                WHERE 
                                    {(id is null ? string.Empty : "NOT(Conf_Codigo = @empresaId AND CC_Codigo = @cuentaCorrienteId) AND")}
                                    CC_Numero = @numero";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId?.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    cuentaCorrienteId = new DbString { Value = splitId?.CuentaCorrienteId, IsAnsi = true, IsFixedLength = false, Length = 2 },
                    numero = new DbString { Value = numero, IsAnsi = true, IsFixedLength = false, Length = 25 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId(string empresaId) => await GetNuevoId("SELECT MAX(CC_Codigo) FROM Cuenta_Corriente WHERE Conf_Codigo = @empresaId", new { empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 } }, "0#");

        public static oSplitCuentaCorrienteId SplitId(string id) => new oSplitCuentaCorrienteId(id);
        #endregion
    }
}
