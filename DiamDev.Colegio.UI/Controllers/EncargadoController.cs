using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class EncargadoController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {
                var Tipos = new TipoEncargadoBL().ObtenerListado();
                var Generos = new GeneroBL().ObtenerListado();
                var Estados = new EstadoCivilBL().ObtenerListado();

                ViewBag.Tipos = new SelectList(Tipos, "TipoId", "Nombre");
                ViewBag.Generos = new SelectList(Generos, "GeneroId", "Nombre");
                ViewBag.Estados = new SelectList(Estados, "EstadoId", "Nombre");
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
                reporte.DataSources.Add(new ReportDataSource("Notas_Actitudinal", reportDataSource.Tables[4]));

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

        // GET: Encargado
        [Permiso("Colegio.Encargado.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Encargado(a)", "Listado");

            List<Encargado> Encargados = new List<Encargado>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Encargados = new EncargadoBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Encargados = new EncargadoBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Encargados.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Responsable_x_Alumno.Ver_Listado")]
        public ActionResult Alumno(int? page)
        {
            CustomHelper.setTitulo("Encargado(a)", "Alumnos");

            List<AlumnoxResponsable> Alumnos = new List<AlumnoxResponsable>();

            try
            {
                Alumnos = new EncargadoBL().ObtenerAlumnoxResponsable(CustomHelper.getColegioId(), CustomHelper.getUsuarioId());
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(Alumnos.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Encargado.Alumno_x_Curso")]
        public ActionResult Alumno_x_Curso(long id)
        {
            AlumnoxResponsable AlumnoActual = new EncargadoBL().ObtenerAlumnoxId(CustomHelper.getColegioId(), id, true);

            if (AlumnoActual == null || AlumnoActual.AlumnoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Encargado(a)", "Alumno Por Curso");

            return View(AlumnoActual);
        }

        [Permiso("Colegio.Encargado.Cuadro_x_Asistencia")]
        public ActionResult Cuadro_x_Asistencia(long id, long seccionId, long cursoId, long alumnoId, DateTime? fechaInicial, DateTime? fechaFinal)
        {
            if (!fechaInicial.HasValue && !fechaFinal.HasValue)
            {
                fechaInicial = DateTime.Today;
                fechaFinal = DateTime.Today;
            }

            CursoModel CursoActual = new GradoBL().ObtenerAsistenciaxCursoAlumno(id, seccionId, cursoId, CustomHelper.getColegioId(), alumnoId, fechaInicial.Value, fechaFinal.Value);

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Encargado(a)", "Cuadro de Asistencia Por Curso y Alumno");

            ViewBag.fechaInicial = fechaInicial.Value.ToString("yyyy-MM-dd");
            ViewBag.fechaFinal = fechaFinal.Value.ToString("yyyy-MM-dd");

            return View(CursoActual);
        }

        [Permiso("Colegio.Encargado.Cuadro_x_Curso")]
        public ActionResult Cuadro_x_Curso(long id, long seccionId, long cursoId, long alumnoId)
        {
            CursoModel CursoActual = new GradoBL().ObtenerCuadroxCursoAlumno(id, seccionId, cursoId, alumnoId, CustomHelper.getColegioId());

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Grado", "Cuadro De Actividades Por Curso");

            return View(CursoActual);
        }

        [Permiso("Colegio.Encargado.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Encargado(a)", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Encargado.Crear")]
        public ActionResult Crear(Encargado modelo, long[] alumnoIds, string[] nombreAlumnoIds, bool activo)
        {
            if (alumnoIds != null)
            {
                modelo.Alumnos = new List<EncargadoAlumno>();
                for (int i = 0; i < alumnoIds.Length; i++)
                {
                    EncargadoAlumno Alumno = new EncargadoAlumno();
                    Alumno.AlumnoId = alumnoIds[i];

                    modelo.Alumnos.Add(Alumno);
                }
            }

            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new EncargadoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Encargado-Success"] = strMensaje;
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

            ViewBag.alumnoIds = alumnoIds;
            ViewBag.nombreAlumnoIds = nombreAlumnoIds;

            this.CargaControles();
            return View(modelo);
        }

        [Permiso("Colegio.Encargado.Editar")]
        public ActionResult Editar(long id)
        {
            Encargado EncargadoActual = new EncargadoBL().ObtenerxId(id, true, false, true);

            if (EncargadoActual == null || EncargadoActual.EncargadoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Encargado(a)", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = EncargadoActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = EncargadoActual.Activo == false ? strAtributo : "";

            if (EncargadoActual.Alumnos != null && EncargadoActual.Alumnos.Count() > 0)
            {
                ViewBag.alumnoIds = EncargadoActual.Alumnos.Select(x => x.AlumnoId);
                ViewBag.nombreAlumnoIds = EncargadoActual.Alumnos.Select(x => x.Alumno.Nombre);
            }
            else
            {
                ViewBag.alumnoIds = 0;
                ViewBag.nombreAlumnoIds = "";
            }

            this.CargaControles();
            return View(EncargadoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Encargado.Editar")]
        public ActionResult Editar(Encargado modelo, long[] alumnoIds, string[] nombreAlumnoIds, bool activo)
        {
            if (alumnoIds != null)
            {
                modelo.Alumnos = new List<EncargadoAlumno>();
                for (int i = 0; i < alumnoIds.Length; i++)
                {
                    EncargadoAlumno Alumno = new EncargadoAlumno();
                    Alumno.AlumnoId = alumnoIds[i];

                    modelo.Alumnos.Add(Alumno);
                }
            }         

            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new EncargadoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Encargado-Success"] = strMensaje;
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

            ViewBag.alumnoIds = alumnoIds;
            ViewBag.nombreAlumnoIds = nombreAlumnoIds;

            this.CargaControles();
            return View(modelo);
        }

        [Permiso("Colegio.Encargado.Detalle")]
        public ActionResult Detalle(long id)
        {
            Encargado EncargadoActual = new EncargadoBL().ObtenerxId(id, true, true, true);

            if (EncargadoActual == null || EncargadoActual.EncargadoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Encargado(a)", "Detalle");

            return View(EncargadoActual);
        }

        [Permiso("Colegio.Encargado.Nota")]
        public ActionResult Nota(long id, long seccionId, long alumnoId)
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
                DataTable DetalleActitudinalNotas = new DataTable("Notas_Actitudinal");

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

                //Notas Actitudinal
                DetalleActitudinalNotas.Columns.Add(new DataColumn("AlumnoId", typeof(string)));
                DetalleActitudinalNotas.Columns.Add(new DataColumn("Curso", typeof(string)));
                DetalleActitudinalNotas.Columns.Add(new DataColumn("Unidad1", typeof(decimal)));
                DetalleActitudinalNotas.Columns.Add(new DataColumn("Unidad2", typeof(decimal)));
                DetalleActitudinalNotas.Columns.Add(new DataColumn("Unidad3", typeof(decimal)));
                DetalleActitudinalNotas.Columns.Add(new DataColumn("Unidad4", typeof(decimal)));
                DetalleActitudinalNotas.Columns.Add(new DataColumn("Total", typeof(decimal)));

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
                    List<string> CursoNormalesIds = TNotas.Where(x => x.Actitudinal == 0).OrderBy(x => x.Curso).Select(x => x.Curso).Distinct().ToList();
                    List<string> CursoActitudinalIds = TNotas.Where(x => x.Actitudinal == 1).OrderBy(x => x.Curso).Select(x => x.Curso).Distinct().ToList();

                    //Cursos Normales
                    int Correlativo = 1;
                    foreach (string CursoActual in CursoNormalesIds)
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

                    //Cursos Actitudinal
                    foreach (string CursoActual in CursoActitudinalIds)
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

                        DetalleActitudinalNotas.Rows.Add(alumnoId, CursoActual, NotaUnidad1, NotaUnidad2, NotaUnidad3, NotaUnidad4, Promedio);
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
                Notas.Tables.Add(DetalleActitudinalNotas);

                // Se define la ruta del reporte
                var reportPath = Server.MapPath("~/Reports/ReportNotaxAlumno.rdlc");

                // se obtienen los bytes del reporte en pdf
                var bytes = GetNotaReportBytes(reportPath, Notas, 8.5m, 11.0m, 0m, 0m);

                return File(bytes, "application/pdf");
            }

            return View();
        }

        [Permiso("Colegio.Encargado.Boleta_x_Asistencia")]
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
    }
}
