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
    public class CicloBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public CicloBL()
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
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (CicloActual != null)
                    {
                        Id = CicloActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Ciclo entidad)
            {
                string Mensaje = "OK";               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngCicloId = new Herramienta().Formato_Correlativo(Id);

                        if (lngCicloId > 0)
                        {
                            entidad.CicloId = lngCicloId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<Ciclo>().Add(entidad);
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

            private string Actualizar(Ciclo entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Ciclo CicloActual = ObtenerxId(entidad.CicloId);

                    if (CicloActual.CicloId > 0)
                    {
                        CicloActual.Nombre = entidad.Nombre;                        
                        CicloActual.Activo = entidad.Activo;

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El ciclo escolar seleccionado no se encuentra con ID valido";
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

            public string Guardar(Ciclo entidad)
            {
                string Mensaje = "OK";

                if (entidad.CicloId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Ciclo ObtenerxId(long id)
            {
                Ciclo CicloActual = new Ciclo();

                try
                {
                    CicloActual = db.Set<Ciclo>().Where(x => x.CicloId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return CicloActual;
            }

            public List<Ciclo> ObtenerListado(bool todo, long colegioId)
            {
                List<Ciclo> Ciclos = new List<Ciclo>();

                try
                {
                    if (todo)
                    {
                        Ciclos = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).Take(200).ToList();
                    }
                    else
                    {
                        Ciclos = db.Set<Ciclo>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Ciclos;
            }

            public List<Ciclo> Buscar(string search, long colegioId)
            {
                List<Ciclo> Ciclos = new List<Ciclo>();

                try
                {
                    Ciclos = db.Set<Ciclo>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Ciclos;
            }       

        #endregion
    }
}
