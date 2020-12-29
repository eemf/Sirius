using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class ActividadTipoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public ActividadTipoBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<ActividadTipo> ObtenerListado()
            {
                List<ActividadTipo> Tipos = new List<ActividadTipo>();

                try
                {
                    Tipos = db.Set<ActividadTipo>().AsNoTracking().Where(x => x.Activo).ToList();
                }
                catch (Exception)
                { }

                return Tipos;
            }

        #endregion
    }
}
