using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class TipoEncargadoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public TipoEncargadoBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<TipoEncargado> ObtenerListado()
            {
                List<TipoEncargado> Tipos = new List<TipoEncargado>();

                try
                {
                    Tipos = db.Set<TipoEncargado>().AsNoTracking().ToList();
                }
                catch (Exception)
                { }

                return Tipos;
            }

        #endregion
    }
}
