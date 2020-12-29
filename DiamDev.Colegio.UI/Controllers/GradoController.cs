using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DiamDev.Colegio.BLL;
using DiamDev.Colegio.Entities;
using DiamDev.Colegio.UI.App_Start;
using Microsoft.Reporting.WebForms;
using PagedList;

namespace DiamDev.Colegio.UI.Controllers
{
    [Authorize]
    [Seguridad]
    [HandleError]
    public class GradoController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {
                var Niveles = new NivelAcademicoBL().ObtenerListado(false, CustomHelper.getColegioId());
                var Jornadas = new JornadaBL().ObtenerListado(false, CustomHelper.getColegioId());

                ViewBag.Niveles = new SelectList(Niveles, "NivelId", "Nombre");
                ViewBag.Jornadas = new SelectList(Jornadas, "JornadaId", "Nombre");
            }

            private byte[] GetAsistenciaReportBytes(string reportPath, DataSet reportDataSource, decimal pageWidth = 13.38m, decimal pageHeight = 8.5m, decimal MarginLeft = 1m, decimal MarginRight = 1m)
            {
                byte[] reportBytes = null;

                // Se crea la instancia del reporte y se cargan sus datos.
                LocalReport reporte = new LocalReport() { ReportPath = reportPath };
                reporte.DataSources.Add(new ReportDataSource("EncabezadoColegio", reportDataSource.Tables[0]));
                reporte.DataSources.Add(new ReportDataSource("EncabezadoAlumno", reportDataSource.Tables[1]));
                reporte.DataSources.Add(new ReportDataSource("Asistencias", reportDataSource.Tables[2]));

                string deviceInfo =
                    "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" + // Formato del documento PDF
                    "  <PageWidth>" + pageWidth + "in</PageWidth>" + // Ancho de 8.5 pulgadas para paginas oficio
                    "  <PageHeight>" + pageHeight + "in</PageHeight>" + // Alto de 13.38 pulgadas para paginas oficio
                    "  <MarginTop>0.0in</MarginTop>" + // margen superior de 0.5 pulgadas
                    "  <MarginLeft>" + MarginLeft + "</MarginLeft>" + // margen izquierdo de 1 pulgada
                    "  <MarginRight>" + MarginRight + "</MarginRight>" + // margen derecho de 1 pulgada.
                    "  <MarginBottom>0.0in</MarginBottom>" + // margen inferior de 0.5 pulgadas.
                    "</DeviceInfo>";

                string mimeType;
                string encoding;
                string fileNameExtension;
                Warning[] warnings;
                string[] streams;

                // Se renderiza el reporte.
                reportBytes = reporte.Render("PDF",
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                return reportBytes;
            }

            private byte[] GetNotaReportBytes(string reportPath, DataSet reportDataSource, decimal pageWidth = 13.38m, decimal pageHeight = 8.5m, decimal MarginLeft = 1m, decimal MarginRight = 1m)
            {
                byte[] reportBytes = null;

                // Se crea la instancia del reporte y se cargan sus datos.
                LocalReport reporte = new LocalReport() { ReportPath = reportPath };
                reporte.DataSources.Add(new ReportDataSource("EncabezadoColegio", reportDataSource.Tables[0]));
                reporte.DataSources.Add(new ReportDataSource("EncabezadoAlumno", reportDataSource.Tables[1]));
                reporte.DataSources.Add(new ReportDataSource("Notas", reportDataSource.Tables[2]));
                reporte.DataSources.Add(new ReportDataSource("Asistencia", reportDataSource.Tables[3]));

                string deviceInfo =
                    "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" + // Formato del documento PDF
                    "  <PageWidth>" + pageWidth + "in</PageWidth>" + // Ancho de 8.5 pulgadas para paginas oficio
                    "  <PageHeight>" + pageHeight + "in</PageHeight>" + // Alto de 13.38 pulgadas para paginas oficio
                    "  <MarginTop>0.0in</MarginTop>" + // margen superior de 0.5 pulgadas
                    "  <MarginLeft>" + MarginLeft + "</MarginLeft>" + // margen izquierdo de 1 pulgada
                    "  <MarginRight>" + MarginRight + "</MarginRight>" + // margen derecho de 1 pulgada.
                    "  <MarginBottom>0.0in</MarginBottom>" + // margen inferior de 0.5 pulgadas.
                    "</DeviceInfo>";

                string mimeType;
                string encoding;
                string fileNameExtension;
                Warning[] warnings;
                string[] streams;

                // Se renderiza el reporte.
                reportBytes = reporte.Render("PDF",
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                return reportBytes;
            }

        #endregion

        // GET: Grado
        [Permiso("Colegio.Grado.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Grado", "Listado");

            List<Grado> Grados = new List<Grado>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Grados = new GradoBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Grados = new GradoBL().ObtenerListado(true, CustomHelper.getColegioId());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }

            ViewBag.Search = search;

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(Grados.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Grado.Asistencia")]
        public ActionResult Asistencia(int? page)
        {
            CustomHelper.setTitulo("Grado", "Asistencia");

            List<GradoxCicloModel> Grados = new List<GradoxCicloModel>();

            try
            {
                Grados = new GradoBL().ObtenerGradoxCiclo(CustomHelper.getColegioId());
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(Grados.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Grado.Asistencia_x_Curso")]
        public ActionResult Asistencia_x_Curso(long id, long seccionId)
        {
            CustomHelper.setTitulo("Grado", "Asistencia Por Curso");

            List<NotaxCursoModel> Cursos = new List<NotaxCursoModel>();

            try
            {
                Cursos = new GradoBL().ObtenerCursoxGrado(CustomHelper.getColegioId(), id, seccionId);
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }

            return View(Cursos);
        }

        [Permiso("Colegio.Grado.Cuadro_x_Asistencia")]
        public ActionResult Cuadro_x_Asistencia(long id, long seccionId, long cursoId, DateTime? fecha)
        {
            if (!fecha.HasValue)
            {
                fecha = DateTime.Today;                
            }

            CursoModel CursoActual = new GradoBL().ObtenerAsistenciaxCurso(id, seccionId, cursoId, CustomHelper.getColegioId(), fecha.Value);

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Grado", "Cuadro de Asistencia Por Curso");

            ViewBag.fecha = fecha.Value.ToString("yyyy-MM-dd");

            return View(CursoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Grado.Cuadro_x_Asistencia")]
        public ActionResult Cuadro_x_Asistencia(CursoModel modelo, long[] alumnoId, int[] asistencia, string[] comentario)
        {
            List<Asistencia> Asistencias = new List<Asistencia>();

            if (alumnoId != null && alumnoId.Length > 0)
            {
                for (int i = 0; i < alumnoId.Length; i++)
                {
                    Asistencia AsistenciaActual = new Asistencia();

                    AsistenciaActual.ColegioId = modelo.ColegioId;
                    AsistenciaActual.CursoId = modelo.CursoId;
                    AsistenciaActual.GradoId = modelo.GradoId;
                    AsistenciaActual.SeccionId = modelo.SeccionId;

                    AsistenciaActual.AlumnoId = alumnoId[i];

                    AsistenciaActual.Si = asistencia[i] == 0 ? true : false;
                    AsistenciaActual.No = asistencia[i] == 1 ? true : false;
                    AsistenciaActual.Tarde = asistencia[i] == 2 ? true : false;

                    AsistenciaActual.Comentario = string.IsNullOrWhiteSpace(comentario[i]) ? "" : comentario[i].Trim();

                    Asistencias.Add(AsistenciaActual);
                }
            }

            string strMensaje = new GradoBL().GuardarAsistencia(Asistencias, modelo.ColegioId, modelo.CursoId, modelo.GradoId, modelo.SeccionId, CustomHelper.getUsuarioId(), modelo.Fecha);

            if (strMensaje.Equals("OK"))
            {
                TempData["Maestro_Asistencia-Success"] = strMensaje;
                return RedirectToAction("Cuadro_x_Asistencia", new { id = modelo.GradoId, seccionId = modelo.SeccionId, cursoId = modelo.CursoId, fecha = modelo.Fecha.ToString("yyyy-MM-dd") });
            }
            else
            {
                ModelState.AddModelError("", strMensaje);
            }

            return View(new GradoBL().ObtenerAsistenciaxCurso(modelo.GradoId, modelo.SeccionId, modelo.CursoId, CustomHelper.getColegioId(), modelo.Fecha));
        }

        [Permiso("Colegio.Grado.Nota")]
        public ActionResult Nota(int? page)
        {
            CustomHelper.setTitulo("Grado", "Notas");

            List<GradoxCicloModel> Grados = new List<GradoxCicloModel>();

            try
            {
                Grados = new GradoBL().ObtenerGradoxCiclo(CustomHelper.getColegioId());
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }           

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(Grados.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Grado.Nota_x_Curso")]
        public ActionResult Nota_x_Curso(long id, long seccionId)
        {
            CustomHelper.setTitulo("Grado", "Notas Por Curso");

            List<NotaxCursoModel> Cursos = new List<NotaxCursoModel>();

            try
            {
                Cursos = new GradoBL().ObtenerCursoxGrado(CustomHelper.getColegioId(), id, seccionId);
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }
            
            return View(Cursos);
        }

        [Permiso("Colegio.Grado.Notas_x_Grado")]
        public ActionResult Nota_x_Alumno(long id, long seccionId)
        {
            CustomHelper.setTitulo("Grado", "Notas");

            GradoModel GradoActual = new GradoModel();

            try
            {
                GradoActual = new GradoBL().ObtenerEncabezadoGrado(id, seccionId, CustomHelper.getColegioId());
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }

            return View(GradoActual);
        }

        [Permiso("Colegio.Grado.Cuadro_x_Curso")]
        public ActionResult Cuadro_x_Curso(long id, long seccionId, long cursoId)
        {
            CursoModel CursoActual = new GradoBL().ObtenerCuadroxCurso(id, seccionId, cursoId, CustomHelper.getColegioId());

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Grado", "Cuadro De Actividades Por Curso");

            return View(CursoActual);
        }

        [Permiso("Colegio.Grado.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Grado", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Grado.Crear")]
        public ActionResult Crear(Grado modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new GradoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Grado-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = activo == true ? strAtributo : "";
            ViewBag.ActivoNo = activo == false ? strAtributo : "";

            this.CargaControles();
            return View(modelo);
        }

        [Permiso("Colegio.Grado.Editar")]
        public ActionResult Editar(long id)
        {
            Grado GradoActual = new GradoBL().ObtenerxId(id);

            if (GradoActual == null || GradoActual.GradoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Grado", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = GradoActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = GradoActual.Activo == false ? strAtributo : "";

            this.CargaControles();
            return View(GradoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Grado.Editar")]
        public ActionResult Editar(Grado modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new GradoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Grado-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = activo == true ? strAtributo : "";
            ViewBag.ActivoNo = activo == false ? strAtributo : "";

            this.CargaControles();
            return View(modelo);
        }

        [Permiso("Colegio.Grado.Notas_x_Grado")]
        public ActionResult Boleta(long id, long seccionId, long alumnoId)
        {
            long ColegioActualId = CustomHelper.getColegioId();
            string PathLogo = ConfigurationManager.AppSettings["Path_LogoApp"].ToString();

            Entities.Colegio ColegioActual = new ColegioBL().ObtenerxId(ColegioActualId);

            if (ColegioActual != null)
            {
                DataSet Notas = new DataSet("Notas");

                DataTable EncabezadoColegio = new DataTable("EncabezadoColegio");
                DataTable EncabezadoAlumno = new DataTable("EncabezadoAlumno");
                DataTable DetalleNotas = new DataTable("Notas");
                DataTable Asistencia = new DataTable("Asistencia");

                //Encabezado del colegio
                EncabezadoColegio.Columns.Add(new DataColumn("ColegioId", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Colegio", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Direccion", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Telefono", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Logo", typeof(byte[])));

                byte[] Logo = null;

                //Se crea carpeta por colegio para almacenar el logo
                string Colegio_Logo = string.Format(@"{0}\{1}\logo.png", PathLogo, ColegioActualId);

                if (System.IO.File.Exists(Colegio_Logo))
                {
                    Logo = System.IO.File.ReadAllBytes(Colegio_Logo);
                }
                else
                {
                    Logo = System.IO.File.ReadAllBytes(string.Format(@"{0}\logo.jpeg", PathLogo));
                }

                EncabezadoColegio.Rows.Add(ColegioActual.ColegioId, ColegioActual.Nombre, ColegioActual.Direccion, ColegioActual.Telefono, Logo);

                //Encabezado del alumno
                EncabezadoAlumno.Columns.Add(new DataColumn("AlumnoId", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Alumno", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Ciclo", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Grado", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Seccion", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Jornada", typeof(string)));

                //Notas
                DetalleNotas.Columns.Add(new DataColumn("AlumnoId", typeof(string)));
                DetalleNotas.Columns.Add(new DataColumn("Numero", typeof(string)));
                DetalleNotas.Columns.Add(new DataColumn("Curso", typeof(string)));
                DetalleNotas.Columns.Add(new DataColumn("Unidad1", typeof(decimal)));
                DetalleNotas.Columns.Add(new DataColumn("Unidad2", typeof(decimal)));
                DetalleNotas.Columns.Add(new DataColumn("Unidad3", typeof(decimal)));
                DetalleNotas.Columns.Add(new DataColumn("Unidad4", typeof(decimal)));
                DetalleNotas.Columns.Add(new DataColumn("Total", typeof(decimal)));

                //Asistencia
                Asistencia.Columns.Add(new DataColumn("AlumnoId", typeof(string)));
                Asistencia.Columns.Add(new DataColumn("Si", typeof(int)));
                Asistencia.Columns.Add(new DataColumn("No", typeof(int)));
                Asistencia.Columns.Add(new DataColumn("Tarde", typeof(int)));

                //Se cargan el alumno
                AlumnoModel AlumnoActual = new AlumnoBL().ObtenerxId(ColegioActualId, alumnoId);

                if (AlumnoActual != null)
                {
                    EncabezadoAlumno.Rows.Add(alumnoId, AlumnoActual.Alumno, string.Format("CICLO ESCOLAR {0}", AlumnoActual.Ciclo), AlumnoActual.Grado, AlumnoActual.Seccion, AlumnoActual.Jornada);
                }

                //Se carga las notas
                List<NotaModel> TNotas = new GradoBL().ObtenerNotasxAlumno(id, seccionId, ColegioActualId, alumnoId);
                if (TNotas != null && TNotas.Count() > 0)
                {
                    List<long> UnidadIds = new List<long>() { 20201108001, 20201108002, 20201108003, 20201108004 };
                    List<string> CursoIds = TNotas.OrderBy(x => x.Curso).Select(x => x.Curso).Distinct().ToList();

                    int Correlativo = 1;
                    foreach (string CursoActual in CursoIds)
                    {
                        decimal NotaUnidad1 = 0;
                        decimal NotaUnidad2 = 0;
                        decimal NotaUnidad3 = 0;
                        decimal NotaUnidad4 = 0;

                        decimal TPromedio = 0;
                        decimal Promedio = 0;

                        int CantidadUnidad = 1;

                        NotaModel TNotaUnidad1 = TNotas.Where(x => x.Curso.Equals(CursoActual) && x.UnidadId == 20201108001).FirstOrDefault();
                        if (TNotaUnidad1 != null)
                        {
                            NotaUnidad1 = TNotaUnidad1.Nota;
                        }

                        NotaModel TNotaUnidad2 = TNotas.Where(x => x.Curso.Equals(CursoActual) && x.UnidadId == 20201108002).FirstOrDefault();
                        if (TNotaUnidad2 != null)
                        {
                            NotaUnidad2 = TNotaUnidad2.Nota;

                            if (NotaUnidad2 > 0)
                            {
                                CantidadUnidad = 2;
                            }
                        }

                        NotaModel TNotaUnidad3 = TNotas.Where(x => x.Curso.Equals(CursoActual) && x.UnidadId == 20201108003).FirstOrDefault();
                        if (TNotaUnidad3 != null)
                        {
                            NotaUnidad3 = TNotaUnidad3.Nota;

                            if (NotaUnidad3 > 0)
                            {
                                CantidadUnidad = 3;
                            }
                        }

                        NotaModel TNotaUnidad4 = TNotas.Where(x => x.Curso.Equals(CursoActual) && x.UnidadId == 20201108004).FirstOrDefault();
                        if (TNotaUnidad4 != null)
                        {
                            NotaUnidad4 = TNotaUnidad4.Nota;

                            if (NotaUnidad4 > 0)
                            {
                                CantidadUnidad = 4;
                            }
                        }

                        TPromedio = decimal.Round(NotaUnidad1 + NotaUnidad2 + NotaUnidad3 + NotaUnidad4, 2);
                        if (TPromedio > 0)
                        {
                            Promedio = TPromedio / CantidadUnidad;
                        }

                        DetalleNotas.Rows.Add(alumnoId, Correlativo, CursoActual, NotaUnidad1, NotaUnidad2, NotaUnidad3, NotaUnidad4, Promedio);
                        Correlativo++;
                    }
                }

                //Se cargan la asistencia
                AsistenciaModel AsistenciaActual = new GradoBL().ObtenerAsistenciaxAlumno(id, seccionId, ColegioActualId, alumnoId);

                if (AsistenciaActual != null)
                {
                    Asistencia.Rows.Add(alumnoId, AsistenciaActual.Si, AsistenciaActual.No, AsistenciaActual.Tarde);
                }

                Notas.Tables.Add(EncabezadoColegio);
                Notas.Tables.Add(EncabezadoAlumno);
                Notas.Tables.Add(DetalleNotas);
                Notas.Tables.Add(Asistencia);

                // Se define la ruta del reporte
                var reportPath = Server.MapPath("~/Reports/ReportNotaxAlumno.rdlc");

                // se obtienen los bytes del reporte en pdf
                var bytes = GetNotaReportBytes(reportPath, Notas, 8.5m, 11.0m, 0m, 0m);

                return File(bytes, "application/pdf");
            }

            return View();
        }

        [Permiso("Colegio.Grado.Boleta_x_Asistencia")]
        public ActionResult Boleta_x_Asistencia(long id, long seccionId, long cursoId, long alumnoId, DateTime? fechaInicial, DateTime? fechaFinal)
        {
            long ColegioActualId = CustomHelper.getColegioId();
            string PathLogo = ConfigurationManager.AppSettings["Path_LogoApp"].ToString();

            Entities.Colegio ColegioActual = new ColegioBL().ObtenerxId(ColegioActualId);

            if (ColegioActual != null)
            {
                DataSet Notas = new DataSet("Notas");

                DataTable EncabezadoColegio = new DataTable("EncabezadoColegio");
                DataTable EncabezadoAlumno = new DataTable("EncabezadoAlumno");
                DataTable Asistencias = new DataTable("Asistencias");

                //Encabezado del colegio
                EncabezadoColegio.Columns.Add(new DataColumn("ColegioId", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Colegio", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Direccion", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Telefono", typeof(string)));
                EncabezadoColegio.Columns.Add(new DataColumn("Logo", typeof(byte[])));

                byte[] Logo = null;

                //Se crea carpeta por colegio para almacenar el logo
                string Colegio_Logo = string.Format(@"{0}\{1}\logo.png", PathLogo, ColegioActualId);

                if (System.IO.File.Exists(Colegio_Logo))
                {
                    Logo = System.IO.File.ReadAllBytes(Colegio_Logo);
                }
                else
                {
                    Logo = System.IO.File.ReadAllBytes(string.Format(@"{0}\logo.jpeg", PathLogo));
                }

                EncabezadoColegio.Rows.Add(ColegioActual.ColegioId, ColegioActual.Nombre, ColegioActual.Direccion, ColegioActual.Telefono, Logo);

                //Encabezado del alumno
                EncabezadoAlumno.Columns.Add(new DataColumn("AlumnoId", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Alumno", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Ciclo", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Grado", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Seccion", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Jornada", typeof(string)));
                EncabezadoAlumno.Columns.Add(new DataColumn("Curso", typeof(string)));

                //Asistencias
                Asistencias.Columns.Add(new DataColumn("AlumnoId", typeof(string)));
                Asistencias.Columns.Add(new DataColumn("Fecha", typeof(DateTime)));
                Asistencias.Columns.Add(new DataColumn("Asistencia", typeof(string)));
                Asistencias.Columns.Add(new DataColumn("Comentario", typeof(string)));

                //Se cargan el alumno
                AlumnoModel AlumnoActual = new AlumnoBL().ObtenerxId(ColegioActualId, alumnoId);

                //Se carga la informacion de asistencia
                CursoModel CursoActual = new GradoBL().ReporteDiarioPedagogicoAsistenciaxCursoAlumno(id, seccionId, cursoId, ColegioActualId, alumnoId, fechaInicial.Value, fechaFinal.Value);
                if (CursoActual != null)
                {
                    if (AlumnoActual != null)
                    {
                        EncabezadoAlumno.Rows.Add(alumnoId, AlumnoActual.Alumno, string.Format("CICLO ESCOLAR {0}", AlumnoActual.Ciclo), AlumnoActual.Grado, AlumnoActual.Seccion, AlumnoActual.Jornada, CursoActual.Curso);
                    }

                    if (CursoActual.Asistencias != null && CursoActual.Asistencias.Count() > 0)
                    {
                        CursoActual.Asistencias = CursoActual.Asistencias.OrderBy(x => x.FechaAsistencia).ToList();
                        CursoActual.Asistencias.ForEach(x =>
                        {
                            Asistencias.Rows.Add(alumnoId, x.FechaAsistencia, (x.No ? "NO ASISTIO" : "LLEGO TARDE"), x.Comentario);
                        });
                    }
                }
                else
                {
                    if (AlumnoActual != null)
                    {
                        EncabezadoAlumno.Rows.Add(alumnoId, AlumnoActual.Alumno, string.Format("CICLO ESCOLAR {0}", AlumnoActual.Ciclo), AlumnoActual.Grado, AlumnoActual.Seccion, AlumnoActual.Jornada, "SIN CURSO ASIGNADO");
                    }
                }

                Notas.Tables.Add(EncabezadoColegio);
                Notas.Tables.Add(EncabezadoAlumno);
                Notas.Tables.Add(Asistencias);

                // Se define la ruta del reporte
                var reportPath = Server.MapPath("~/Reports/ReportAsistenciaxCurso.rdlc");

                // se obtienen los bytes del reporte en pdf
                var bytes = GetAsistenciaReportBytes(reportPath, Notas, 8.5m, 11.0m, 0m, 0m);

                return File(bytes, "application/pdf");
            }

            return View();
        }

        [ActionName("ObtenerGradoxNivelAcademico")]
        public JsonResult ObtenerGradoxNivelAcademico(long id)
        {
            IList _result = new List<SelectListItem>();
            _result = new GradoBL().ObtenerxNivelAcademico(CustomHelper.getColegioId(), id).Select(m => new SelectListItem() { Text = m.Nombre, Value = m.GradoId.ToString() }).ToList();
            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        [ActionName("ObtenerGradoxNivelAcademicoJornada")]
        public JsonResult ObtenerGradoxNivelAcademicoJornada(long id, long jornadaId)
        {
            IList _result = new List<SelectListItem>();
            _result = new GradoBL().ObtenerxNivelAcademicoJornada(CustomHelper.getColegioId(), id, jornadaId).Select(m => new SelectListItem() { Text = m.Nombre, Value = m.GradoId.ToString() }).ToList();
            return Json(_result, JsonRequestBehavior.AllowGet);
        }
    }
}