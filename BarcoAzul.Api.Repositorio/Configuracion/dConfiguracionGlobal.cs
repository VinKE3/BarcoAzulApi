using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Configuracion
{
    public class dConfiguracionGlobal : dComun
    {
        public dConfiguracionGlobal(string connectionString) : base(connectionString) { }

        public async Task<oConfiguracionGlobal> Get()
        {
            string query = @"   SELECT 
	                                Conf_Codigo AS EmpresaId,
	                                Conf_Ruc AS EmpresaNumeroDocumentoIdentidad,
	                                Conf_RazonSocial AS EmpresaNombre,
	                                Conf_Direccion AS EmpresaDireccion,
	                                Dep_Codigo AS EmpresaDepartamentoId,
	                                Pro_Codigo AS EmpresaProvinciaId,
	                                Dis_Codigo AS EmpresaDistritoId,
	                                Conf_FechaIni AS FiltroFechaInicio,
	                                Conf_FechaFin AS FiltroFechaFin,
                                    Conf_UsuarioId AS DefaultUsuarioId,
	                                Conf_PorcIgv AS DefaultPorcentajeIgv,
	                                Conf_Cliente AS DefaultClienteId,
	                                Conf_Proveedor AS DefaultProveedorId,
	                                Conf_Personal AS DefaultPersonalId,
	                                Conf_Articulo AS DefaultArticuloId,
                                    Conf_LineaId AS DefaultLineaId,
                                    Conf_SubLineaId AS DefaultSubLineaId,
                                    Conf_MarcaId AS DefaultMarcaId,
	                                Conf_Transport AS DefaultConductorId,
                                    Conf_Year AS AnioHabilitado1,
                                    Conf_Año AS AnioHabilitado2,
                                    Conf_Meses AS MesesHabilitados
                                FROM 
                                    Conf_Empresa";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oConfiguracionGlobal>(query);
            }
        }
    }
}
