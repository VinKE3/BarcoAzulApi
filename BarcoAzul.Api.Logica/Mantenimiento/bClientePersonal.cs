using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
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
    public class bClientePersonal : bComun, ILogicaService
    {
        public bClientePersonal(IConnectionManager connectionManager) : base(connectionManager, origen: "Cliente - Personal") { }

        public async Task<bool> Registrar(ClientePersonalDTO model)
        {
            try
            {
                var clientePersonal = Mapping.Mapper.Map<oClientePersonal>(model);

                dClientePersonal dClientePersonal = new(GetConnectionString());
                await dClientePersonal.Registrar(clientePersonal);

                if (clientePersonal.Default)
                    await dClientePersonal.ActualizarDefault(model.Id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Eliminar(string id)
        {
            try
            {
                dClientePersonal dClientePersonal = new(GetConnectionString());
                await dClientePersonal.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oClientePersonal> GetPorId(string id)
        {
            try
            {
                dClientePersonal dClientePersonal = new(GetConnectionString());
                var clientePersonal = await dClientePersonal.GetPorId(id);

                clientePersonal.Personal = await new dPersonal(GetConnectionString()).GetPorId(clientePersonal.PersonalId);

                return clientePersonal;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<IEnumerable<oClientePersonal>> ListarPorCliente(string clienteId)
        {
            try
            {
                dClientePersonal dClientePersonal = new(GetConnectionString());
                var clientePersonal = await dClientePersonal.ListarPorCliente(clienteId);

                dPersonal dPersonal = new(GetConnectionString());

                foreach (var p in clientePersonal)
                {
                    p.Personal = await dPersonal.GetPorId(p.PersonalId);
                }

                return clientePersonal;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dClientePersonal(GetConnectionString()).Existe(id);

        public async Task<object> FormularioTablas()
        {
            var personal = await new dPersonal(GetConnectionString()).ListarTodos();

            return new
            {
                personal
            };
        }
    }
}
