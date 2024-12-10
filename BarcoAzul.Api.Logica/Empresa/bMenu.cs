using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Empresa;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Empresa
{
    public class bMenu : bComun, ILogicaService
    {
        public bMenu(IConnectionManager connectionManager) : base(connectionManager, origen: "Menú") { }

        public async Task Registrar(IEnumerable<oMenu> menus)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dMenu = new dMenu(GetConnectionString());
                    await dMenu.Registrar(menus);

                    scope.Complete();
                }
            }
            catch
            {

            }
        }

        public async Task<IEnumerable<oMenu>> Listar() => await new dMenu(GetConnectionString()).Listar();

        public async Task<bool> Existe(string id) => await new dMenu(GetConnectionString()).Existe(id);
    }
}
