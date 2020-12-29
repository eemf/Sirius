using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class UnidadBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public UnidadBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<Unidad> ObtenerListado()
            {
                List<Unidad> Unidades = new List<Unidad>();

                try
                {
                    Unidades = db.Set<Unidad>().AsNoTracking().Where(x => x.Activo).ToList();
                }
                catch (Exception)
                { }

                return Unidades;
            }

        #endregion
    }
}
