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
    public class dClienteContacto : dComun
    {
        public dClienteContacto(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oClienteContacto clienteContacto)
        {
            string query = @"   INSERT INTO Cliente_Contacto (Cli_codigo, Con_Item, Con_Nombres, Con_Dni, Con_celular, Con_telefono1, Car_Codigo, Con_Direccion, con_correo)
                                VALUES (@ClienteId, @ContactoId, @Nombres, @NumeroDocumentoIdentidad, @Celular, @Telefono, @CargoId, @Direccion, @CorreoElectronico)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, clienteContacto);
            }
        }

        public async Task Modificar(oClienteContacto clienteContacto)
        {
            string query = @"   UPDATE Cliente_Contacto SET Con_Nombres = @Nombres, Con_Dni = @NumeroDocumentoIdentidad, Con_Celular = @Celular, Con_telefono1 = @Telefono,
                                Car_Codigo = @CargoId, Con_Direccion = @Direccion, Con_Correo = @CorreoElectronico WHERE Cli_Codigo = @ClienteId AND Con_Item = @ContactoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, clienteContacto);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Cliente_Contacto WHERE Cli_Codigo = @clienteId AND Con_Item = @contactoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    contactoId = splitId.ContactoId
                });
            }
        }

        public async Task EliminarDeCliente(string clienteId)
        {
            string query = @"DELETE Cliente_Contacto WHERE Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oClienteContacto> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"  SELECT
	                                Cli_Codigo AS ClienteId,
	                                Con_Item AS ContactoId,
	                                Con_Nombres AS Nombres,
	                                Con_Dni AS NumeroDocumentoIdentidad,
	                                Con_Celular AS Celular,
	                                con_Telefono1 AS Telefono,
	                                Car_Codigo AS CargoId,
	                                Con_Correo AS CorreoElectronico,
	                                Con_Direccion AS Direccion
                                FROM
	                                Cliente_Contacto
                                WHERE
	                                Cli_Codigo = @clienteId 
                                    AND Con_Item = @contactoId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oClienteContacto>(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    contactoId = splitId.ContactoId
                });
            }
        }

        public async Task<IEnumerable<oClienteContacto>> ListarPorCliente(string clienteId)
        {
            string query = @"  SELECT
	                                Cli_Codigo AS ClienteId,
	                                Con_Item AS ContactoId,
	                                Con_Nombres AS Nombres,
	                                Con_Dni AS NumeroDocumentoIdentidad,
	                                Con_Celular AS Celular,
	                                con_Telefono1 AS Telefono,
	                                Car_Codigo AS CargoId,
	                                Con_Correo AS CorreoElectronico,
	                                Con_Direccion AS Direccion
                                FROM
	                                Cliente_Contacto
                                WHERE
	                                Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oClienteContacto>(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT COUNT(Cli_Codigo) FROM Cliente_Contacto WHERE Cli_Codigo = @clienteId AND Con_Item = @contactoId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    contactoId = splitId.ContactoId
                });
                return existe > 0;
            }
        }

        public async Task<int> GetNuevoId(string clienteId)
        {
            string query = "SELECT MAX(Con_Item) FROM Cliente_Contacto WHERE Cli_codigo = @clienteId";

            using (var db = GetConnection())
            {
                var id = await db.QueryFirstOrDefaultAsync<int?>(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });

                if (id is null)
                    return 1;

                return id.Value + 1;
            }
        }

        public static oSplitClienteContactoId SplitId(string id) => new(id);
        #endregion
    }
}
