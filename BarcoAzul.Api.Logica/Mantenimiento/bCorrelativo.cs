using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bCorrelativo : bComun, ILogicaService
    {
        public bCorrelativo(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Correlativo", configuracionGlobal: configuracionGlobal) { }

        public async Task<bool> Registrar(oCorrelativo model)
        {
            try
            {
                model.ProcesarDatos();
                model.EmpresaId = _configuracionGlobal.EmpresaId;

                dCorrelativo dCorrelativo = new(GetConnectionString());
                await dCorrelativo.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oCorrelativo model)
        {
            try
            {
                model.ProcesarDatos();
                model.EmpresaId = _configuracionGlobal.EmpresaId;

                dCorrelativo dCorrelativo = new(GetConnectionString());
                await dCorrelativo.Modificar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<oCorrelativo> GetPorId(string tipoDocumentoId, string serie)
        {
            try
            {
                dCorrelativo dCorrelativo = new(GetConnectionString());
                return await dCorrelativo.GetPorId(_configuracionGlobal.EmpresaId, tipoDocumentoId, serie);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<oCorrelativo>> Listar(string[] tiposDocumento, oPaginacion paginacion)
        {
            try
            {
                tiposDocumento ??= TiposDocumentoPermitidos();

                dCorrelativo dCorrelativo = new(GetConnectionString());
                return await dCorrelativo.Listar(tiposDocumento, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string tipoDocumentoId, string serie) => await new dCorrelativo(GetConnectionString()).Existe(_configuracionGlobal.EmpresaId, tipoDocumentoId, serie);

        public async Task<object> FormularioTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(TiposDocumentoPermitidos());

            return new
            {
                tiposDocumento
            };
        }

        private static string[] TiposDocumentoPermitidos() => new string[] { "01", "03", "07", "08", "09", "LC", "NV", "CT" };
    }
}
