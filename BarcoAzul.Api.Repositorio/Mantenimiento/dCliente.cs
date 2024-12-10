﻿using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dCliente : dComun
    {
        public dCliente(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oCliente cliente)
        {
            string query = @"   INSERT INTO Cliente (Cli_Codigo, Cli_Ruc, Cli_RazonSocial, Cli_Telefono, Cli_Correo, Cli_Direccion, Dep_Codigo, Pro_Codigo, Dis_Codigo, 
                                Car_Codigo, Ban_Codigo, Cli_CreditoSol, Cli_CreditoDol, Cli_MaxCredSol, Cli_MaxCredDol, Cli_FechaReg, Usu_Codigo, Cli_DescPorc, 
		                        Cli_DescGral, Cli_Moneda, Cli_direcc, Cli_TipoDoc, Zon_Codigo, Cli_Telefax, Cli_Observacion, Cli_TVenta, Cli_TPago)
		                        VALUES (@Id, @NumeroDocumentoIdentidad, @Nombre, @Telefono, @CorreoElectronico, @DireccionPrincipal, @DepartamentoId, @ProvinciaId, @DistritoId,
		                        1, 1, @CreditoPEN, @CreditoUSD, @MaximoCreditoPEN, @MaximoCreditoUSD, GETDATE(), @UsuarioId, 0, 
		                        0, 'S', null, @TipoDocumentoIdentidadId, @ZonaId, @Celular, @Observacion, @TipoVentaId, @TipoCobroId)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, cliente);
            }
        }

        public async Task Modificar(oCliente cliente)
        {
            string query = @"   UPDATE 
                                    Cliente
                                SET
	                                Cli_TipoDoc = @TipoDocumentoIdentidadId,
	                                Cli_Ruc = @NumeroDocumentoIdentidad,
	                                Cli_RazonSocial = @Nombre,
	                                Cli_Telefono = @Telefono,
	                                Cli_Correo = @CorreoElectronico,
	                                Cli_Direccion = @DireccionPrincipal,
	                                Dep_Codigo = @DepartamentoId,
	                                Pro_Codigo = @ProvinciaId,
	                                Dis_Codigo = @DistritoId,
	                                Zon_Codigo = @ZonaId,
	                                Cli_Telefax = @Celular,
	                                Cli_MaxCredDol = @MaximoCreditoUSD,
	                                Cli_MaxCredSol = @MaximoCreditoPEN,
	                                Cli_CreditoDol = @CreditoUSD,
	                                Cli_CreditoSol = @CreditoPEN,
	                                Cli_Observacion = @Observacion,
	                                Cli_TVenta = @TipoVentaId,
                                    Cli_TPago = @TipoCobroId,
	                                Usu_Codigo = @UsuarioId
                                WHERE 
                                    Cli_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, cliente);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Cliente WHERE Cli_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oCliente> GetPorId(string id)
        {
            string query = @"   SELECT
	                                Cli_Codigo AS Id,
	                                Cli_TipoDoc AS TipoDocumentoIdentidadId,
	                                RTRIM(Cli_Ruc) AS NumeroDocumentoIdentidad,
	                                Cli_RazonSocial AS Nombre,
	                                Cli_Telefono AS Telefono,
	                                Cli_Correo AS CorreoElectronico,
	                                Cli_Direccion AS DireccionPrincipal,
	                                Dep_Codigo AS DepartamentoId,
	                                Pro_Codigo AS ProvinciaId,
	                                Dis_Codigo AS DistritoId,
	                                Zon_Codigo AS ZonaId,
	                                Cli_Telefax AS Celular,
	                                Cli_MaxCredDol AS MaximoCreditoUSD,
	                                Cli_MaxCredSol AS MaximoCreditoPEN,
	                                Cli_CreditoDol AS CreditoUSD,
	                                Cli_CreditoSol AS CreditoPEN,
	                                Cli_Observacion AS Observacion,
                                    Cli_TVenta AS TipoVentaId,
                                    Cli_TPago AS TipoCobroId
                                FROM 
                                    Cliente
                                WHERE 
                                    Cli_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCliente>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<oPagina<vCliente>> Listar(string numeroDocumentoIdentidad, string nombre, oPaginacion paginacion)
        {
            string query = $@"  SELECT
	                                Codigo AS Id,
	                                RTRIM(Ruc) AS NumeroDocumentoIdentidad,
	                                RTRIM(Razon_Social) AS Nombre,
	                                Direccion AS DireccionPrincipal,
	                                Distrito,
	                                Personal AS PersonalNombreCompleto
                                FROM 
                                    v_lst_cliente
                                WHERE   
                                    Ruc LIKE '%' + @numeroDocumentoIdentidad + '%' 
                                    AND Razon_Social LIKE '%' + @nombre + '%'
                                ORDER BY
                                    Codigo
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCliente> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    numeroDocumentoIdentidad,
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 250 }
                }))
                {
                    pagina = new oPagina<vCliente>
                    {
                        Data = await result.ReadAsync<vCliente>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Cli_Codigo) FROM Cliente WHERE Cli_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad)
        {
            string query = $"SELECT COUNT(Cli_Codigo) FROM Cliente WHERE {(id is null ? string.Empty : "Cli_Codigo <> @id AND")} Cli_Ruc = @numeroDocumentoIdentidad";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumentoIdentidad
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Cli_Codigo) FROM Cliente", null, "00000#");

        public async Task<(decimal CreditoPEN, decimal CreditoUSD)> GetCredito(string id)
        {
            string query = @"   SELECT 
	                                SUM(Ven_Saldo)
                                FROM 
	                                Venta 
                                WHERE 
	                                Cli_Codigo = @id 
	                                AND Ven_Moneda = @monedaId
	                                AND Ven_Saldo <> 0 
	                                AND TDoc_Codigo IN ('FT', 'BV', 'TK', 'RC', 'ND', 'NC', 'LC', 'PE', 'RE', 'RH', 'NV', 'PD', 'PR', 'CR')";

            using (var db = GetConnection())
            {
                var creditoPEN = await db.QueryFirstOrDefaultAsync<decimal?>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    monedaId = new DbString { Value = "S", IsAnsi = true, IsFixedLength = true, Length = 1 }
                }) ?? 0;

                var creditoUSD = await db.QueryFirstOrDefaultAsync<decimal?>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    monedaId = new DbString { Value = "D", IsAnsi = true, IsFixedLength = true, Length = 1 }
                }) ?? 0;

                return (creditoPEN, creditoUSD);
            }
        }

        public async Task<(decimal CreditoPEN, decimal CreditoUSD)> GetCreditoDisponible(string id)
        {
            string query = "SELECT ISNULL(Cli_MaxCredSol, 0) - ISNULL(Cli_CreditoSol, 0), ISNULL(Cli_MaxCredDol, 0) - ISNULL(Cli_CreditoDol, 0) FROM Cliente WHERE Cli_Codigo = @id";

            using (var db = GetConnection())
            {
                var (creditoPEN, creditoUSD) = await db.QueryFirstAsync<(decimal? CreditoPEN, decimal? CreditoUSD)>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
                return (creditoPEN ?? 0, creditoUSD ?? 0);
            }
        }
        #endregion
    }
}