using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dConductor : dComun
    {
        public dConductor(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oConductor conductor)
        {
            string query = @"   INSERT INTO Transportista (Tra_Codigo, Conf_Codigo, Tran_Codigo, Tra_Tipo, Tra_RazonSocial, Tra_Apellidos, Tra_TipoDocIde, Tra_RucDni, Tra_Licencia, Tra_Telefono,
                                Tra_TeleFax, Tra_Correo, Tra_Direccion, Dep_Codigo, Pro_Codigo, Dis_codigo, Tra_nroregistrotransp, Tra_FechaReg)
                                VALUES (@Id, @EmpresaId, @EmpresaTransporteId, @TipoConductor, @Nombre, @Apellidos, @TipoDocumentoIdentidadId, @NumeroDocumentoIdentidad, @LicenciaConducir, @Telefono,
                                @Celular, @CorreoElectronico, @Direccion, @DepartamentoId, @ProvinciaId, @DistritoId, @NroRegistro, GETDATE())";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    conductor.Id,
                    conductor.EmpresaId,
                    conductor.EmpresaTransporteId,
                    conductor.TipoConductor,
                    conductor.Nombre,
                    conductor.Apellidos, // nuevo
                    conductor.TipoDocumentoIdentidadId, // nuevo
                    conductor.NumeroDocumentoIdentidad,
                    conductor.LicenciaConducir,
                    conductor.Telefono,
                    conductor.Celular,
                    conductor.CorreoElectronico,
                    conductor.Direccion,
                    conductor.DepartamentoId,
                    conductor.ProvinciaId,
                    conductor.DistritoId,
                    conductor.NroRegistro // nuevo
                });
            }
        }

        public async Task Modificar(oConductor conductor)
        {
            string query = @"UPDATE Transportista SET 
                        Conf_Codigo = @EmpresaId, 
                        Tran_Codigo = @EmpresaTransporteId, 
                        Tra_Tipo = @TipoConductor, 
                        Tra_RazonSocial = @Nombre, 
                        Tra_Apellidos = @Apellidos, 
                        Tra_TipoDocIde = @TipoDocumentoIdentidadId, 
                        Tra_RucDni = @NumeroDocumentoIdentidad, 
                        Tra_Licencia = @LicenciaConducir, 
                        Tra_Telefono = @Telefono, 
                        Tra_TeleFax = @Celular, 
                        Tra_Correo = @CorreoElectronico, 
                        Tra_Direccion = @Direccion, 
                        Dep_Codigo = @DepartamentoId, 
                        Pro_Codigo = @ProvinciaId, 
                        Dis_codigo = @DistritoId, 
                        Tra_nroregistrotransp = @NroRegistro, 
                        Tra_FechaMod = GETDATE()
                    WHERE Tra_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    conductor.EmpresaId,
                    conductor.EmpresaTransporteId,
                    conductor.TipoConductor,
                    conductor.Nombre,
                    conductor.Apellidos,
                    conductor.TipoDocumentoIdentidadId,
                    conductor.NumeroDocumentoIdentidad,
                    conductor.LicenciaConducir,
                    conductor.Telefono,
                    conductor.Celular,
                    conductor.CorreoElectronico,
                    conductor.Direccion,
                    conductor.DepartamentoId,
                    conductor.ProvinciaId,
                    conductor.DistritoId,
                    conductor.NroRegistro, // Número de registro del transportista
                    conductor.Id           // Identificador del conductor
                });
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Transportista WHERE Tra_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oConductor> GetPorId(string id)
        {
            string query = @"SELECT 
                        Tra_Codigo AS Id, 
                        Conf_Codigo AS EmpresaId, 
                        Tran_Codigo AS EmpresaTransporteId, 
                        Tra_Tipo AS TipoConductor, 
                        Tra_RazonSocial AS Nombre, 
                        Tra_Apellidos AS Apellidos, 
                        Tra_TipoDocIde AS TipoDocumentoIdentidadId, 
                        Tra_RucDni AS NumeroDocumentoIdentidad, 
                        Tra_Licencia AS LicenciaConducir, 
                        Tra_Telefono AS Telefono, 
                        Tra_TeleFax AS Celular, 
                        Tra_Correo AS CorreoElectronico, 
                        Tra_Direccion AS Direccion, 
                        Dep_Codigo AS DepartamentoId, 
                        Pro_Codigo AS ProvinciaId, 
                        Dis_codigo AS DistritoId, 
                        Tra_nroregistrotransp AS NroRegistro 
                    FROM Transportista
                    WHERE Tra_Codigo = @Id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oConductor>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<IEnumerable<vConductor>> ListarTodos()
        {
            string query = @"   SELECT
	                                Codigo AS Id,
	                                Razon_Social AS Nombre,
	                                RucDni AS NumeroDocumentoIdentidad,
	                                Licencia AS LicenciaConducir,
	                                Tran_Codigo AS EmpresaTransporteId
                                FROM
	                                v_lst_transportista";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<vConductor>(query);
            }
        }

        public async Task<oPagina<vConductor>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = $@"   SELECT
	                                Codigo AS Id,
	                                Razon_Social AS Nombre,
	                                RucDni AS NumeroDocumentoIdentidad,
	                                Licencia AS LicenciaConducir,
                                    Tran_Codigo AS EmpresaTransporteId,
                                    TipoConductor
                                FROM
	                                v_lst_transportista
                                WHERE
                                    Razon_Social LIKE '%' + @nombre + '%'
                                ORDER BY
                                    Razon_Social
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vConductor> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<vConductor>
                    {
                        Data = await result.ReadAsync<vConductor>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Tra_Codigo) FROM Transportista WHERE Tra_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad)
        {
            string query = $@"SELECT COUNT(Tra_Codigo) FROM Transportista WHERE {(id is null ? string.Empty : "Tra_Codigo <> @id AND ")} Tra_RucDni = @numeroDocumentoIdentidad";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumentoIdentidad = new DbString { Value = numeroDocumentoIdentidad, IsAnsi = true, IsFixedLength = false, Length = 11 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Codigo) FROM v_lst_transportista", null, "000000");
        #endregion
    }
}
