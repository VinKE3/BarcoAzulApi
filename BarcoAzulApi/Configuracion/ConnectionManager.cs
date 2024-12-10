using BarcoAzul.Api.Modelos.Interfaces;

namespace BarcoAzulApi.Configuracion
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IConfiguration _configuration;

        public ConnectionManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string connectionName = "Default") => _configuration.GetConnectionString(connectionName);
    }
}
