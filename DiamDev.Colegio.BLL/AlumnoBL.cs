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
    public class AlumnoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public AlumnoBL()
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
                    Alumno AlumnoActual = db.Set<Alumno>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (AlumnoActual != null)
                    {
                        Id = AlumnoActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }            

            private string Nombre_Completo(Alumno entidad)
            {
                string Nombre = string.Empty;

                if (!string.IsNullOrWhiteSpace(entidad.PrimerNombre) && !string.IsNullOrWhiteSpace(entidad.SegundoNombre) && !string.IsNullOrWhiteSpace(entidad.PrimerApellido) && !string.IsNullOrWhiteSpace(entidad.SegundoApellido))
                {
                    Nombre = string.Format("{0} {1}, {2} {3}",  entidad.PrimerApellido.Trim(), entidad.SegundoApellido.Trim(), entidad.PrimerNombre.Trim(), entidad.SegundoNombre.Trim());
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

            private string Agregar(Alumno entidad)
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
                        long lngAlumnoId = new Herramienta().Formato_Correlativo(Id);

                        if (lngAlumnoId > 0)
                        {
                            entidad.AlumnoId = lngAlumnoId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            entidad.Nombre = Nombre_Completo(entidad);

                            //Se crea el alumno
                            db.Set<Alumno>().Add(entidad);

                            //Se verifica que no exista el usuario
                            string LoginOriginal = string.Format("{0}.{1}.a.{2}", entidad.PrimerNombre.ToLower(), entidad.PrimerApellido.ToLower(), Alias).Replace(" ", "_");
                            string Login = LoginOriginal;

                            int CantidadUsuario = db.Set<Usuario>().AsNoTracking().Where(x => x.LoginOriginal.Equals(Login)).Count();
                            if (CantidadUsuario > 0)
                            {
                                Login = string.Format("{0}.{1}.{2}.a.{3}", entidad.PrimerNombre.ToLower(), entidad.PrimerApellido.ToLower(), CantidadUsuario, Alias).Replace(" ", "_");
                            }

                            //Se crea usuario del alumno
                            Usuario UsuarioAlumno = new Usuario();
                            UsuarioAlumno.RelacionId = entidad.AlumnoId;
                            UsuarioAlumno.ColegioId = entidad.ColegioId;
                            UsuarioAlumno.RolId = 20201001001;
                            UsuarioAlumno.LoginOriginal = LoginOriginal;
                            UsuarioAlumno.Login = Login;
                            UsuarioAlumno.Password = Key(Concat_Usuario(UsuarioAlumno.Login, DateTime.Today.Year.ToString()));
                            UsuarioAlumno.Nombre = entidad.Nombre;
                            UsuarioAlumno.Fecha = DateTime.Today;
                            UsuarioAlumno.ReiniciarPassword = false;
                            UsuarioAlumno.Administrador = false;
                            UsuarioAlumno.Activo = true;

                            int UsuarioId = CorrelativoUsuario();

                            if (UsuarioId == 0)
                            {
                                return "Se le informa que no se creo el alumno(a), por favor intente de nuevo";
                            }

                            long lngUsuarioId = new Herramienta().Formato_Correlativo(UsuarioId);

                            if (lngUsuarioId > 0)
                            {
                                UsuarioAlumno.UsuarioId = lngUsuarioId;
                                UsuarioAlumno.Correlativo = UsuarioId;
                            }   

                            db.Set<Usuario>().Add(UsuarioAlumno);
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

            private string Actualizar(Alumno entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Alumno AlumnoActual = ObtenerxId(entidad.AlumnoId, false, false, false, false);

                    if (AlumnoActual.AlumnoId > 0)
                    {
                        AlumnoActual.GeneroId = entidad.GeneroId;
                        AlumnoActual.EstadoCivilId = entidad.EstadoCivilId;

                        AlumnoActual.PrimerNombre = entidad.PrimerNombre;
                        AlumnoActual.SegundoNombre = entidad.SegundoNombre;
                        AlumnoActual.PrimerApellido = entidad.PrimerApellido;
                        AlumnoActual.SegundoApellido = entidad.SegundoApellido;

                        AlumnoActual.FechaNacimiento = entidad.FechaNacimiento;
                        AlumnoActual.LugarNacimiento = entidad.LugarNacimiento;

                        AlumnoActual.Direccion = entidad.Direccion;

                        AlumnoActual.Observaciones = entidad.Observaciones;

                        AlumnoActual.Activo = entidad.Activo;

                        AlumnoActual.Nombre = Nombre_Completo(entidad);

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El alumno(a) seleccionada no se encuentra con ID valido";
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

            public string Guardar(Alumno entidad)
            {
                string Mensaje = "OK";

                if (entidad.AlumnoId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public Alumno ObtenerxId(long id, bool todo, bool usuario, bool encargados, bool inscripciones)
            {
                Alumno AlumnoActual = new Alumno();

                try
                {
                    if (todo)
                    {
                        AlumnoActual = db.Set<Alumno>().Include("Genero").Include("EstadoCivil").AsNoTracking().Where(x => x.AlumnoId == id).FirstOrDefault();
                        if (usuario)
                        {
                            if (AlumnoActual != null)
                            {
                                AlumnoActual.Usuario = new Usuario();
                                AlumnoActual.Usuario = db.Set<Usuario>().AsNoTracking().Where(x => x.RelacionId == AlumnoActual.AlumnoId && x.RolId == 20201001001).FirstOrDefault();
                            }
                        }

                        if (encargados)
                        {
                            if (AlumnoActual != null)
                            {
                                List<long> EncagadoIds = db.Set<EncargadoAlumno>().AsNoTracking().Where(x => x.AlumnoId == AlumnoActual.AlumnoId).Select(x => x.EncargadoId).Distinct().ToList();
                                
                                AlumnoActual.Encargados = new List<Encargado>();

                                if (EncagadoIds != null && EncagadoIds.Count() > 0)
                                {
                                    AlumnoActual.Encargados = db.Set<Encargado>().Include("Tipo").AsNoTracking().Where(x => EncagadoIds.Contains(x.EncargadoId)).ToList();
                                }                                
                            }
                        }

                        if (inscripciones)
                        {
                            if (AlumnoActual != null)
                            {
                                AlumnoActual.Inscripciones = new List<Inscripcion>();
                                AlumnoActual.Inscripciones = db.Set<Inscripcion>().Include("Ciclo").Include("NivelAcademico").Include("Jornada").Include("Grado").Include("Grado.Jornada").Include("Seccion").AsNoTracking().Where(x => x.AlumnoId == AlumnoActual.AlumnoId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.InscripcionId).ToList();
                            }
                        }
                    }
                    else
                    {
                        AlumnoActual = db.Set<Alumno>().Where(x => x.AlumnoId == id).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }

                return AlumnoActual;
            }

            public List<Alumno> ObtenerListado(bool todo, long colegioId)
            {
                List<Alumno> Alumnos = new List<Alumno>();

                try
                {
                    if (todo)
                    {
                        Alumnos = db.Set<Alumno>().AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.AlumnoId).Take(200).ToList();
                    }
                    else
                    {
                        Alumnos = db.Set<Alumno>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Alumnos;
            }

            public List<Alumno> Buscar(string search, long colegioId)
            {
                List<Alumno> Alumnos = new List<Alumno>();

                try
                {
                    Alumnos = db.Set<Alumno>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.AlumnoId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Alumnos;
            }

            public List<Alumno> BuscarAlumnoxAutocompletar(string search, long colegioId)
            {
                List<Alumno> Alumnos = new List<Alumno>();

                try
                {
                    List<AlumnoConsultaModel> Consultas = new List<AlumnoConsultaModel>();

                    Consultas = db.Database.SqlQuery<AlumnoConsultaModel>("dbo.sp_busqueda_libre_alumno @Buscar, @ColegioId", new SqlParameter("@Buscar", search), new SqlParameter("@ColegioId", colegioId)).ToList();

                    if (Consultas != null && Consultas.Count() > 0)
                    {
                        Alumnos = Consultas.Select(x => new Alumno() { AlumnoId = x.AlumnoId, Nombre = x.Nombre }).ToList();
                    }
                }
                catch (Exception)
                { }

                return Alumnos;
            }

            public List<Alumno> BuscarAlumnoxTextoLibre(string search, long colegioId)
            {
                List<Alumno> Alumnos = new List<Alumno>();

                try
                {
                    List<AlumnoConsultaModel> Consultas = db.Database.SqlQuery<AlumnoConsultaModel>("dbo.sp_busqueda_libre_de_alumno @Buscar, @ColegioId", new SqlParameter("@Buscar", search), new SqlParameter("@ColegioId", colegioId)).ToList();
                    if (Consultas != null && Consultas.Count() > 0)
                    {
                        Alumnos = Consultas.Select(x => new Alumno() { AlumnoId = x.AlumnoId, Nombre = x.Nombre }).ToList();
                    }
                }
                catch (Exception)
                { }

                return Alumnos;
            }

            public AlumnoModel ObtenerxId(long colegioId, long alumnoId)
            {
            AlumnoModel AlumnoActual = new AlumnoModel();

                try
                {
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        AlumnoActual = db.Database.SqlQuery<AlumnoModel>("dbo.sp_consulta_encabezado_alumno_x_Id @ColegioId, @CicloId, @AlumnoId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId)).FirstOrDefault();                       
                    }
                }
                catch (Exception)
                { }

                return AlumnoActual;
            }

        #endregion
    }
}
