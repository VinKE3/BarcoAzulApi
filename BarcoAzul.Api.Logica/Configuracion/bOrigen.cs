using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Configuracion;

namespace BarcoAzul.Api.Logica.Configuracion
{

    public class bOrigen : bComun
    {
        public bOrigen(IConnectionManager connectionManager) : base(connectionManager, string.Empty, null) { }

        public async Task<IEnumerable<string>> Listar() => await new dOrigen(GetConnectionString()).Listar();
    }
}
