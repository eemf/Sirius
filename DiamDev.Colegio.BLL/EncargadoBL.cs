using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using Sistema.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class EncargadoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public EncargadoBL()
            {
                this.db = new ColegioContext();
            }

        #endregion

        #region Metodos Privados Usuario

            private int CorrelativoUsuario()
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

            private string Key(string key)
            {
                return Criptografia.Base64StringAHexString(Criptografia.EncriptarSha512(key));
            }

            private string Concat_Usuario(string Usuario, string Password)
            {
                return string.Concat(Usuario, Password, Usuario);
            }

        #endregion

        #region Metodos Privados

            private int Correlativo()
            {
                int Id = 1;

                try
                {
                    Encargado EncargadoActual = db.Set<Encargado>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (EncargadoActual != null)
                    {
                        Id = EncargadoActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Nombre_Completo(Encargado entidad)
            {
                string Nombre = string.Empty;

                if (!string.IsNullOrWhiteSpace(entidad.PrimerNombre) && !string.IsNullOrWhiteSpace(entidad.SegundoNombre) && !string.IsNullOrWhiteSpace(entidad.PrimerApellido) && !string.IsNullOrWhiteSpace(entidad.SegundoApellido))
                {
                    Nombre = string.Format("{0} {1}, {2} {3}", entidad.PrimerApellido.Trim(), entidad.SegundoApellido.Trim(), entidad.PrimerNombre.Trim(), entidad.SegundoNombre.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(entidad.PrimerNombre) && string.IsNullOrWhiteSpace(entidad.SegundoNombre) && !string.IsNullOrWhiteSpace(entidad.PrimerApellido) && !string.IsNullOrWhiteSpace(entidad.SegundoApellido))
                {
                    Nombre = string.Format("{0} {1}, {2}", entidad.PrimerApellido.Trim(), entidad.SegundoApellido.Trim(), entidad.PrimerNombre.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(entidad.PrimerNombre) && string.IsNullOrWhiteSpace(entidad.SegundoNombre) && !string.IsNullOrWhiteSpace(entidad.PrimerApellido) && string.IsNullOrWhiteSpace(entidad.SegundoApellido))
                {
                    Nombre = string.Format("{0}, {1}", entidad.PrimerApellido.Trim(), entidad.PrimerNombre.Trim());
                }

                Nombre = Nombre.Trim();
                return Nombre;
            }

            private string Agregar(Encargado entidad)
            {
                string Mensaje = "OK";
                string Alias = string.Empty;

                try
                {
                    //Se obtiene el alias del colegio
                    Entities.Colegio ColegioActual = db.Set<Entities.Colegio>().AsNoTracking().Where(x => x.ColegioId == entidad.ColegioId).FirstOrDefault();
                    if (ColegioActual == null)
                    {
                        return "Se le informa que el colegio no se encuentra registrado en el sistema";
                    }

                    if (string.IsNullOrWhiteSpace(ColegioActual.Alias))
                    {
                        return "Se le informa que no tiene configurado el alias del colegio, por favor comunicarse con el administrador";
                    }

                    Alias = ColegioActual.Alias.ToLower();

                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngEncargadoId = new Herramienta().Formato_Correlativo(Id);

                        if (lngEncargadoId > 0)
                        {
                            entidad.EncargadoId = lngEncargadoId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            entidad.Nombre = Nombre_Completo(entidad);

                            //Se crea el encargado
                            db.Set<Encargado>().Add(entidad);

                            //Se verifica que no exista el usuario
                            string LoginOriginal = string.Format("{0}.{1}.e.{2}", entidad.PrimerNombre.ToLower(), entidad.PrimerApellido.ToLower(), Alias).Replace(" ", "_");
                            string Login = LoginOriginal;

                            int CantidadUsuario = db.Set<Usuario>().AsNoTracking().Where(x => x.LoginOriginal.Equals(Login)).Count();
                            if (CantidadUsuario > 0)
                            {
                                Login = string.Format("{0}.{1}.{2}.e.{3}", entidad.PrimerNombre.ToLower(), entidad.PrimerApellido.ToLower(), CantidadUsuario, Alias).Replace(" ", "_");
                            }

                            //Se crea usuario del encargado
                            Usuario UsuarioEncargado = new Usuario();
                            UsuarioEncargado.RelacionId = entidad.EncargadoId;
                            UsuarioEncargado.ColegioId = entidad.ColegioId;
                            UsuarioEncargado.RolId = 20200928004;
                            UsuarioEncargado.LoginOriginal = LoginOriginal;
                            UsuarioEncargado.Login = Login;
                            UsuarioEncargado.Password = Key(Concat_Usuario(UsuarioEncargado.Login, DateTime.Today.Year.ToString()));
                            UsuarioEncargado.Nombre = entidad.Nombre;
                            UsuarioEncargado.Fecha = DateTime.Today;
                            UsuarioEncargado.ReiniciarPassword = false;
                            UsuarioEncargado.Administrador = false;
                            UsuarioEncargado.Activo = true;

                            int UsuarioId = CorrelativoUsuario();

                            if (UsuarioId == 0)
                            {
                                return "Se le informa que no se creo el encargado(a), por favor intente de nuevo";
                            }

                            long lngUsuarioId = new Herramienta().Formato_Correlativo(UsuarioId);

                            if (lngUsuarioId > 0)
                            {
                                UsuarioEncargado.UsuarioId = lngUsuarioId;
                                UsuarioEncargado.Correlativo = UsuarioId;
                            }   

                            //Se crea el usuario del encargado
                            db.Set<Usuario>().Add(UsuarioEncargado);
                        
                            //Se agrega los alumnos al encargado
                            if (entidad.Alumnos != null && entidad.Alumnos.Count() > 0)
                            {
                                entidad.Alumnos.ForEach(x =>
                                {
                                    x.EncargadoId = entidad.EncargadoId;
                                });
                            }

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

            private string Actualizar(Encargado entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Encargado EncargadoActual = ObtenerxId(entidad.EncargadoId, false, false, true);

                    if (EncargadoActual.EncargadoId > 0)
                    {
                        EncargadoActual.TipoId = entidad.TipoId;
                        EncargadoActual.GeneroId = entidad.GeneroId;
                        EncargadoActual.EstadoCivilId = entidad.EstadoCivilId;

                        EncargadoActual.PrimerNombre = entidad.PrimerNombre;
                        EncargadoActual.SegundoNombre = entidad.SegundoNombre;
                        EncargadoActual.PrimerApellido = entidad.PrimerApellido;
                        EncargadoActual.SegundoApellido = entidad.SegundoApellido;

                        EncargadoActual.FechaNacimiento = entidad.FechaNacimiento;
                        EncargadoActual.LugarNacimiento = entidad.LugarNacimiento;
                        EncargadoActual.Profesion = entidad.Profesion;

                        EncargadoActual.Direccion = entidad.Direccion;

                        EncargadoActual.CorreoElectronico = entidad.CorreoElectronico;
                        EncargadoActual.TelefonoCelular = entidad.TelefonoCelular;
                        EncargadoActual.TelefonoCasa = entidad.TelefonoCasa;
                        EncargadoActual.TelefonoOficina = entidad.TelefonoOficina;

                        EncargadoActual.NombreEmpresa = entidad.NombreEmpresa;

                        EncargadoActual.Observaciones = entidad.Observaciones;

                        EncargadoActual.Activo = entidad.Activo;

                        EncargadoActual.Nombre = Nombre_Completo(entidad);

                        if (entidad.Alumnos != null && entidad.Alumnos.Count() > 0)
                        {
                            List<EncargadoAlumno> Alumnos = db.Set<EncargadoAlumno>().Where(x => x.EncargadoId == EncargadoActual.EncargadoId).ToList();
                            db.Set<EncargadoAlumno>().RemoveRange(Alumnos);

                            entidad.Alumnos.ForEach(x =>
                            {
                                x.EncargadoId = entidad.EncargadoId;
                                db.Set<EncargadoAlumno>().Add(x);
                            });
                        }

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El encargado(a) seleccionada no se encuentra con ID valido";
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

            public string Guardar(Encargado entidad)
            {
                string Mensaje = "OK";

                if (entidad.EncargadoId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Encargado ObtenerxId(long id, bool todo, bool usuario, bool alumnos)
            {
                Encargado EncargadoActual = new Encargado();

                try
                {
                    if (todo)
                    {
                        EncargadoActual = db.Set<Encargado>().Include("Tipo").Include("Genero").Include("EstadoCivil").AsNoTracking().Where(x => x.EncargadoId == id).FirstOrDefault();
                        if (usuario)
                        {
                            if (EncargadoActual != null)
                            {
                                EncargadoActual.Usuario = new Usuario();
                                EncargadoActual.Usuario = db.Set<Usuario>().AsNoTracking().Where(x => x.RelacionId == EncargadoActual.EncargadoId && x.RolId == 20200928004).FirstOrDefault();
                            }
                        }

                        if (alumnos)
                        {
                            if (EncargadoActual != null)
                            {
                                EncargadoActual.Alumnos = new List<EncargadoAlumno>();
                                EncargadoActual.Alumnos = db.Set<EncargadoAlumno>().Include("Alumno").AsNoTracking().Where(x => x.EncargadoId == EncargadoActual.EncargadoId).ToList();
                            }
                        }
                    }
                    else
                    {
                        EncargadoActual = db.Set<Encargado>().Where(x => x.EncargadoId == id).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }

                return EncargadoActual;
            }

            public List<Encargado> ObtenerListado(bool todo, long colegioId)
            {
                List<Encargado> Encargados = new List<Encargado>();

                try
                {
                    if (todo)
                    {
                        Encargados = db.Set<Encargado>().Include("Tipo").AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.EncargadoId).Take(200).ToList();
                    }
                    else
                    {
                        Encargados = db.Set<Encargado>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Encargados;
            }

            public List<Encargado> Buscar(string search, long colegioId)
            {
                List<Encargado> Encargados = new List<Encargado>();

                try
                {
                    Encargados = db.Set<Encargado>().Include("Tipo").AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.EncargadoId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Encargados;
            }

            public List<AlumnoxResponsable> ObtenerAlumnoxResponsable(long colegioId, long responsableId)
            {
                List<AlumnoxResponsable> Alumnos = new List<AlumnoxResponsable>();

                try
                {
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Alumnos = db.Database.SqlQuery<AlumnoxResponsable>("dbo.sp_consulta_alumno_x_responsable @ColegioId, @CicloId, @ResponsableId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@ResponsableId", responsableId)).ToList();
                    }
                }
                catch (Exception)
                { }

                return Alumnos;
            }

            public AlumnoxResponsable ObtenerAlumnoxId(long colegioId, long alumnoId, bool todo)
            {
                AlumnoxResponsable AlumnoActual = new AlumnoxResponsable();

                try
                {
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        AlumnoActual = db.Database.SqlQuery<AlumnoxResponsable>("dbo.sp_consulta_alumno_x_Id @ColegioId, @CicloId, @AlumnoId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId)).FirstOrDefault();
                        if (todo)
                        {
                            if (AlumnoActual != null)
                            {
                                AlumnoActual.Cursos = new List<NotaxCursoModel>();
                                AlumnoActual.Cursos = db.Database.SqlQuery<NotaxCursoModel>("dbo.sp_consulta_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", AlumnoActual.GradoId), new SqlParameter("@SeccionId", AlumnoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                            }
                        }
                    }
                }
                catch (Exception)
                { }

                return AlumnoActual;
            }

        #endregion
    }
}
