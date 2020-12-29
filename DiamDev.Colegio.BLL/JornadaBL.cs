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
    public class JornadaBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public JornadaBL()
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
                    Jornada JornadaActual = db.Set<Jornada>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (JornadaActual != null)
                    {
                        Id = JornadaActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Jornada entidad)
            {
                string Mensaje = "OK";               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngJornadaId = new Herramienta().Formato_Correlativo(Id);

                        if (lngJornadaId > 0)
                        {
                            entidad.JornadaId = lngJornadaId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<Jornada>().Add(entidad);
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

            private string Actualizar(Jornada entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Jornada JornadaActual = ObtenerxId(entidad.JornadaId);

                    if (JornadaActual.JornadaId > 0)
                    {
                        JornadaActual.Nombre = entidad.Nombre;
                        JornadaActual.Activo = entidad.Activo;

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "La jornada escolar seleccionada no se encuentra con ID valido";
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

            public string Guardar(Jornada entidad)
            {
                string Mensaje = "OK";

                if (entidad.JornadaId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Jornada ObtenerxId(long id)
            {
                Jornada JornadaActual = new Jornada();

                try
                {
                    JornadaActual = db.Set<Jornada>().Where(x => x.JornadaId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return JornadaActual;
            }

            public List<Jornada> ObtenerListado(bool todo, long colegioId)
            {
                List<Jornada> Jornadas = new List<Jornada>();

                try
                {
                    if (todo)
                    {
                        Jornadas = db.Set<Jornada>().AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.JornadaId).Take(200).ToList();
                    }
                    else
                    {
                        Jornadas = db.Set<Jornada>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Jornadas;
            }

            public List<Jornada> Buscar(string search, long colegioId)
            {
                List<Jornada> Jornadas = new List<Jornada>();

                try
                {
                    Jornadas = db.Set<Jornada>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.JornadaId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Jornadas;
            }       

        #endregion
    }
}
