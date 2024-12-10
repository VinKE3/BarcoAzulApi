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
    public class bArticulo : bComun, ILogicaService
    {
        public bArticulo(oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Artículo", datosUsuario) { }

        public async Task<bool> Registrar(ArticuloDTO model)
        {
            try
            {
                var articulo = Mapping.Mapper.Map<oArticulo>(model);
                articulo.ProcesarDatos();
                articulo.UsuarioId = _datosUsuario.Id;

                dArticulo dArticulo = new(GetConnectionString());
                articulo.ArticuloId = model.ArticuloId = await dArticulo.GetNuevoId(articulo.LineaId, articulo.SubLineaId);
                articulo.CodigoBarras = model.CodigoBarras = string.IsNullOrWhiteSpace(articulo.CodigoBarras) ? articulo.Id : articulo.CodigoBarras;
                await dArticulo.Registrar(articulo);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(ArticuloDTO model)
        {
            try
            {
                var articulo = Mapping.Mapper.Map<oArticulo>(model);
                articulo.ProcesarDatos();
                articulo.UsuarioId = _datosUsuario.Id;
                articulo.CodigoBarras = model.CodigoBarras = string.IsNullOrWhiteSpace(articulo.CodigoBarras) ? articulo.Id : articulo.CodigoBarras;

                dArticulo dArticulo = new(GetConnectionString());
                await dArticulo.Modificar(articulo);

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
                dArticulo dArticulo = new(GetConnectionString());
                await dArticulo.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oArticulo> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dArticulo dArticulo = new(GetConnectionString());
                var articulo = await dArticulo.GetPorId(id);

                if (incluirReferencias)
                {
                    articulo.Linea = await new dLinea(GetConnectionString()).GetPorId(articulo.LineaId);
                    articulo.SubLinea = await new dSubLinea(GetConnectionString()).GetPorId(articulo.LineaId + articulo.SubLineaId);
                    articulo.TipoExistencia = await new dTipoExistencia(GetConnectionString()).GetPorId(articulo.TipoExistenciaId);
                    articulo.UnidadMedida = await new dUnidadMedida(GetConnectionString()).GetPorId(articulo.UnidadMedidaId);
                    articulo.Marca = await new dMarca(GetConnectionString()).GetPorId(articulo.MarcaId);
                    articulo.Moneda = dMoneda.GetPorId(articulo.MonedaId);
                }

                return articulo;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vArticulo>> Listar(string codigoBarras, string descripcion, bool? isActivo, oPaginacion paginacion)
        {
            try
            {
                dArticulo dArticulo = new(GetConnectionString());
                return await dArticulo.Listar(codigoBarras ?? string.Empty, descripcion ?? string.Empty, isActivo, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dArticulo(GetConnectionString()).Existe(id);

        public async Task<(bool Existe, string ValorRepetido)> DatosRepetidos(string id, string codigoBarras, string descripcion) => await new dArticulo(GetConnectionString()).DatosRepetidos(id, codigoBarras, descripcion);

        public async Task<object> FormularioTablas()
        {
            var tiposExistencia = await new dTipoExistencia(GetConnectionString()).ListarTodos();
            var lineas = await new dLinea(GetConnectionString()).ListarTodos();
            var subLineas = await new dSubLinea(GetConnectionString()).ListarTodos();
            var marcas = await new dMarca(GetConnectionString()).ListarTodos();
            var unidadesMedida = await new dUnidadMedida(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();

            return new
            {
                tiposExistencia,
                lineas,
                subLineas,
                marcas,
                unidadesMedida,
                monedas
            };
        }
    }
}
