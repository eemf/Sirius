using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class MenuBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public MenuBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados

            private bool MenuTieneHijos(int MenuId)
            {
                return db.Set<Menu>().AsNoTracking().Where(x => x.MenuPadreId == MenuId).Count() > 0;
            }

            private List<Menu> ObtenerSubMenu(int menuPadreId, List<string> Permisos)
            {
                List<Menu> SubMenus = new List<Menu>();

                try
                {
                    var Menus = db.Set<Menu>().AsNoTracking().Where(x => x.MenuPadreId == menuPadreId && x.IsActive == true && Permisos.Contains(x.PermisoId)).OrderBy(x => x.Orden).ToList();

                    if (Menus != null && Menus.Count() > 0)
                    {
                        foreach (var SubMenu in Menus)
                        {
                            if (MenuTieneHijos(SubMenu.MenuId))
                            {
                                SubMenu.Items = new List<Menu>();
                                SubMenu.Items = ObtenerSubMenu(SubMenu.MenuId, Permisos);
                            }

                            SubMenus.Add(SubMenu);
                        }
                    }
                }
                catch (Exception)
                { }

                return SubMenus;
            }

        #endregion

        #region Metodos Publicos

            public List<Menu> ObtenerMenuxUsuario(string usuario)
            {
                List<Menu> Menus = new List<Menu>();

                try
                {
                    List<RolPermiso> RolPermisos = new RolBL().ObtenerPermisoxUsuario(usuario);

                    if (RolPermisos != null && RolPermisos.Count() > 0)
                    {

                        List<string> Permisos = RolPermisos.Select(x => x.PermisoId).ToList();
                        List<Menu> MenusPadre = db.Set<Menu>().AsNoTracking().Where(x => x.MenuPadreId == null && x.IsActive == true && Permisos.Contains(x.PermisoId)).OrderBy(x => x.Orden).ToList();

                        if (MenusPadre != null && MenusPadre.Count() > 0)
                        {
                            foreach (var Menu in MenusPadre)
                            {
                                if (MenuTieneHijos(Menu.MenuId))
                                {
                                    Menu.Items = new List<Menu>();
                                    Menu.Items = ObtenerSubMenu(Menu.MenuId, Permisos);
                                }

                                Menus.Add(Menu);
                            }
                        }
                    }
                }
                catch (Exception)
                { }

                return Menus;
            }

        #endregion
    }
}
