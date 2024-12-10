namespace BarcoAzul.Api.Modelos.Interfaces
{
    public interface IConnectionManager
    {
        string GetConnectionString(string connectionName);
    }
}
