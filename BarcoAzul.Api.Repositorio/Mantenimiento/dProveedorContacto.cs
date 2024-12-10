using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dProveedorContacto : dComun
    {
        public dProveedorContacto(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oProveedorContacto proveedorContacto)
        {
            string query = @"   INSERT INTO Proveedor_Contacto (Prov_Codigo, Con_Item, Con_Nombres, Con_Dni, Con_celular, Con_telefono1, Car_Codigo, Con_Direccion, con_correo)
                                VALUES (@ProveedorId, @ContactoId, @Nombres, @NumeroDocumentoIdentidad, @Celular, @Telefono, @CargoId, @Direccion, @CorreoElectronico)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, proveedorContacto);
            }
        }

        public async Task Modificar(oProveedorContacto proveedorContacto)
        {
            string query = @"   UPDATE Proveedor_Contacto SET Con_Nombres = @Nombres, Con_Dni = @NumeroDocumentoIdentidad, Con_Celular = @Celular, Con_telefono1 = @Telefono,
                                Car_Codigo = @CargoId, Con_Direccion = @Direccion, Con_Correo = @CorreoElectronico WHERE Prov_Codigo = @ProveedorId AND Con_Item = @ContactoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, proveedorContacto);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Proveedor_Contacto WHERE Prov_Codigo = @proveedorId AND Con_Item = @contactoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    contactoId = splitId.ContactoId
                });
            }
        }

        public async Task EliminarDeProveedor(string proveedorId)
        {
            string query = @"DELETE Proveedor_Contacto WHERE Prov_Codigo = @proveedorId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oProveedorContacto> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"  SELECT
	                                Prov_Codigo AS ProveedorId,
	                                Con_Item AS ContactoId,
	                                Con_Nombres AS Nombres,
	                                Con_Dni AS NumeroDocumentoIdentidad,
	                                Con_Celular AS Celular,
	                                con_Telefono1 AS Telefono,
	                                Car_Codigo AS CargoId,
	                                Con_Correo AS CorreoElectronico,
	                                Con_Direccion AS Direccion
                                FROM
	                                Proveedor_Contacto
                                WHERE
	                                Prov_Codigo = @proveedorId 
                                    AND Con_Item = @contactoId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oProveedorContacto>(query, new
                {
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    contactoId = splitId.ContactoId
                });
            }
        }

        public async Task<IEnumerable<oProveedorContacto>> ListarPorProveedor(string proveedorId)
        {
            string query = @"  SELECT
	                                Prov_Codigo AS ProveedorId,
	                                Con_Item AS ContactoId,
	                                Con_Nombres AS Nombres,
	                                Con_Dni AS NumeroDocumentoIdentidad,
	                                Con_Celular AS Celular,
	                                con_Telefono1 AS Telefono,
	                                Car_Codigo AS CargoId,
	                                Con_Correo AS CorreoElectronico,
	                                Con_Direccion AS Direccion
                                FROM
	                                Proveedor_Contacto
                                WHERE
	                                Prov_Codigo = @proveedorId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oProveedorContacto>(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT COUNT(Prov_Codigo) FROM Proveedor_Contacto WHERE Prov_Codigo = @proveedorId AND Con_Item = @contactoId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    contactoId = splitId.ContactoId
                });
                return existe > 0;
            }
        }

        public async Task<int> GetNuevoId(string proveedorId)
        {
            string query = "SELECT MAX(Con_Item) FROM Proveedor_Contacto WHERE Prov_Codigo = @proveedorId";

            using (var db = GetConnection())
            {
                var id = await db.QueryFirstOrDefaultAsync<int?>(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });

                if (id is null)
                    return 1;

                return id.Value + 1;
            }
        }

        public static oSplitProveedorContactoId SplitId(string id) => new(id);
        #endregion
    }
}
