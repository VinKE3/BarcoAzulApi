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

        public string GetConnectionString(string connectionName = "http://localhost:5173") => _configuration.GetConnectionString(connectionName);
    }
}
