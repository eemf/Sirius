using DiamDev.Colegio.DAL;
using DiamDev.Colegio.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiamDev.Colegio.BLL
{
    public class GradoBL
    {
        #region Variables Globales

            private ColegioContext db;

        #endregion

        #region Constructores

            public GradoBL()
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
                    Grado GradoActual = db.Set<Grado>().AsNoTracking().Where(x => x.Fecha.Year == DateTime.Today.Year && x.Fecha.Month == DateTime.Today.Month && x.Fecha.Day == DateTime.Today.Day).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                    if (GradoActual != null)
                    {
                        Id = GradoActual.Correlativo + 1;
                    }
                }
                catch (Exception)
                { }

                return Id;
            }

            private string Agregar(Grado entidad)
            {
                string Mensaje = "OK";               

                try
                {
                    int Id = Correlativo();

                    if (Id > 0)
                    {
                        long lngGradoId = new Herramienta().Formato_Correlativo(Id);

                        if (lngGradoId > 0)
                        {
                            entidad.GradoId = lngGradoId;
                            entidad.Correlativo = Id;
                            entidad.Fecha = DateTime.Today;

                            db.Set<Grado>().Add(entidad);
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

            private string Actualizar(Grado entidad)
            {
                string Mensaje = "OK";

                try
                {
                    Grado GradoActual = ObtenerxId(entidad.GradoId);

                    if (GradoActual.GradoId > 0)
                    {
                        GradoActual.NivelId = entidad.NivelId;
                        GradoActual.JornadaId = entidad.JornadaId;
                        GradoActual.Nombre = entidad.Nombre;
                        GradoActual.Precio = entidad.Precio;
                        GradoActual.Activo = entidad.Activo;

                        db.SaveChanges();                       
                    }
                    else
                    {
                        Mensaje = "El grado seleccionado no se encuentra con ID valido";
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

            public string Guardar(Grado entidad)
            {
                string Mensaje = "OK";

                if (entidad.GradoId > 0)
                {
                    Mensaje = Actualizar(entidad);
                }
                else
                {
                    Mensaje = Agregar(entidad);
                }

                return Mensaje;
            }

            public string GuardarAsistencia(List<Asistencia> entidad, long colegioId, long cursoId, long gradoId, long seccionId, long usuarioId, DateTime fecha)
            {
                string Mensaje = "OK";
                long CicloActualId = 0;
                DateTime FechaActual = fecha;

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CicloActualId = CicloActual.CicloId;
                    }

                    if (CicloActualId == 0)
                    {
                        return "Se le informa que el colegio no tiene activado el ciclo escolar";
                    }

                    //Se elimina las asistencias
                    List<Asistencia> Asistencias = db.Set<Asistencia>().Where(x => x.ColegioId == colegioId && x.CursoId == cursoId && x.GradoId == gradoId && x.SeccionId == seccionId && x.CicloId == CicloActualId && x.FechaAsistencia == FechaActual).ToList();
                    db.Set<Asistencia>().RemoveRange(Asistencias);

                    entidad.ForEach(x => 
                    {
                        x.CicloId = CicloActualId;
                        x.FechaAsistencia = FechaActual;
                        x.ResponsableId = usuarioId;
                        x.Fecha = DateTime.Today;

                        db.Set<Asistencia>().Add(x);
                    });

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Mensaje = string.Format("Descripción del Error {0}", ex.Message);
                }

                return Mensaje;
            }

            public Grado ObtenerxId(long id)
            {
                Grado GradoActual = new Grado();

                try
                {
                    GradoActual = db.Set<Grado>().Where(x => x.GradoId == id).FirstOrDefault();
                }
                catch (Exception)
                { }

                return GradoActual;
            }

            public List<Grado> ObtenerxNivelAcademico(long colegioId, long nivelId) 
            {
                List<Grado> Grados = new List<Grado>();

                try
                {
                    Grados = db.Set<Grado>().Include("Jornada").AsNoTracking().Where(x => x.ColegioId == colegioId && x.NivelId == nivelId).ToList();
                    if (Grados != null && Grados.Count() > 0)
                    {
                        Grados.ForEach(x => 
                        {
                            x.Nombre = string.Format("{0} - {1}", x.Nombre, x.Jornada.Nombre);
                        });
                    }
                }
                catch (Exception)
                {}

                return Grados;
            }

            public List<Grado> ObtenerxNivelAcademicoJornada(long colegioId, long nivelId, long jornadaId)
            {
                List<Grado> Grados = new List<Grado>();

                try
                {
                    Grados = db.Set<Grado>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.NivelId == nivelId && x.JornadaId == jornadaId).ToList();                   
                }
                catch (Exception)
                { }

                return Grados;
            }

            public List<Grado> ObtenerListado(bool todo, long colegioId)
            {
                List<Grado> Grados = new List<Grado>();

                try
                {
                    if (todo)
                    {
                        Grados = db.Set<Grado>().Include("Nivel").Include("Jornada").AsNoTracking().Where(x => x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.GradoId).Take(200).ToList();
                    }
                    else
                    {
                        Grados = db.Set<Grado>().Include("Nivel").Include("Jornada").AsNoTracking().Where(x => x.Activo && x.ColegioId == colegioId).Take(200).ToList();
                    }
                }
                catch (Exception)
                { }

                return Grados;
            }

            public List<Grado> Buscar(string search, long colegioId)
            {
                List<Grado> Grados = new List<Grado>();

                try
                {
                    Grados = db.Set<Grado>().AsNoTracking().Include("Nivel").Include("Jornada").Where(x => x.Nombre.ToLower().Contains(search.ToLower()) && x.ColegioId == colegioId).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.GradoId).Take(200).ToList();
                }
                catch (Exception)
                { }

                return Grados;
            }

            public List<GradoxCicloModel> ObtenerGradoxCiclo(long colegioId)
            {
                List<GradoxCicloModel> Grados = new List<GradoxCicloModel>();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Grados = db.Database.SqlQuery<GradoxCicloModel>("dbo.sp_consulta_grado_x_ciclo @ColegioId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                    }
                }
                catch (Exception)
                { }

                return Grados;
            }

            public List<NotaxCursoModel> ObtenerCursoxGrado(long colegioId, long gradoId, long seccionId)
            {
                List<NotaxCursoModel> Cursos = new List<NotaxCursoModel>();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Cursos = db.Database.SqlQuery<NotaxCursoModel>("dbo.sp_consulta_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                    }
                }
                catch (Exception)
                { }

                return Cursos;
            }

            public CursoModel ObtenerAsistenciaxCurso(long gradoId, long seccionId, long cursoId, long colegioId, DateTime fecha)
            {
                CursoModel CursoActual = new CursoModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CursoActual = db.Database.SqlQuery<CursoModel>("dbo.sp_consulta_detalle_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CursoId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CursoId", cursoId), new SqlParameter("@CicloId", CicloActual.CicloId)).FirstOrDefault();
                        if (CursoActual != null)
                        {
                            CursoActual.Asistencias = new List<AsistenciaxCursoModel>();
                            CursoActual.Asistencias = db.Database.SqlQuery<AsistenciaxCursoModel>("dbo.sp_consulta_asistencia_x_curso @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId, @Fecha", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@Fecha", fecha)).ToList();
                        }
                    }
                }
                catch (Exception)
                { }


                return CursoActual;
            }

            public CursoModel ObtenerAsistenciaxCursoAlumno(long gradoId, long seccionId, long cursoId, long colegioId, long alumnoId, DateTime fechaInicial, DateTime fechaFinal)
            {
                CursoModel CursoActual = new CursoModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CursoActual = db.Database.SqlQuery<CursoModel>("dbo.sp_consulta_detalle_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CursoId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CursoId", cursoId), new SqlParameter("@CicloId", CicloActual.CicloId)).FirstOrDefault();
                        if (CursoActual != null)
                        {
                            CursoActual.AlumnoId = alumnoId;

                            CursoActual.Asistencias = new List<AsistenciaxCursoModel>();
                            CursoActual.Asistencias = db.Database.SqlQuery<AsistenciaxCursoModel>("dbo.sp_consulta_asistencia_x_curso_alumno @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId, @AlumnoId, @FechaInicial, @FechaFinal", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId), new SqlParameter("@FechaInicial", fechaInicial), new SqlParameter("@FechaFinal", fechaFinal)).ToList();
                        }
                    }
                }
                catch (Exception)
                { }


                return CursoActual;
            }

            public CursoModel ReporteDiarioPedagogicoAsistenciaxCursoAlumno(long gradoId, long seccionId, long cursoId, long colegioId, long alumnoId, DateTime fechaInicial, DateTime fechaFinal)
            {
                CursoModel CursoActual = new CursoModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CursoActual = db.Database.SqlQuery<CursoModel>("dbo.sp_consulta_detalle_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CursoId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CursoId", cursoId), new SqlParameter("@CicloId", CicloActual.CicloId)).FirstOrDefault();
                        if (CursoActual != null)
                        {
                            CursoActual.AlumnoId = alumnoId;

                            CursoActual.Asistencias = new List<AsistenciaxCursoModel>();
                            CursoActual.Asistencias = db.Database.SqlQuery<AsistenciaxCursoModel>("dbo.sp_reporte_diario_pedagogico_asistencia_x_curso_alumno @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId, @AlumnoId, @FechaInicial, @FechaFinal", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId), new SqlParameter("@FechaInicial", fechaInicial), new SqlParameter("@FechaFinal", fechaFinal)).ToList();
                        }
                    }
                }
                catch (Exception)
                { }


                return CursoActual;
            }

            public CursoModel ObtenerCuadroxCurso(long gradoId, long seccionId, long cursoId, long colegioId)
            {
                CursoModel CursoActual = new CursoModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CursoActual = db.Database.SqlQuery<CursoModel>("dbo.sp_consulta_detalle_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CursoId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CursoId", cursoId), new SqlParameter("@CicloId", CicloActual.CicloId)).FirstOrDefault();
                        if (CursoActual != null)
                        {
                            CursoActual.Cuadros = new List<CuadroxCursoModel>();
                            CursoActual.Cuadros = db.Database.SqlQuery<CuadroxCursoModel>("dbo.sp_consulta_cuadro_x_curso @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                        }
                    }                  
                }
                catch (Exception)
                { }


                return CursoActual;
            }

            public CursoModel ObtenerCuadroxCursoAlumno(long gradoId, long seccionId, long cursoId, long alumnoId, long colegioId)
            {
                CursoModel CursoActual = new CursoModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        CursoActual = db.Database.SqlQuery<CursoModel>("dbo.sp_consulta_detalle_curso_x_grado @ColegioId, @GradoId, @SeccionId, @CursoId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CursoId", cursoId), new SqlParameter("@CicloId", CicloActual.CicloId)).FirstOrDefault();
                        if (CursoActual != null)
                        {
                            CursoActual.AlumnoId = alumnoId;

                            CursoActual.Cuadros = new List<CuadroxCursoModel>();
                            CursoActual.Cuadros = db.Database.SqlQuery<CuadroxCursoModel>("dbo.sp_consulta_cuadro_x_curso_alumno @ColegioId, @CursoId, @GradoId, @SeccionId, @CicloId, @AlumnoId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CursoId", CursoActual.CursoId), new SqlParameter("@GradoId", CursoActual.GradoId), new SqlParameter("@SeccionId", CursoActual.SeccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId)).ToList();
                        }
                    }
                }
                catch (Exception)
                { }


                return CursoActual;
            }

            public List<EncabezadoNotaModel> ObtenerNotasxGrado(long colegioId, long gradoId, long seccionId)
            {
                List<EncabezadoNotaModel> Alumnos = new List<EncabezadoNotaModel>();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Alumnos = db.Database.SqlQuery<EncabezadoNotaModel>("dbo.sp_consulta_encabezado_notas_x_grado_seccion @ColegioId, @CicloId, @GradoId, @SeccionId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId)).ToList();
                    }
                }
                catch (Exception)
                { }

                return Alumnos;
            }           

            public GradoModel ObtenerEncabezadoGrado(long gradoId, long seccionId, long colegioId)
            {
                GradoModel GradoActual = new GradoModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        GradoActual = db.Database.SqlQuery<GradoModel>("dbo.sp_consulta_encabezado_grado @ColegioId, @CicloId, @GradoId, @SeccionId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId)).FirstOrDefault();
                        if (GradoActual != null)
                        {
                            GradoActual.Alumnos = new List<AlumnoxGrado>();
                            GradoActual.Alumnos = db.Database.SqlQuery<AlumnoxGrado>("dbo.sp_consulta_alumno_x_grado @ColegioId, @GradoId, @SeccionId, @CicloId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CicloId", CicloActual.CicloId)).ToList();
                    }
                    }
                }
                catch (Exception)
                { }


                return GradoActual;
            }

            public List<NotaModel> ObtenerNotasxAlumno(long gradoId, long seccionId, long colegioId, long alumnoId)
            {
                List<NotaModel> Notas = new List<NotaModel>();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        Notas = db.Database.SqlQuery<NotaModel>("dbo.sp_consulta_nota_x_alumno @ColegioId, @GradoId, @SeccionId, @CicloId, @AlumnoId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId)).ToList();
                    }
                }
                catch (Exception)
                { }


                return Notas;
            }

            public AsistenciaModel ObtenerAsistenciaxAlumno(long gradoId, long seccionId, long colegioId, long alumnoId)
            {
                AsistenciaModel AsistenciaActual = new AsistenciaModel();

                try
                {
                    //Se obtiene el ciclo actual del colegio
                    Ciclo CicloActual = db.Set<Ciclo>().AsNoTracking().Where(x => x.ColegioId == colegioId && x.Activo).OrderByDescending(x => x.Fecha).ThenByDescending(x => x.CicloId).FirstOrDefault();
                    if (CicloActual != null)
                    {
                        AsistenciaActual = db.Database.SqlQuery<AsistenciaModel>("dbo.sp_consulta_asistencia_resumen_x_alumno @ColegioId, @GradoId, @SeccionId, @CicloId, @AlumnoId", new SqlParameter("@ColegioId", colegioId), new SqlParameter("@GradoId", gradoId), new SqlParameter("@SeccionId", seccionId), new SqlParameter("@CicloId", CicloActual.CicloId), new SqlParameter("@AlumnoId", alumnoId)).FirstOrDefault();
                    }
                }
                catch (Exception)
                { }


                return AsistenciaActual;
            }

        #endregion
    }
}
