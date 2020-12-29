using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using Sistema.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class InscripcionBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public InscripcionBL()
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
                    Inscripcion InscripcionActual = db.Set<Inscripcion>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (InscripcionActual != null)
                    {
                        Id = InscripcionActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }         

            private string Agregar(Inscripcion entidad)
            {
                string Mensaje = "OK";
            
                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngInscripcionId = new Herramienta().Formato_Correlativo(Id);

                        if (lngInscripcionId > 0)
                        {
                            entidad.InscripcionId = lngInscripcionId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            entidad.FechaHoraUltimaModificacion = DateTime.Now;
                          
                            db.Set<Inscripcion>().Add(entidad);
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

        #endregion

        #region Metodos Publicos

            public string Guardar(Inscripcion entidad)
            {
                string Mensaje = "OK";

                if (entidad.InscripcionId == 0)
                {
                    Mensaje = Agregar(entidad);
                }                  

                return Mensaje;
            }

            public string Eliminar(Inscripcion entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Inscripcion InscripcionActual = db.Set<Inscripcion>().Where(x => x.InscripcionId == entidad.InscripcionId).FirstOrDefault();
                    if (InscripcionActual != null)
                    {
                        db.Set<Inscripcion>().Remove(InscripcionActual);
                        db.SaveChanges();
                    }
                    else
                    {
                        return "Se le informa que la inscripción no se encuentra registrada en el sistema";
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            public Inscripcion ObtenerxId(long id, bool todo)
            {
                Inscripcion InscripcionActual = new Inscripcion();

                try
                {
                    if (todo)
                    {
                        InscripcionActual = db.Set<Inscripcion>().Include("Alumno").Include("Ciclo").Include("NivelAcademico").Include("Jornada").Include("Grado").Include("Seccion").AsNoTracking().Where(x => x.InscripcionId == id).FirstOrDefault();
                    }
                    else
                    {
                        InscripcionActual = db.Set<Inscripcion>().Where(x => x.InscripcionId == id).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }

                return InscripcionActual;
            }

            public List<Inscripcion> ObtenerListado(long colegioId)
            {
                List<Inscripcion> Inscripciones = new List<Inscripcion>();

                try
                {
                    Inscripciones = db.Set<Inscripcion>().Include("Alumno").Include("Ciclo").Include("NivelAcademico").Include("Jornada").Include("Grado").Include("Seccion").AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.InscripcionId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Inscripciones;
            }

            public List<Inscripcion> Buscar(string search, long colegioId)
            {
                List<Inscripcion> Inscripciones = new List<Inscripcion>();

                try
                {
                    Inscripciones = db.Set<Inscripcion>().Include("Alumno").Include("Ciclo").Include("NivelAcademico").Include("Jornada").Include("Grado").Include("Seccion").AsNoTracking().Where(x => x.Alumno.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.InscripcionId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Inscripciones;
            }       

        #endregion
    }
}
