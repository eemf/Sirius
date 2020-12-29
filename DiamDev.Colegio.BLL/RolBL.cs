using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class RolBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public RolBL()
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
                    Rol RolActual = db.Set<Rol>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (RolActual != null)
                    {
                        Id = RolActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Rol entidad)
            {
                string Mensaje = "OK";

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngRolId = new Herramienta().Formato_Correlativo(Id);

                        if (lngRolId > 0)
                        {
                            entidad.RolId = lngRolId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<Rol>().Add(entidad);
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

            private string Actualizar(Rol entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Rol RolActual = ObtenerxId(entidad.RolId);

                    if (RolActual.RolId > 0)
                    {
                        RolActual.Nombre = entidad.Nombre;

                        //Eliminar permiso por rolId
                        var Permisos = db.Set<RolPermiso>().Where(x => x.RolId == RolActual.RolId).ToList();
                        db.Set<RolPermiso>().RemoveRange(Permisos);

                        //Agregar los nuevos permisos
                        RolActual.Permisos = new List<RolPermiso>();
                        foreach (var Permiso in entidad.Permisos)
                        {
                            db.Set<RolPermiso>().Add(new RolPermiso() { RolId = RolActual.RolId, PermisoId = Permiso.PermisoId });
                        }

                        db.SaveChanges();
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

            public string Guardar(Rol entidad)
            {
                string Mensaje = "OK";

                if (entidad.RolId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Rol ObtenerxId(long id)
            {
                Rol RolActual = new Rol();

                try
                {
                    RolActual = db.Set<Rol>().Include("Permisos").Where(x => x.RolId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return RolActual;
            }

            public string ObtenerPermisoxUsuario(long usuario)
            {
                Rol RolActual = new Rol();
                string Descripcion = string.Empty;

                try
                {
                    RolActual = db.Set<Usuario>().AsNoTracking().Where(x => x.UsuarioId == usuario).Join(db.Set<Rol>().AsNoTracking(), R => R.RolId, RP => RP.RolId, (R, RP) => new { Permisos = RP }).Select(x => x.Permisos).FirstOrDefault();
                    if (RolActual != null)
                    {
                        Descripcion = RolActual.Nombre;
                    }
                }
                catch (Exception)
                { }

                return Descripcion;
            }

            public bool AutorizacionPermisoxUsuario(string usuario, string permiso)
            {
                bool Autorizacion = false;

                try
                {
                    Autorizacion = db.Set<Usuario>().AsNoTracking().Where(x => x.Login.Equals(usuario)).Join(db.Set<RolPermiso>().AsNoTracking(), R => R.RolId, RP => RP.RolId, (R, RP) => new { Permisos = RP }).Select(x => x.Permisos).Any(x => x.PermisoId.Equals(permiso));
                }
                catch (Exception)
                { }

                return Autorizacion;
            }

            public List<RolPermiso> ObtenerPermisoxUsuario(string usuario)
            {
                List<RolPermiso> Permisos = new List<RolPermiso>();

                try
                {
                    Permisos = db.Set<Usuario>().AsNoTracking().Where(x => x.Login.Equals(usuario)).Join(db.Set<RolPermiso>().AsNoTracking(), R => R.RolId, RP => RP.RolId, (R, RP) => new { Permisos = RP }).Select(x => x.Permisos).ToList();
                }
                catch (Exception)
                { }

                return Permisos;
            }

            public List<RolPermiso> ObtenerPermisoxRolId(int id)
            {
                List<RolPermiso> Permisos = new List<RolPermiso>();

                try
                {
                    Permisos = db.Set<RolPermiso>().AsNoTracking().Where(x => x.RolId == id).ToList();
                }
                catch (Exception)
                { }

                return Permisos;
            }

            public List<Rol> ObtenerListado()
            {
                List<Rol> Roles = new List<Rol>();

                try
                {
                    Roles = db.Set<Rol>().Include("Permisos").AsNoTracking().Where(x => x.RolId > 20200928001).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.RolId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Roles;
            }

            public List<Rol> ObtenerxResponsable(long usuarioId)
            {
                List<Rol> Roles = new List<Rol>();
                List<long> RolIds = new List<long>() { 20200928003, 20200928004 };

                try
                {
                    if (usuarioId == 20200928001)
                    {
                        Roles = db.Set<Rol>().AsNoTracking().Where(x => x.RolId == 20200928002).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.RolId).Take(200).ToList();
                    }
                    else if (usuarioId > 0)
                    {
                        Roles = db.Set<Rol>().AsNoTracking().Where(x => RolIds.Contains(x.RolId)).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.RolId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Roles;
            }

            public List<Rol> Buscar(string Buscar)
            {
                List<Rol> Roles = new List<Rol>();

                try
                {
                    Roles = db.Set<Rol>().Include("Permisos").AsNoTracking().Where(x => x.Nombre.Contains(Buscar) && x.RolId > 20200928001).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.RolId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Roles;
            }

        #endregion
    }
}
