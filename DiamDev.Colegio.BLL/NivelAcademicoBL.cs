using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class NivelAcademicoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public NivelAcademicoBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados

            private int Correlativo()
            {
                int Id = 1;

                try
                {
                    NivelAcademico NivelAcademicoActual = db.Set<NivelAcademico>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (NivelAcademicoActual != null)
                    {
                        Id = NivelAcademicoActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(NivelAcademico entidad)
            {
                string Mensaje = "OK";               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngNivelAcademicoId = new Herramienta().Formato_Correlativo(Id);

                        if (lngNivelAcademicoId > 0)
                        {
                            entidad.NivelId = lngNivelAcademicoId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<NivelAcademico>().Add(entidad);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            private string Actualizar(NivelAcademico entidad)
            {
                string Mensaje = "OK";

                try
                {
                    NivelAcademico NivelAcademicoActual = ObtenerxId(entidad.NivelId);

                    if (NivelAcademicoActual.NivelId > 0)
                    {
                        NivelAcademicoActual.Nombre = entidad.Nombre;
                        NivelAcademicoActual.Activo = entidad.Activo;

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El nivel academico seleccionado no se encuentra con ID valido";
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }          

        #endregion

        #region Metodos Publicos

            public string Guardar(NivelAcademico entidad)
            {
                string Mensaje = "OK";

                if (entidad.NivelId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public NivelAcademico ObtenerxId(long id)
            {
                NivelAcademico NivelAcademicoActual = new NivelAcademico();

                try
                {
                    NivelAcademicoActual = db.Set<NivelAcademico>().Where(x => x.NivelId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return NivelAcademicoActual;
            }

            public List<NivelAcademico> ObtenerListado(bool todo, long colegioId)
            {
                List<NivelAcademico> Niveles = new List<NivelAcademico>();

                try
                {
                    if (todo)
                    {
                        Niveles = db.Set<NivelAcademico>().AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.NivelId).Take(200).ToList();
                    }
                    else
                    {
                        Niveles = db.Set<NivelAcademico>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Niveles;
            }

            public List<NivelAcademico> Buscar(string search, long colegioId)
            {
                List<NivelAcademico> Niveles = new List<NivelAcademico>();

                try
                {
                    Niveles = db.Set<NivelAcademico>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.NivelId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Niveles;
            }       

        #endregion
    }
}
