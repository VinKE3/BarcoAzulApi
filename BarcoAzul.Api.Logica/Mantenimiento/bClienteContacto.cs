using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bClienteContacto : bComun, ILogicaService
    {
        public bClienteContacto(IConnectionManager connectionManager) : base(connectionManager, origen: "Cliente - Contacto") { }

        public async Task<bool> Registrar(oClienteContacto model)
        {
            try
            {
                model.ProcesarDatos();

                dClienteContacto dClienteContacto = new(GetConnectionString());
                model.ContactoId = await dClienteContacto.GetNuevoId(model.ClienteId);

                await dClienteContacto.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oClienteContacto model)
        {
            try
            {
                model.ProcesarDatos();

                dClienteContacto dClienteContacto = new(GetConnectionString());
                await dClienteContacto.Modificar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<bool> Eliminar(string id)
        {
            try
            {
                dClienteContacto dClienteContacto = new(GetConnectionString());
                await dClienteContacto.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oClienteContacto> GetPorId(string id)
        {
            try
            {
                dClienteContacto dClienteContacto = new(GetConnectionString());
                return await dClienteContacto.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<IEnumerable<oClienteContacto>> ListarPorCliente(string clienteId)
        {
            try
            {
                dClienteContacto dClienteContacto = new(GetConnectionString());
                return await dClienteContacto.ListarPorCliente(clienteId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dClienteContacto(GetConnectionString()).Existe(id);

        public async Task<object> FormularioTablas()
        {
            var cargos = await new dCargo(GetConnectionString()).ListarTodos();

            return new
            {
                cargos
            };
        }
    }
}
