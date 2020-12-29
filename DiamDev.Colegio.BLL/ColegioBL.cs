using DiamDev.Colegio.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class ColegioBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public ColegioBL()
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
                    Entities.Colegio ColegioActual = db.Set<Entities.Colegio>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (ColegioActual != null)
                    {
                        Id = ColegioActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Entities.Colegio entidad)
            {
                string Mensaje = "OK";

                string PathLogo = ConfigurationManager.AppSettings["Path_LogoApp"].ToString();               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngColegioId = new Herramienta().Formato_Correlativo(Id);

                        if (lngColegioId > 0)
                        {
                            entidad.ColegioId = lngColegioId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<Entities.Colegio>().Add(entidad);
                            db.SaveChanges();

                            if (Mensaje.Equals("OK"))
                            {
                                //Se crea carpeta por colegio para almacenar el logo
                                string Path_Colegio_Logo = string.Format(@"{0}\{1}", PathLogo, entidad.ColegioId);

                                if (!(Directory.Exists(Path_Colegio_Logo)))
                                {
                                    Directory.CreateDirectory(Path_Colegio_Logo);
                                }

                                if (entidad.Fotografia != null)
                                {
                                    ConvetirbyteImage(entidad.Fotografia.Content).Save(string.Format(@"{0}\logo.png", Path_Colegio_Logo));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            private string Actualizar(Entities.Colegio entidad)
            {
                string Mensaje = "OK";

                string PathLogo = ConfigurationManager.AppSettings["Path_LogoApp"].ToString();

                try
                {
                    Entities.Colegio ColegioActual = ObtenerxId(entidad.ColegioId);

                    if (ColegioActual.ColegioId > 0)
                    {                      
                        ColegioActual.Nombre = entidad.Nombre;
                        ColegioActual.Direccion = entidad.Direccion;
                        ColegioActual.Telefono = entidad.Telefono;
                        ColegioActual.Contacto = entidad.Contacto;
                        ColegioActual.TelefonoContacto = entidad.TelefonoContacto;
                        ColegioActual.Alias = entidad.Alias;
                        ColegioActual.Activo = entidad.Activo;

                        db.SaveChanges();

                        if (Mensaje.Equals("OK"))
                        {
                            //Se crea carpeta por colegio para almacenar el logo
                            string Path_Colegio_Logo = string.Format(@"{0}\{1}", PathLogo, entidad.ColegioId);

                            if (!(Directory.Exists(Path_Colegio_Logo)))
                            {
                                Directory.CreateDirectory(Path_Colegio_Logo);
                            }

                            if (entidad.Fotografia != null)
                            {
                                ConvetirbyteImage(entidad.Fotografia.Content).Save(string.Format(@"{0}\logo.png", Path_Colegio_Logo));
                            }
                        }
                    }
                    else
                    {
                        Mensaje = "El colegio seleccionado no se encuentra con ID valido";
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            private Image ConvetirbyteImage(byte[] byteArrayIn)
            {
                return Image.FromStream(new MemoryStream(byteArrayIn));
            }

        #endregion

        #region Metodos Publicos

            public string Guardar(Entities.Colegio entidad)
            {
                string Mensaje = "OK";

                if (entidad.ColegioId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Entities.Colegio ObtenerxId(long id)
            {
                Entities.Colegio ColegioActual = new Entities.Colegio();

                try
                {
                    ColegioActual = db.Set<Entities.Colegio>().Where(x => x.ColegioId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return ColegioActual;
            }

            public List<Entities.Colegio> ObtenerListado(bool todo)
            {
                List<Entities.Colegio> Colegios = new List<Entities.Colegio>();

                try
                {
                    if (todo)
                    {
                        Colegios = db.Set<Entities.Colegio>().AsNoTracking().OrderByDescending(x => x.Fecha).ThenByDescending(x => x.ColegioId).Take(200).ToList();
                    }
                    else
                    {
                        Colegios = db.Set<Entities.Colegio>().AsNoTracking().Where(x => x.Activo).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Colegios;
            }

            public List<Entities.Colegio> Buscar(string search)
            {
                List<Entities.Colegio> Colegios = new List<Entities.Colegio>();

                try
                {
                    Colegios = db.Set<Entities.Colegio>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower())).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.ColegioId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Colegios;
            }       

        #endregion
    }
}
