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
    public class ActividadBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public ActividadBL()
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
                    Actividad ActividadActual = db.Set<Actividad>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (ActividadActual != null)
                    {
                        Id = ActividadActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }                        

            private string Agregar(Actividad entidad)
            {
                string Mensaje = "OK";
                decimal TotalActividadActual = 0;

                try
                {
                    //Se asigna el punteo de la actividad actual
                    TotalActividadActual = entidad.NotaMaxima;

                    //Se verifica la cantidad de puntos ingresados por unidad
                    List<Actividad> Actividades = db.Set<Actividad>().AsNoTracking().Where(x => x.ColegioId == entidad.ColegioId && x.CursoId == entidad.CursoId && x.GradoId == entidad.GradoId && x.SeccionId == entidad.SeccionId && x.CicloId == entidad.CicloId && x.UnidadId == entidad.UnidadId).ToList();
                    if (Actividades != null && Actividades.Count() > 0)
                    {
                        TotalActividadActual += Actividades.Sum(x => x.NotaMaxima);                    
                    }

                    if (TotalActividadActual > 100)
                    {
                        return "Se le informa que la actividad que desea registrar, excede de la nota total";
                    }

                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngActividadId = new Herramienta().Formato_Correlativo(Id);

                        if (lngActividadId > 0)
                        {
                            entidad.ActividadId = lngActividadId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;
                                                        
                            db.Set<Actividad>().Add(entidad);                          
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

            public string Guardar(Actividad entidad)
            {
                string Mensaje = "OK";

                //Se obtiene el ciclo actual del colegio
                Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == entidad.ColegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                if (CicloActual != null)
                {
                    entidad.CicloId = CicloActual.CicloId;
                }
                else
                {
                    return "Se le informa que el colegio no tiene activado el ciclo escolar";
                }

                if (entidad.ActividadId == 0)
                {
                    Mensaje = Agregar(entidad);
                }               

                return Mensaje;
            }

            public string GuardarNota(ActividadNotaModel entidad, long usuarioId)
            {
                string Mensaje = "OK";

                try
                {
                    ActividadNota ActividadNotaActual = db.Set<ActividadNota>().Where(x => x.ActividadId == entidad.ActividadId && x.AlumnoId == entidad.AlumnoId).FirstOrDefault();
                    if (ActividadNotaActual != null)
                    {
                        ActividadNotaActual.Nota = entidad.Nota;
                        ActividadNotaActual.Observacion = entidad.Comentario;
                        ActividadNotaActual.ResponsableId = usuarioId;
                        ActividadNotaActual.Fecha = DateTime.Today;
                    }
                    else
                    {
                        db.Set<ActividadNota>().Add(new ActividadNota() { ActividadId = entidad.ActividadId, AlumnoId = entidad.AlumnoId, Nota = entidad.Nota, Observacion = entidad.Comentario, ResponsableId = usuarioId, Fecha = DateTime.Today });
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            public Actividad ObtenerxId(long id)
            {
                Actividad ActividadActual = new Actividad();

                try
                {
                    ActividadActual = db.Set<Actividad>().Include("Curso").Include("Grado").Include("Seccion").Include("Ciclo").Include("Unidad").Include("Tipo").AsNoTracking().Where(x => x.ActividadId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return ActividadActual;
            }

            public string Eliminar(Actividad entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Actividad ActividadActual = db.Set<Actividad>().Where(x => x.ActividadId == entidad.ActividadId).FirstOrDefault();
                    if (ActividadActual != null)
                    {
                        db.Set<Actividad>().Remove(ActividadActual);
                        db.SaveChanges();
                    }
                    else
                    {
                        return "La actividad que desea eliminar no se encuentra registrada en el sistema";
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

        #endregion
    }
}
