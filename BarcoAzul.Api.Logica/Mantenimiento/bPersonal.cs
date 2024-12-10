using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bPersonal : bComun, ILogicaService
    {
        public bPersonal(oDatosUsuario datosUsuario, IConnectionManager connectionManager) : base(connectionManager, origen: "Personal", datosUsuario) { }

        public async Task<bool> Registrar(PersonalDTO model)
        {
            try
            {
                var personal = Mapping.Mapper.Map<oPersonal>(model);

                personal.ProcesarDatos();
                personal.UsuarioId = _datosUsuario.Id;

                dPersonal dPersonal = new(GetConnectionString());
                personal.Id = await dPersonal.GetNuevoId(personal);
                model.Id = personal.Id;

                await dPersonal.Registrar(personal);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(PersonalDTO model)
        {
            try
            {
                var personal = Mapping.Mapper.Map<oPersonal>(model);

                personal.ProcesarDatos();
                personal.UsuarioId = _datosUsuario.Id;

                dPersonal dPersonal = new(GetConnectionString());
                await dPersonal.Modificar(personal);

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
                dPersonal dPersonal = new(GetConnectionString());
                await dPersonal.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oPersonal> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dPersonal dPersonal = new(GetConnectionString());
                var personal = await dPersonal.GetPorId(id);

                if (incluirReferencias)
                {
                    if (!string.IsNullOrEmpty(personal.DepartamentoId))
                        personal.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(personal.DepartamentoId);

                    if (!string.IsNullOrEmpty(personal.ProvinciaId))
                        personal.Provincia = await new dProvincia(GetConnectionString()).GetPorId(personal.DepartamentoId + personal.ProvinciaId);

                    if (!string.IsNullOrEmpty(personal.DistritoId))
                        personal.Distrito = await new dDistrito(GetConnectionString()).GetPorId(personal.DepartamentoId + personal.ProvinciaId + personal.DistritoId);

                    personal.Sexo = dSexo.GetPorId(personal.SexoId);
                    personal.EstadoCivil = dEstadoCivil.GetPorId(personal.EstadoCivilId);

                    if (personal.CargoId is not null)
                        personal.Cargo = await new dCargo(GetConnectionString()).GetPorId(personal.CargoId.Value);

                    if (personal.EntidadBancariaId is not null)
                        personal.EntidadBancaria = await new dEntidadBancaria(GetConnectionString()).GetPorId(personal.EntidadBancariaId.Value);

                    if (!string.IsNullOrEmpty(personal.TipoCuentaBancariaId))
                        personal.TipoCuentaBancaria = dTipoCuentaBancaria.GetPorId(personal.TipoCuentaBancariaId);

                    personal.Moneda = dMoneda.GetPorId(personal.MonedaId);
                }

                return personal;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vPersonal>> Listar(string nombreCompleto, oPaginacion paginacion)
        {
            try
            {
                dPersonal dPersonal = new(GetConnectionString());
                return await dPersonal.Listar(nombreCompleto ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dPersonal(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad) => await new dPersonal(GetConnectionString()).DatosRepetidos(id, numeroDocumentoIdentidad);

        public async Task<object> FormularioTablas()
        {
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();
            var sexos = dSexo.ListarTodos();
            var estadosCivil = dEstadoCivil.ListarTodos();
            var cargos = await new dCargo(GetConnectionString()).ListarTodos();
            var entidadesBancaria = await new dEntidadBancaria(GetConnectionString()).ListarTodos();
            var tiposCuentaBancaria = dTipoCuentaBancaria.ListarTodos();
            var monedas = dMoneda.ListarTodos();

            return new
            {
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos),
                sexos,
                estadosCivil,
                cargos,
                entidadesBancaria,
                tiposCuentaBancaria,
                monedas
            };
        }
    }
}
