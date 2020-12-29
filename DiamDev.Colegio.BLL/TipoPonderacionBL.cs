using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class TipoPonderacionBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public TipoPonderacionBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<TipoPonderacion> ObtenerListado()
            {
                List<TipoPonderacion> Tipos = new List<TipoPonderacion>();

                try
                {
                    Tipos = db.Set<TipoPonderacion>().AsNoTracking().ToList();
                }
                catch (Exception)
                { }

                return Tipos;
            }

        #endregion
    }
}
