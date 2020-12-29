using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using Sistema.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class UsuarioBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public UsuarioBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados

            private string Key(string key)
            {
                return Criptografia.Base64StringAHexString(Criptografia.EncriptarSha512(key));
            }

            private string Concat_Usuario(string Usuario, string Password)
            {
                return string.Concat(Usuario, Password, Usuario);
            }

            private bool ExisteLogin(string Login)
            {
                return db.Set<Usuario>().AsNoTracking().Where(x => x.Login.Equals(Login)).Count() > 0;
            }

            private int Correlativo()
            {
                int Id = 1;

                try
                {
                    Usuario UsuarioActual = db.Set<Usuario>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (UsuarioActual != null)
                    {
                        Id = UsuarioActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Usuario entidad)
            {
                string Mensaje = "OK";

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        string concat = Concat_Usuario(entidad.Login, entidad.Password);
                        long lngUsuarioId = new Herramienta().Formato_Correlativo(Id);

                        if (lngUsuarioId > 0)
                        {
                            entidad.UsuarioId = lngUsuarioId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;                            
                            entidad.Password = Key(concat);
                                                    
                            entidad.LoginOriginal = entidad.Login;

                            db.Set<Usuario>().Add(entidad);
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

            private string Actualizar(Usuario entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Usuario UsuarioActual = ObtenerxId(entidad.UsuarioId, false);

                    if (UsuarioActual.UsuarioId > 0)
                    {
                        string concat = Concat_Usuario(entidad.Login, entidad.Password);

                        UsuarioActual.ColegioId = entidad.ColegioId;
                        UsuarioActual.RolId = entidad.RolId;
                        UsuarioActual.Nombre = entidad.Nombre;
                        UsuarioActual.Password = Key(concat);                        
                        UsuarioActual.ReiniciarPassword = entidad.ReiniciarPassword;
                        UsuarioActual.Activo = entidad.Activo;                      

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            private void ActualizarUltimaActividad(Usuario entidad)
            {
                try
                {
                    Usuario UsuarioActual = ObtenerxId(entidad.UsuarioId, false);

                    if (UsuarioActual.UsuarioId > 0)
                    {
                        UsuarioActual.FechaUltimaActividad = entidad.FechaUltimaActividad;
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                { }
            }

        #endregion

        #region Metodos Publicos

            public string ValidarUsuario(string usuario, string password)
            {
                string Mensaje = "OK";

                var Usuario = ObtenerxLogin(usuario);

                if (Usuario == null)
                {
                    return "El usuario que ingreso no se encuentra registrado";
                }

                if (Usuario.UsuarioId == 0)
                {
                    return "El usuario que ingreso no se encuentra registrado";
                }

                if (!Usuario.Activo)
                {
                    return "El usuario que ingreso no se encuentra activo";
                }

                bool PasswordValido = false;

                if (Usuario.Password == password)
                {
                    PasswordValido = true;
                }

                if (!PasswordValido)
                {
                    return "El usuario o password están incorrectos";
                }
                else
                {
                    Usuario.FechaUltimaActividad = DateTime.Now;
                    ActualizarUltimaActividad(Usuario);
                }

                return Mensaje;
            }

            public string Guardar(Usuario entidad)
            {
                string Mensaje = "OK";

                if (entidad.UsuarioId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    if (!ExisteLogin(entidad.Login))
                    {
                        Mensaje = Agregar(entidad);
                    }
                    else
                    {
                        Mensaje = "El usuario que ingreso ya existe en el sistema";
                    }
                }

                return Mensaje;
            }

            public string DesactivarUsuario(long usuarioId)
            {
                string Mensaje = "OK";

                try
                {
                    Usuario UsuarioActual = ObtenerxId(usuarioId, false);

                    if (UsuarioActual == null)
                    {
                        return "El usuario que quiere desactivar no está registrado en el sistema";
                    }

                    if (UsuarioActual.UsuarioId == 0)
                    {
                        return "El usuario que quiere desactivar no está registrado en el sistema";
                    }

                    UsuarioActual.Activo = false;
                    db.SaveChanges();
                }
                catch (Exception)
                { }

                return Mensaje;
            }

            public string ActualizarPassword(Usuario entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Usuario UsuarioActual = ObtenerxId(entidad.UsuarioId, false);

                    if (UsuarioActual == null)
                    {
                        return "El usuario que quiere actualizar el password no está registrado en el sistema";
                    }

                    if (UsuarioActual.UsuarioId == 0)
                    {
                        return "El usuario que quiere actualizar el password no está registrado en el sistema";
                    }

                    string concat = Concat_Usuario(entidad.Login, entidad.Password);
                    string PasswordEncriptado = Key(concat);

                    if (UsuarioActual.Password != PasswordEncriptado)
                    {
                        return "La contraseña actual no corresponde a la que se encuentra registrada en el sistema";
                    }

                    if (entidad.NuevoPassword != entidad.ConfirmarPassword)
                    {
                        return "La nueva contraseña y la confirmación no son iguales";
                    }

                    string concatNuevoPassword = Concat_Usuario(entidad.Login, entidad.NuevoPassword);

                    UsuarioActual.Password = Key(concatNuevoPassword);
                    UsuarioActual.ReiniciarPassword = false;

                    db.SaveChanges();
                }
                catch (Exception)
                { }

                return Mensaje;
            }

            public Usuario ObtenerxLogin(string usuario, bool cargaLogo = false)
            {
                Usuario UsuarioActual = new Usuario();

                string PathLogo = ConfigurationManager.AppSettings["Path_LogoApp"].ToString();
                string UrlLogo = ConfigurationManager.AppSettings["Url_LogoApp"].ToString();

                try
                {
                    UsuarioActual = db.Set<Usuario>().Include("Colegio").Include("Rol").AsNoTracking().Where(x => x.Login.Equals(usuario)).FirstOrDefault();
                    if (cargaLogo)
                    {
                        if (UsuarioActual != null)
                        {
                            if (UsuarioActual.Colegio != null)
                            {
                                string Path_Colegio_Logo = string.Format(@"{0}\{1}", PathLogo, UsuarioActual.Colegio.ColegioId);
                                if ((Directory.Exists(Path_Colegio_Logo)))
                                {
                                    DirectoryInfo Directorio = new DirectoryInfo(Path_Colegio_Logo);
                                    foreach (FileInfo Logo in Directorio.GetFiles())
                                    {
                                        string LogoActual = string.Format(@"{0}\{1}", Path_Colegio_Logo, Logo.Name);

                                        if (File.Exists(LogoActual))
                                        {
                                            UsuarioActual.Colegio.Logo = string.Format("{0}/{1}/logo.png", UrlLogo, UsuarioActual.Colegio.ColegioId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                { }

                return UsuarioActual;
            }

            public Usuario ObtenerxId(long id, bool todos)
            {
                Usuario UsuarioActual = new Usuario();

                try
                {
                    if (todos)
                    {
                        UsuarioActual = db.Set<Usuario>().Include("Colegio").Include("Rol").Where(x => x.UsuarioId == id).FirstOrDefault();
                    }
                    else
                    {
                        UsuarioActual = db.Set<Usuario>().Where(x => x.UsuarioId == id).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }

                return UsuarioActual;
            }

            public Usuario ObtenerUsuarioConRol(string usuario)
            {
                Usuario UsuarioActual = new Usuario();

                try
                {
                    UsuarioActual = db.Set<Usuario>().Include("Colegio").Include("Rol").AsNoTracking().Where(x => x.Login.Equals(usuario)).FirstOrDefault();

                    if (UsuarioActual != null)
                    {
                        UsuarioActual.RolesPermiso = new RolBL().ObtenerPermisoxUsuario(UsuarioActual.Login);
                    }
                }
                catch (Exception)
                { }

                return UsuarioActual;
            }

            public List<Usuario> ObtenerListado(long colegioId)
            {
                List<Usuario> Usuarios = new List<Usuario>();

                try
                {
                    if (colegioId == 0)
                    {
                        Usuarios = db.Set<Usuario>().Include("Colegio").Include("Rol").AsNoTracking().Where(x => x.Administrador).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.UsuarioId).Take(200).ToList();
                    }
                    else
                    {
                        Usuarios = db.Set<Usuario>().Include("Colegio").Include("Rol").AsNoTracking().Where(x => x.ColegioId == colegioId && !x.Administrador).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.UsuarioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Usuarios;
            }

            public List<Usuario> Buscar(string Buscar, long colegioId)
            {
                List<Usuario> Usuarios = new List<Usuario>();

                try
                {
                    if (colegioId == 0)
                    {
                        Usuarios = db.Set<Usuario>().Include("Colegio").Include("Rol").AsNoTracking().Where(x => x.Nombre.ToLower().Contains(Buscar.ToLower()) && x.Administrador).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.UsuarioId).Take(200).ToList();
                    }
                    else
                    {
                        Usuarios = db.Set<Usuario>().Include("Colegio").Include("Rol").AsNoTracking().Where(x => x.Nombre.ToLower().Contains(Buscar.ToLower()) && x.ColegioId == colegioId && !x.Administrador).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.UsuarioId).Take(200).ToList();
                    }                
                }
                catch (Exception)
                { }

                return Usuarios;
            }      

        #endregion
    }
}
