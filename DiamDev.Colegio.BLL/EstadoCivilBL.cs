using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class EstadoCivilBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public EstadoCivilBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<EstadoCivil> ObtenerListado()
            {
                List<EstadoCivil> Estados = new List<EstadoCivil>();

                try
                {
                    Estados = db.Set<EstadoCivil>().AsNoTracking().Where(x => x.Activo).ToList();
                }
                catch (Exception)
                { }

                return Estados;
            }

        #endregion
    }
}
