using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class CursoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public CursoBL()
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
                    Curso CursoActual = db.Set<Curso>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (CursoActual != null)
                    {
                        Id = CursoActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Curso entidad)
            {
                string Mensaje = "OK";               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngCursoId = new Herramienta().Formato_Correlativo(Id);

                        if (lngCursoId > 0)
                        {
                            entidad.CursoId = lngCursoId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            if (entidad.Grados != null && entidad.Grados.Count() > 0)
                            {
                                entidad.Grados.ForEach(x => 
                                {
                                    x.CursoId = entidad.CursoId;
                                });
                            }

                            db.Set<Curso>().Add(entidad);
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

            private string Actualizar(Curso entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Curso CursoActual = ObtenerxId(entidad.CursoId);

                    if (CursoActual.CursoId > 0)
                    {
                        CursoActual.TipoId = entidad.TipoId;
                        CursoActual.Nombre = entidad.Nombre;
                        CursoActual.Abreviatura = entidad.Abreviatura;
                        CursoActual.Ministerial = entidad.Ministerial;
                        CursoActual.Activo = entidad.Activo;

                        if (entidad.Grados != null && entidad.Grados.Count() > 0)
                        {
                            List<CursoGrado> Grados = db.Set<CursoGrado>().Where(x => x.CursoId == CursoActual.CursoId).ToList();
                            db.Set<CursoGrado>().RemoveRange(Grados);

                            entidad.Grados.ForEach(x =>
                            {
                                x.CursoId = entidad.CursoId;
                                db.Set<CursoGrado>().Add(x);
                            });
                        }

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El curso seleccionado no se encuentra con ID valido";
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

            public string Guardar(Curso entidad)
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

                if (entidad.CursoId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Curso ObtenerxId(long id, bool todo = false)
            {
                Curso CursoActual = new Curso();

                try
                {
                    if (todo)
                    {
                        CursoActual = db.Set<Curso>().Include("Ciclo").Include("Grados").Include("Grados.Grado").Include("Grados.Grado.Jornada").AsNoTracking().Where(x => x.CursoId == id).FirstOrDefault();
                    }
                    else
                    {
                        CursoActual = db.Set<Curso>().Where(x => x.CursoId == id).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }

                return CursoActual;
            }

            public List<Curso> ObtenerxGradoId(long gradoId, long colegioId) 
            {
                List<Curso> Cursos = new List<Curso>();

                try
                {
                    Cursos = db.Set<CursoGrado>().AsNoTracking().Where(x => x.GradoId == gradoId).Join(db.Set<Curso>().AsNoTracking().Where(x => x.ColegioId == colegioId), CG => CG.CursoId, C => C.CursoId, (CG, C) => new { C }).Select(x => x.C).ToList();
                }
                catch (Exception)
                {}

                return Cursos;
            }

            public List<Curso> ObtenerListado(bool todo, long colegioId)
            {
                List<Curso> Cursos = new List<Curso>();
                long CicloId = 0;

                try
                {
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CicloId = CicloActual.CicloId;
                    }

                    if (todo)
                    {
                        Cursos = db.Set<Curso>().Include("Ciclo").Include("Tipo").Include("Grados").AsNoTracking().Where(x => x.ColegioId == colegioId && x.CicloId == CicloId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CursoId).Take(200).ToList();
                    }
                    else
                    {
                        Cursos = db.Set<Curso>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId && x.CicloId == CicloId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Cursos;
            }

            public List<Curso> Buscar(string search, long colegioId)
            {
                List<Curso> Cursos = new List<Curso>();
                long CicloId = 0;

                try
                {
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CicloId = CicloActual.CicloId;
                    }

                    Cursos = db.Set<Curso>().Include("Ciclo").Include("Tipo").Include("Grados").AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId && x.CicloId == CicloId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CursoId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Cursos;
            }       

        #endregion
    }
}
