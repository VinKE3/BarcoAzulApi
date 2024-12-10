using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dProveedorCuentaCorriente : dComun
    {
        public dProveedorCuentaCorriente(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oProveedorCuentaCorriente proveedorCuentaCorriente)
        {
            string query = "INSERT INTO Proveedor_CtaCte (Prov_Codigo, Cta_Item, Cta_Moneda, Cta_Numero, Ban_Codigo) VALUES (@ProveedorId, @CuentaCorrienteId, @MonedaId, @Numero, @EntidadBancariaId)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, proveedorCuentaCorriente);
            }
        }

        public async Task Modificar(oProveedorCuentaCorriente proveedorCuentaCorriente)
        {
            string query = "UPDATE Proveedor_CtaCte SET Cta_Moneda = @MonedaId, Cta_Numero = @Numero, Ban_Codigo = @EntidadBancariaId WHERE Prov_Codigo = @ProveedorId AND Cta_Item = @CuentaCorrienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, proveedorCuentaCorriente);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = "DELETE Proveedor_CtaCte WHERE Prov_Codigo = @proveedorId AND Cta_Item = @cuentaCorrienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    cuentaCorrienteId = splitId.CuentaCorrienteId
                });
            }
        }

        public async Task EliminarDeProveedor(string proveedorId)
        {
            string query = "DELETE Proveedor_CtaCte WHERE Prov_Codigo = @proveedorId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oProveedorCuentaCorriente> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"  SELECT
                                    Prov_Codigo AS ProveedorId,
                                    Cta_Item AS CuentaCorrienteId,
                                    Cta_Moneda AS MonedaId,
                                    Cta_Numero AS Numero,
                                    Ban_Codigo AS EntidadBancariaId
                                FROM 
                                    Proveedor_CtaCte
                                WHERE 
                                    Prov_Codigo = @proveedorId 
                                    AND Cta_Item = @cuentaCorrienteId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oProveedorCuentaCorriente>(query, new
                {
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    cuentaCorrienteId = splitId.CuentaCorrienteId
                });
            }
        }

        public async Task<IEnumerable<oProveedorCuentaCorriente>> ListarPorProveedor(string proveedorId)
        {
            string query = @"   
                            SELECT
                                PC.Prov_Codigo AS ProveedorId,
                                PC.Cta_Item AS CuentaCorrienteId,
                                PC.Cta_Moneda AS MonedaId,
                                PC.Cta_Numero AS Numero,
                                PC.Ban_Codigo AS EntidadBancariaId,
	                            EB.Ban_Nombre AS EntidadBancariaNombre,
	                            EB.Ban_Tipo AS EntidadBancariaTipo
                            FROM 
                                Proveedor_CtaCte PC
	                            INNER JOIN Entidad_Bancaria EB ON PC.Ban_Codigo = EB.Ban_Codigo
                            WHERE 
                                Prov_Codigo = @proveedorId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oProveedorCuentaCorriente>(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT COUNT(Prov_Codigo) FROM Proveedor_CtaCte WHERE Prov_Codigo = @proveedorId AND Cta_Item = @cuentaCorrienteId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    cuentaCorrienteId = splitId.CuentaCorrienteId
                });
                return existe > 0;
            }
        }

        public async Task<int> GetNuevoId(string proveedorId)
        {
            string query = "SELECT MAX(Cta_Item) FROM Proveedor_CtaCte WHERE Prov_Codigo = @proveedorId";

            using (var db = GetConnection())
            {
                var id = await db.QueryFirstOrDefaultAsync<int?>(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });

                if (id is null)
                    return 1;

                return id.Value + 1;
            }
        }

        public static oSplitProveedorCuentaCorrienteId SplitId(string id) => new(id);
        #endregion
    }
}
