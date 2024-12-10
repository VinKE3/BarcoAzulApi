using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dEmpresa : dComun
    {
        public dEmpresa(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Modificar(oConfiguracionEmpresa configuracionEmpresa)
        {
            string query = @"   UPDATE Conf_Empresa SET Conf_Ruc = @NumeroDocumentoIdentidad, Conf_RazonSocial = @Nombre, Conf_Direccion = @Direccion,
                                Dep_Codigo = @DepartamentoId, Pro_Codigo = @ProvinciaId, Dis_Codigo = @DistritoId, Conf_tefefono = @Telefono, Conf_TeleFax = @Celular,
                                Conf_Celular = @CorreoElectronico, Conf_Observ = @Observacion, Conf_FechaIni = @FiltroFechaInicio, Conf_FechaFin = @FiltroFechaFin,
                                Conf_year = @AnioHabilitado1, Conf_Año = @AnioHabilitado2, Conf_Meses = @MesesHabilitados,
                                Conf_Ene = @Enero1, Conf_Feb = @Febrero1, Conf_Mar = @Marzo1, Conf_Abr = @Abril1, Conf_May = @Mayo1, Conf_Jun = @Junio1,
                                Conf_Jul = @Julio1, Conf_Ago = @Agosto1, Conf_Sep = @Septiembre1, Conf_Oct = @Octubre1, Conf_Nov = @Noviembre1, Conf_Dic = @Diciembre1,
                                Conf_Ene01 = @Enero2, Conf_Feb01 = @Febrero2, Conf_Mar01 = @Marzo2, Conf_Abr01 = @Abril2, Conf_May01 = @Mayo2, Conf_Jun01 = @Junio2,
                                Conf_Jul01 = @Julio2, Conf_Ago01 = @Agosto2, Conf_Sep01 = @Septiembre2, Conf_Oct01 = @Octubre2, Conf_Nov01 = @Noviembre2, Conf_Dic01 = @Diciembre2,
                                Conf_Almacen = @ConcarEmpresaId, Conf_Via = @ConcarEmpresaNombre, Conf_Interior = @ConcarUsuarioVenta, Conf_Numero = @ConcarUsuarioCompra,
                                Conf_Pago = @ConcarUsuarioPago, Conf_Zona = @ConcarUsuarioCobro";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    configuracionEmpresa.NumeroDocumentoIdentidad,
                    configuracionEmpresa.Nombre,
                    configuracionEmpresa.Direccion,
                    configuracionEmpresa.DepartamentoId,
                    configuracionEmpresa.ProvinciaId,
                    configuracionEmpresa.DistritoId,
                    configuracionEmpresa.Telefono,
                    configuracionEmpresa.Celular,
                    configuracionEmpresa.CorreoElectronico,
                    configuracionEmpresa.Observacion,
                    configuracionEmpresa.FiltroFechaInicio,
                    configuracionEmpresa.FiltroFechaFin,
                    configuracionEmpresa.AnioHabilitado1,
                    configuracionEmpresa.AnioHabilitado2,
                    configuracionEmpresa.MesesHabilitados,
                    Enero1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}01"),
                    Febrero1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}02"),
                    Marzo1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}03"),
                    Abril1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}04"),
                    Mayo1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}05"),
                    Junio1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}06"),
                    Julio1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}07"),
                    Agosto1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}08"),
                    Septiembre1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}09"),
                    Octubre1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}10"),
                    Noviembre1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}11"),
                    Diciembre1 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado1}12"),
                    Enero2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}01"),
                    Febrero2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}02"),
                    Marzo2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}03"),
                    Abril2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}04"),
                    Mayo2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}05"),
                    Junio2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}06"),
                    Julio2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}07"),
                    Agosto2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}08"),
                    Septiembre2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}09"),
                    Octubre2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}10"),
                    Noviembre2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}11"),
                    Diciembre2 = configuracionEmpresa.MesesHabilitados.Contains($"{configuracionEmpresa.AnioHabilitado2}12"),
                    configuracionEmpresa.ConcarEmpresaId,
                    configuracionEmpresa.ConcarEmpresaNombre,
                    configuracionEmpresa.ConcarUsuarioVenta,
                    configuracionEmpresa.ConcarUsuarioCompra,
                    configuracionEmpresa.ConcarUsuarioPago,
                    configuracionEmpresa.ConcarUsuarioCobro
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oConfiguracionEmpresa> Get()
        {
            string query = @"   SELECT
	                                Conf_Codigo AS Id,
	                                Conf_Ruc AS NumeroDocumentoIdentidad,
	                                Conf_RazonSocial AS Nombre,
	                                Conf_Direccion AS Direccion,
	                                Dep_Codigo AS DepartamentoId,
	                                Pro_Codigo AS ProvinciaId,
	                                Dis_Codigo AS DistritoId,
	                                Conf_Tefefono AS Telefono,
	                                Conf_TeleFax AS Celular,
	                                Conf_Celular AS CorreoElectronico,
	                                Conf_Observ As Observacion,
	                                Conf_FechaIni AS FiltroFechaInicio,
	                                Conf_FechaFin AS FiltroFechaFin,
	                                Conf_year AS AnioHabilitado1,
	                                Conf_Año AS AnioHabilitado2,
	                                Conf_Meses AS MesesHabilitados,
                                    Conf_Almacen AS ConcarEmpresaId, 
                                    Conf_Via AS ConcarEmpresaNombre, 
                                    Conf_Interior AS ConcarUsuarioVenta, 
                                    Conf_Numero AS ConcarUsuarioCompra,
                                    Conf_Pago AS ConcarUsuarioPago, 
                                    Conf_Zona AS ConcarUsuarioCobro
                                FROM 
	                                Conf_Empresa";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oConfiguracionEmpresa>(query);
            }
        }

        public async Task ActualizarFechaFiltro()
        {
            string query = @"UPDATE Conf_Empresa SET Conf_FechaFin = @Fin, Conf_FechaAct = @Actual WHERE Conf_Codigo = @EmpresaId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    Fin = DateTime.Today,
                    Actual = DateTime.Today,
                    EmpresaId = new DbString { Value = "01", IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }
        #endregion
    }
}
