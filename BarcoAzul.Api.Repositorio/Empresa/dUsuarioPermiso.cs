using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dUsuarioPermiso : dComun
    {
        public dUsuarioPermiso(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(string usuarioId, string tipoUsuarioId, IEnumerable<oUsuarioPermiso> usuarioPermisos)
        {
            using (var db = GetConnection())
            {
                foreach (var usuarioPermiso in usuarioPermisos)
                {
                    string query = @"   INSERT INTO Acceso_Usuario_Web (Usu_Codigo, Menu_Codigo, Acc_Codigo, Acc_Activo) VALUES 
                                        (@UsuarioId, @MenuId, 'Registrar', @Registrar),
                                        (@UsuarioId, @MenuId, 'Modificar', @Modificar),
                                        (@UsuarioId, @MenuId, 'Eliminar', @Eliminar),
                                        (@UsuarioId, @MenuId, 'Consultar', @Consultar),
                                        (@UsuarioId, @MenuId, 'Anular', @Anular)";

                    await db.ExecuteAsync(query, usuarioPermiso);

                    query = "UPDATE Usuario SET Usu_TUsuario = @tipoUsuarioId WHERE Usu_Codigo = @usuarioId";

                    await db.ExecuteAsync(query, new
                    {
                        tipoUsuarioId = new DbString { Value = tipoUsuarioId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 }
                    });
                }
            }
        }

        public async Task EliminarDeCliente(string usuarioId)
        {
            string query = "DELETE Acceso_Usuario_Web WHERE Usu_Codigo = @usuarioId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oUsuarioPermiso>> ListarPorUsuario(string usuarioId)
        {
            string query = @"   SELECT 
	                                RTRIM(Menu_Codigo) AS MenuId,
	                                RTRIM(Acc_Codigo) AS AccionId,
	                                Acc_Activo AS IsPermitido
                                FROM
                                    Acceso_Usuario_Web 
                                WHERE 
	                                Usu_Codigo = @usuarioId";

            List<dynamic> usuarioPermisosDb;

            using (var db = GetConnection())
            {
                usuarioPermisosDb = (await db.QueryAsync(query, new { usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 } })).ToList();
            }

            //Convertimos lo traido de la BD al tipo oUsuarioPermiso para un mejor manejo en el sistema
            var usuarioPermisos = new List<oUsuarioPermiso>();

            while (usuarioPermisosDb.Count > 0)
            {
                var menuId = usuarioPermisosDb.First().MenuId;
                var usuarioPermisoMenu = usuarioPermisosDb.Where(x => x.MenuId == menuId).ToList();

                usuarioPermisos.Add(new oUsuarioPermiso
                {
                    UsuarioId = usuarioId,
                    MenuId = menuId,
                    Registrar = Convert.ToBoolean(usuarioPermisoMenu.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Registrar.ToString())?.IsPermitido),
                    Modificar = Convert.ToBoolean(usuarioPermisoMenu.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Modificar.ToString())?.IsPermitido),
                    Eliminar = Convert.ToBoolean(usuarioPermisoMenu.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Eliminar.ToString())?.IsPermitido),
                    Consultar = Convert.ToBoolean(usuarioPermisoMenu.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Consultar.ToString())?.IsPermitido),
                    Anular = Convert.ToBoolean(usuarioPermisoMenu.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Anular.ToString())?.IsPermitido)
                });

                usuarioPermisosDb.RemoveAll(x => x.MenuId == menuId);
            }
            return usuarioPermisos;
        }

        public async Task<oUsuarioPermiso> GetPorUsuarioYMenu(string usuarioId, string menuId)
        {
            string query = @"   SELECT 
	                                RTRIM(Menu_Codigo) AS MenuId,
	                                RTRIM(Acc_Codigo) AS AccionId,
	                                Acc_Activo AS IsPermitido
                                FROM
                                    Acceso_Usuario_Web 
                                WHERE 
	                                Usu_Codigo = @usuarioId
                                    AND Menu_Codigo = @menuId";

            IEnumerable<dynamic> usuarioPermisos;

            using (var db = GetConnection())
            {
                usuarioPermisos = await db.QueryAsync(query, new
                {
                    usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 },
                    menuId = new DbString { Value = menuId, IsAnsi = true, IsFixedLength = true, Length = 40 }
                });
            }

            return new oUsuarioPermiso
            {
                UsuarioId = usuarioId,
                MenuId = menuId,
                Registrar = Convert.ToBoolean(usuarioPermisos.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Registrar.ToString())?.IsPermitido),
                Modificar = Convert.ToBoolean(usuarioPermisos.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Modificar.ToString())?.IsPermitido),
                Eliminar = Convert.ToBoolean(usuarioPermisos.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Eliminar.ToString())?.IsPermitido),
                Consultar = Convert.ToBoolean(usuarioPermisos.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Consultar.ToString())?.IsPermitido),
                Anular = Convert.ToBoolean(usuarioPermisos.FirstOrDefault(x => x.AccionId == UsuarioPermiso.Anular.ToString())?.IsPermitido)
            };
        }

        public async Task<bool> IsPermitido(string usuarioId, string menuId, UsuarioPermiso permiso)
        {
            string query = "SELECT Acc_Activo FROM Acceso_Usuario_Web WHERE Usu_Codigo = @usuarioId AND Menu_Codigo = @menuId AND Acc_Codigo = @permiso";

            using (var db = GetConnection())
            {
                var tienePermiso = await db.QueryFirstOrDefaultAsync<bool?>(query, new
                {
                    usuarioId = new DbString { Value = usuarioId, IsAnsi = true, IsFixedLength = false, Length = 3 },
                    menuId = new DbString { Value = menuId, IsAnsi = true, IsFixedLength = true, Length = 40 },
                    permiso = new DbString { Value = permiso.ToString(), IsAnsi = true, IsFixedLength = true, Length = 20 }
                });

                return tienePermiso ?? false;
            }
        }
        #endregion
    }
}
