using BarcoAzul.Api.Modelos.Atributos;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Repositorio.Empresa;
namespace BarcoAzul.Api.Logica
{
    public class bComun
    {
        private readonly IConnectionManager _connectionManager;
        internal readonly string _origen;
        internal readonly oDatosUsuario _datosUsuario;
        internal readonly oConfiguracionGlobal _configuracionGlobal;

        public List<oMensaje> Mensajes { get; private set; }
        public IConnectionManager ConnectionManager { get => _connectionManager; }

        public bComun(IConnectionManager connectionManager, string origen = "", oDatosUsuario datosUsuario = null, oConfiguracionGlobal configuracionGlobal = null)
        {
            Mensajes = new List<oMensaje>();
            _datosUsuario = datosUsuario;
            _connectionManager = connectionManager;
            _origen = origen;
            _configuracionGlobal = configuracionGlobal;
        }

        public string GetConnectionString(string connectionName = "Default") => _connectionManager.GetConnectionString(connectionName);

        /// <summary>
        /// Agrega un mensaje para mostrar en pantalla.
        /// </summary>
        /// <param name="ex">Excepción</param>
        public void ManejarExcepcion(Exception ex)
        {
            if (ex is MensajeException mensajeException)
                Mensajes.Add(mensajeException.Mensaje);
            else
                Mensajes.Add(new oMensaje(MensajeTipo.Error, ex.Message));
        }

        /// <summary>
        /// Agrega un mensaje para mostrar en pantalla. Formato: "{origen}: ocurrión un error al {accion}. Motivo: {ex.Message}"
        /// </summary>
        /// <param name="ex">Excepción</param>
        /// <param name="origen">Origen del mensaje</param>
        /// <param name="accion">Acción que causó el error</param>
        public void ManejarExcepcion(Exception ex, string origen, TipoAccion accion)
        {
            if (ex is MensajeException mensajeException)
                Mensajes.Add(mensajeException.Mensaje);
            else
                Mensajes.Add(new oMensaje(MensajeTipo.Error, $"{origen}: ocurrió un error al {accion.ToString().ToLower()}. Motivo: {ex.Message}"));
        }

        public async Task<bool> AnioMesHabilitado(DateTime fecha)
        {
            try
            {
                if (_configuracionGlobal.AnioHabilitado1 != fecha.Year && _configuracionGlobal.AnioHabilitado2 != fecha.Year)
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"El año {fecha.Year} se encuentra restringido en el sistema."));
                    return false;
                }

                if (!_configuracionGlobal.MesesHabilitados.Contains(fecha.ToString("yyyyMM")))
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"Mes de {dMes.GetPorNumero(fecha.Month).Nombre} no permitido para el año {fecha.Year}."));
                    return false;
                }

                dCerrarMes dCerrarMes = new(GetConnectionString());
                var cierreMes = await dCerrarMes.GetPorAnioMes(fecha.Year, fecha.Month);

                if (cierreMes != null && cierreMes.IsCerrado)
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"Mes cerrado en el sistema ({dMes.GetPorNumero(fecha.Month).Nombre}-{fecha.Year})."));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Mensajes.Add(new oMensaje(MensajeTipo.Error, $"Ocurrió un error al validar el año - mes. Motivo: {ex.Message}"));
                return false;
            }
        }

        public bool IsFechaValida(TipoAccion accion, DateTime fecha)
        {
            if (_configuracionGlobal.FechaUltimoCuadre is null || _configuracionGlobal.FechaUltimoCuadre <= fecha)
                return true;

            int diasTranscurridos = (_configuracionGlobal.FechaUltimoCuadre.Value - fecha).Days;
            Mensajes.Add(new oMensaje(MensajeTipo.Error, $"Hay un cuadre posterior a la fecha. Fecha de cuadre: {fecha:dd/MM/yyyy}. Días transcurridos: {diasTranscurridos}. Imposible {accion.ToString().ToLower()}."));
            return false;
        }

        public async Task<bool> StockSuficiente(IEnumerable<oArticuloValidarStock> articulos)
        {
            try
            {
                int articulosSinStockSuficiente = 0;

                //Quitamos los items que tiene el código de varios
                articulos = articulos.Where(x => x.Id != _configuracionGlobal.DefaultLineaId + _configuracionGlobal.DefaultSubLineaId + _configuracionGlobal.DefaultArticuloId);

                dArticulo dArticulo = new(GetConnectionString());
                await dArticulo.ConsultarStock(articulos);

                foreach (var articulo in articulos)
                {
                    if (!articulo.ControlarStock)
                        continue;

                    if (articulo.Stock < articulo.StockSolicitado)
                    {
                        articulosSinStockSuficiente++;
                        Mensajes.Add(new oMensaje(MensajeTipo.Error, $"El artículo {articulo.Descripcion} no cuenta con stock suficiente (Stock actual: {articulo.Stock})."));
                    }
                }

                return articulosSinStockSuficiente == 0;
            }
            catch (Exception ex)
            {
                Mensajes.Add(new oMensaje(MensajeTipo.Error, $"Error al validar el stock de los artículos. Motivo: {ex.Message}"));
                return false;
            }
        }
    }

    public enum TipoAccion
    {
        Registrar = 0,
        Modificar = 1,
        Eliminar = 2,
        Consultar = 3,
        Anular = 4,
        Listar = 5,
        Exportar = 6
    }
}
