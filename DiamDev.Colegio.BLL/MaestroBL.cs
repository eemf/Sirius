using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using Sistema.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class MaestroBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public MaestroBL()
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
                    Maestro MaestroActual = db.Set<Maestro>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (MaestroActual != null)
                    {
                        Id = MaestroActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Nombre_Completo(Maestro entidad)
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

            private string Agregar(Maestro entidad)
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
                        long lngMaestroId = new Herramienta().Formato_Correlativo(Id);

                        if (lngMaestroId > 0)
                        {
                            entidad.MaestroId = lngMaestroId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            entidad.Nombre = Nombre_Completo(entidad);
                        
                            //Se crea el maestro
                            db.Set<Maestro>().Add(entidad); 
                        
                            //Se verifica que no exista el usuario
                            string LoginOriginal = string.Format("{0}.{1}.m.{2}", entidad.PrimerNombre.ToLower(), entidad.PrimerApellido.ToLower(), Alias).Replace(" ", "_");
                            string Login = LoginOriginal;

                            int CantidadUsuario = db.Set<Usuario>().AsNoTracking().Where(x => x.LoginOriginal.Equals(Login)).Count();
                            if (CantidadUsuario > 0)
                            {
                                Login = string.Format("{0}.{1}.{2}.m.{3}", entidad.PrimerNombre.ToLower(), entidad.PrimerApellido.ToLower(), CantidadUsuario, Alias).Replace(" ", "_");
                            }

                            //Se crea usuario del maestro
                            Usuario UsuarioMaestro = new Usuario();
                            UsuarioMaestro.RelacionId = entidad.MaestroId;
                            UsuarioMaestro.ColegioId = entidad.ColegioId;
                            UsuarioMaestro.RolId = 20200928003;
                            UsuarioMaestro.LoginOriginal = LoginOriginal;
                            UsuarioMaestro.Login = Login;
                            UsuarioMaestro.Password = Key(Concat_Usuario(UsuarioMaestro.Login, DateTime.Today.Year.ToString()));
                            UsuarioMaestro.Nombre = entidad.Nombre;
                            UsuarioMaestro.Fecha = DateTime.Today;
                            UsuarioMaestro.ReiniciarPassword = false;
                            UsuarioMaestro.Administrador = false;
                            UsuarioMaestro.Activo = true;

                            int UsuarioId = CorrelativoUsuario();

                            if (UsuarioId == 0)
                            {
                                return "Se le informa que no se creo el maestro(a), por favor intente de nuevo";
                            }

                            long lngUsuarioId = new Herramienta().Formato_Correlativo(UsuarioId);

                            if (lngUsuarioId > 0)
                            {
                                UsuarioMaestro.UsuarioId = lngUsuarioId;
                                UsuarioMaestro.Correlativo = UsuarioId;
                            }   

                            db.Set<Usuario>().Add(UsuarioMaestro);
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

            private string Actualizar(Maestro entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Maestro MaestroActual = ObtenerxId(entidad.MaestroId, false, false);

                    if (MaestroActual.MaestroId > 0)
                    {
                        MaestroActual.GeneroId = entidad.GeneroId;
                        MaestroActual.EstadoCivilId = entidad.EstadoCivilId;

                        MaestroActual.PrimerNombre = entidad.PrimerNombre;
                        MaestroActual.SegundoNombre = entidad.SegundoNombre;
                        MaestroActual.PrimerApellido = entidad.PrimerApellido;
                        MaestroActual.SegundoApellido = entidad.SegundoApellido;

                        MaestroActual.FechaNacimiento = entidad.FechaNacimiento;
                        MaestroActual.LugarNacimiento = entidad.LugarNacimiento;
                        MaestroActual.Profesion = entidad.Profesion;

                        MaestroActual.Direccion = entidad.Direccion;
                        MaestroActual.Telefono = entidad.Telefono;

                        MaestroActual.Observaciones = entidad.Observaciones;

                        MaestroActual.Activo = entidad.Activo;

                        MaestroActual.Nombre = Nombre_Completo(entidad);

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El maestro(a) seleccionada no se encuentra con ID valido";
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

            public string Guardar(Maestro entidad)
            {
                string Mensaje = "OK";

                if (entidad.MaestroId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public string GuardarAsignacionCurso(Maestro entidad)
            {
                string Mensaje = "OK";

                try
                {
                    if (entidad.Cursos != null && entidad.Cursos.Count() > 0)
                    {
                        //Se eliminan los cursos que tiene asignados
                        List<MaestroCurso> Cursos = db.Set<MaestroCurso>().Where(x => x.MaestroId == entidad.MaestroId).ToList();
                        db.Set<MaestroCurso>().RemoveRange(Cursos);
                        
                        entidad.Cursos.ForEach(x => 
                        {                            
                            x.MaestroId = entidad.MaestroId;

                            db.Set<MaestroCurso>().Add(x);                         
                        });

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            public Maestro ObtenerxId(long id, bool todo, bool usuario)
            {
                Maestro MaestroActual = new Maestro();

                try
                {
                    if (todo)
                    {
                        MaestroActual = db.Set<Maestro>().Include("Genero").Include("EstadoCivil").Include("EstadoCivil").Include("Cursos").Include("Cursos.Curso").Include("Cursos.Grado").Include("Cursos.Grado.Jornada").Include("Cursos.Seccion").AsNoTracking().Where(x => x.MaestroId == id).FirstOrDefault();
                        if (usuario)
                        {
                            if (MaestroActual != null)
                            {
                                MaestroActual.Usuario = new Usuario();
                                MaestroActual.Usuario = db.Set<Usuario>().AsNoTracking().Where(x => x.RelacionId == MaestroActual.MaestroId && x.RolId == 20200928003).FirstOrDefault();
                            }
                        }
                    }
                    else
                    {
                        MaestroActual = db.Set<Maestro>().Where(x => x.MaestroId == id).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }

                return MaestroActual;
            }            

            public List<Maestro> ObtenerListado(bool todo, long colegioId)
            {
                List<Maestro> Maestros = new List<Maestro>();

                try
                {
                    if (todo)
                    {
                        Maestros = db.Set<Maestro>().AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.MaestroId).Take(200).ToList();
                    }
                    else
                    {
                        Maestros = db.Set<Maestro>().AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Maestros;
            }

            public List<Maestro> Buscar(string search, long colegioId)
            {
                List<Maestro> Maestros = new List<Maestro>();

                try
                {
                    Maestros = db.Set<Maestro>().AsNoTracking().Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.MaestroId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Maestros;
            }

            public CursoMaestroModel ObtenerCursoAsignadoxId(string maestroId, long colegioId, bool actividad, bool cuadro, bool asistencia)
            {
                CursoMaestroModel CursoActual = new CursoMaestroModel();

                try
                {
                    CursoActual = db.Database.SqlQuery<CursoMaestroModel>("dbo.sp_consulta_curso_asignado_x_maestro @MaestroId", new SqlParameter("@MaestroId", maestroId)).FirstOrDefault();
                    if (actividad)
                    {
                        if (CursoActual != null)
                        {
                            CursoActual.Actividades = new List<ActividadModel>();

                            //Se obtiene el ciclo actual del colegio
                            Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                            if (CicloActual != null)
                            {
                                CursoActual.Actividades = db.Database.SqlQuery<ActividadModel>("dbo.sp_consulta_actividad_x_curso @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                            }
                        }
                    }

                    if (cuadro)
                    {
                        if (CursoActual != null)
                        {
                            CursoActual.Cuadros = new List<CuadroxCursoModel>();

                            //Se obtiene el ciclo actual del colegio
                            Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                            if (CicloActual != null)
                            {
                                CursoActual.Cuadros = db.Database.SqlQuery<CuadroxCursoModel>("dbo.sp_consulta_cuadro_x_curso @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                            }
                        }
                    }

                    if (asistencia)
                    {
                        if (CursoActual != null)
                        {
                            CursoActual.Asistencias = new List<AsistenciaxCursoModel>();

                            //Se obtiene el ciclo actual del colegio
                            Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                            if (CicloActual != null)
                            {
                                CursoActual.Asistencias = db.Database.SqlQuery<AsistenciaxCursoModel>("dbo.sp_consulta_asistencia_x_curso @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId, @Fecha", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@Fecha", DateTime.Today)).ToList();
                            }
                        }
                    }
                }
                catch (Exception)
                { }


                return CursoActual;
            }

            public List<CursoMaestroModel> ObtenerCursosAsignadoxID(long colegioId, long maestroId, int actitudinal) 
            {
                List<CursoMaestroModel> Cursos = new List<CursoMaestroModel>();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Cursos = db.Database.SqlQuery<CursoMaestroModel>("dbo.sp_consulta_curso_asignados_x_maestro @ColegioId, @MaestroId, @CicloId, @Actitudinal", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@MaestroId", maestroId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@Actitudinal", actitudinal)).ToList();
                    }               
                }
                catch (Exception)
                {}


                return Cursos;
            }

            public List<GradoxCicloModel> ObtenerGradoxId(long colegioId, long maestroId)
            {
                List<GradoxCicloModel> Grados = new List<GradoxCicloModel>();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Grados = db.Database.SqlQuery<GradoxCicloModel>("dbo.sp_consulta_grado_asignados_x_maestro @ColegioId, @MaestroId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@MaestroId", maestroId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                    }
                }
                catch (Exception)
                { }

                return Grados;
            }

        #endregion
    }
}
