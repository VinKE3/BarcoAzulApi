using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dVehiculo : dComun
    {
        public dVehiculo(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oVehiculo vehiculo)
        {
            string query = @"   INSERT INTO Vehiculo (Veh_Codigo, Conf_Codigo, Tran_Codigo, Veh_Placa, Veh_Marca, Veh_Modelo, Veh_CertifInsc, Veh_Observacion)
                                VALUES (@Id, @EmpresaId, @EmpresaTransporteId, @NumeroPlaca, @Marca, @Modelo, @CertificadoInscripcion, @Observacion)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, vehiculo);
            }
        }

        public async Task Modificar(oVehiculo vehiculo)
        {
            string query = @"   UPDATE Vehiculo SET Conf_Codigo = @EmpresaId, Tran_Codigo = @EmpresaTransporteId, Veh_Placa = @NumeroPlaca, Veh_Marca = @Marca, 
                                Veh_Modelo = @Modelo, Veh_CertifInsc = @CertificadoInscripcion, Veh_Observacion = @Observacion WHERE Veh_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, vehiculo);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Vehiculo WHERE Veh_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 3 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oVehiculo> GetPorId(string id)
        {
            string query = @"   SELECT
                                    Veh_Codigo AS Id,
                                    Conf_Codigo AS EmpresaId,
                                    Tran_Codigo AS EmpresaTransporteId,
                                    Veh_Placa AS NumeroPlaca,
                                    Veh_Marca AS Marca,
                                    Veh_Modelo AS Modelo,
                                    Veh_CertifInsc AS CertificadoInscripcion,
                                    Veh_Observacion AS Observacion
                                FROM
                                    Vehiculo
                                WHERE
                                    Veh_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oVehiculo>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 3 } });
            }
        }

        public async Task<IEnumerable<vVehiculo>> ListarTodos()
        {
            string query = @"   SELECT
	                                Codigo As Id,
	                                Placa_Rodaje AS NumeroPlaca,
	                                Marca,
	                                Constancia_Inscripcion AS CertificadoInscripcion,
	                                Tran_Codigo AS EmpresaTransporteId,
	                                Empresa_Transporte AS EmpresaTransporteNombre
                                FROM
	                                v_lst_vehiculo
                                ORDER BY
	                                Placa_Rodaje";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<vVehiculo>(query);
            }
        }

        public async Task<oPagina<vVehiculo>> Listar(string numeroPlaca, oPaginacion paginacion)
        {
            string query = $@"   SELECT
	                                Codigo As Id,
	                                Placa_Rodaje AS NumeroPlaca,
	                                Marca,
	                                Constancia_Inscripcion AS CertificadoInscripcion,
                                    Tran_Codigo AS EmpresaTransporteId,
	                                Empresa_Transporte AS EmpresaTransporteNombre
                                FROM
	                                v_lst_vehiculo
                                WHERE
                                    Placa_Rodaje LIKE '%' + @numeroPlaca + '%'
                                ORDER BY
	                                Placa_Rodaje
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vVehiculo> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { numeroPlaca = new DbString { Value = numeroPlaca, IsAnsi = true, IsFixedLength = false, Length = 20 } }))
                {
                    pagina = new oPagina<vVehiculo>
                    {
                        Data = await result.ReadAsync<vVehiculo>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Veh_Codigo) FROM Vehiculo WHERE Veh_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 3 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numeroPlaca)
        {
            string query = @$"   SELECT
                                    COUNT(Veh_Codigo)
                                FROM
                                    Vehiculo
                                WHERE 
                                    {(id is null ? string.Empty : "Veh_Codigo <> @id AND ")}
                                    Veh_Placa = @numeroPlaca";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 3 },
                    numeroPlaca = new DbString { Value = numeroPlaca, IsAnsi = true, IsFixedLength = false, Length = 20 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Veh_Codigo) FROM Vehiculo", null, "000");
        #endregion
    }
}
