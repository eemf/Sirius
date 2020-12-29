using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class GeneroBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public GeneroBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados
        #endregion

        #region Metodos Publicos

            public List<Genero> ObtenerListado()
            {
                List<Genero> Generos = new List<Genero>();

                try
                {
                    Generos = db.Set<Genero>().AsNoTracking().Where(x => x.Activo).ToList();
                }
                catch (Exception)
                { }

                return Generos;
            }

        #endregion
    }
}
