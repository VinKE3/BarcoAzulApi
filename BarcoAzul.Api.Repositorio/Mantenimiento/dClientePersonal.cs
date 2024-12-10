using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dClientePersonal : dComun
    {
        public dClientePersonal(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oClientePersonal clientePersonal)
        {
            string query = @"   INSERT INTO Cliente_Personal(Cli_Codigo, Per_Codigo, DVend_FechaReg, DVend_PorDefecto)
                                VALUES (@ClienteId, @PersonalId, GETDATE(), @Default)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    clientePersonal.ClienteId,
                    clientePersonal.PersonalId,
                    Default = clientePersonal.Default ? "S" : "N"
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = "DELETE Cliente_Personal WHERE Cli_Codigo = @clienteId AND Per_Codigo = @personalId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    personalId = new DbString { Value = splitId.PersonalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                });
            }
        }

        public async Task EliminarDeCliente(string clienteId)
        {
            string query = "DELETE Cliente_Personal WHERE Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oClientePersonal> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT
	                                Cli_Codigo AS ClienteId,
	                                Per_Codigo AS PersonalId,
	                                CAST(CASE WHEN DVend_PorDefecto = 'S' THEN 1 ELSE 0 END AS BIT) AS 'Default'
                                FROM 
                                    Cliente_Personal
                                WHERE 
                                    Cli_Codigo = @clienteId 
                                    AND Per_Codigo = @personalId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oClientePersonal>(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    personalId = new DbString { Value = splitId.PersonalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                });
            }
        }

        public async Task<IEnumerable<oClientePersonal>> ListarPorCliente(string clienteId)
        {
            string query = @"   SELECT
	                                Cli_Codigo AS ClienteId,
	                                Per_Codigo AS PersonalId,
	                                CAST(CASE WHEN DVend_PorDefecto = 'S' THEN 1 ELSE 0 END AS BIT) AS 'Default'
                                FROM 
                                    Cliente_Personal
                                WHERE 
                                    Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oClientePersonal>(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT COUNT(Cli_Codigo) FROM Cliente_Personal WHERE Cli_Codigo = @clienteId AND Per_Codigo = @personalId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    personalId = new DbString { Value = splitId.PersonalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                });
                return existe > 0;
            }
        }

        public async Task ActualizarDefault(string id)
        {
            var splitId = SplitId(id);
            string query = "UPDATE Cliente_Personal SET Dvend_PorDefecto = 'S' WHERE Cli_Codigo = @clienteId AND Per_Codigo = @personalId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    personalId = new DbString { Value = splitId.PersonalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                });

                query = "UPDATE Cliente_Personal SET Dvend_PorDefecto = 'N' WHERE NOT(Cli_Codigo = @clienteId AND Per_Codigo = @personalId) AND Cli_Codigo = @clienteId";

                await db.ExecuteAsync(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    personalId = new DbString { Value = splitId.PersonalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                });
            }
        }

        public static oSplitClientePersonalId SplitId(string id) => new(id);
        #endregion
    }
}
