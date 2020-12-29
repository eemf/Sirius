using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class PermisoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public PermisoBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<Permiso> ObtenerListado()
            {
                List<Permiso> Permisos = new List<Permiso>();

                try
                {
                    Permisos = db.Set<Permiso>().AsNoTracking().ToList();
                }
                catch (Exception)
                { }

                return Permisos;
            }

        #endregion
    }
}
