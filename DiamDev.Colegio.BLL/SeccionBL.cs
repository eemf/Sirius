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
    public class SeccionBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public SeccionBL()
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
                    Seccion SeccionActual = db.Set<Seccion>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (SeccionActual != null)
                    {
                        Id = SeccionActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Seccion entidad)
            {
                string Mensaje = "OK";               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngSeccionId = new Herramienta().Formato_Correlativo(Id);

                        if (lngSeccionId > 0)
                        {
                            entidad.SeccionId = lngSeccionId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<Seccion>().Add(entidad);
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

            private string Actualizar(Seccion entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Seccion SeccionActual = ObtenerxId(entidad.SeccionId);

                    if (SeccionActual.SeccionId > 0)
                    {
                        SeccionActual.Nombre = entidad.Nombre;
                        SeccionActual.Activo = entidad.Activo;

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "La seccion escolar seleccionada no se encuentra con ID valido";
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

            public string Guardar(Seccion entidad)
            {
                string Mensaje = "OK";

                if (entidad.SeccionId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Seccion ObtenerxId(long id)
            {
                Seccion SeccionActual = new Seccion();

                try
                {
                    SeccionActual = db.Set<Seccion>().Where(x => x.SeccionId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return SeccionActual;
            }

            public List<Seccion> ObtenerListado(bool todo, long colegioId)
            {
                List<Seccion> Secciones = new List<Seccion>();

                try
                {
                    if (todo)
                    {
                        Secciones = db.Set<Seccion>().AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.SeccionId).Take(200).ToList();
                    }
                    else
                    {
                        Secciones = db.Set<Seccion>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Secciones;
            }

            public List<Seccion> Buscar(string search, long colegioId)
            {
                List<Seccion> Secciones = new List<Seccion>();

                try
                {
                    Secciones = db.Set<Seccion>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.SeccionId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Secciones;
            }       

        #endregion
    }
}
