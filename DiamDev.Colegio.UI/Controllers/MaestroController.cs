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
    public class MaestroController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {
                var Generos = new GeneroBL().ObtenerListado();
                var Estados = new EstadoCivilBL().ObtenerListado();

                ViewBag.Generos = new SelectList(Generos, "GeneroId", "Nombre");
                ViewBag.Estados = new SelectList(Estados, "EstadoId", "Nombre");
            }

            private void CargaNiveles()
            {                
                var Niveles = new NivelAcademicoBL().ObtenerListado(false, CustomHelper.getColegioId());
            
                ViewBag.Niveles = new SelectList(Niveles, "NivelId", "Nombre");
            }

            private void CargaSecciones()
            {
                var Secciones = new SeccionBL().ObtenerListado(false, CustomHelper.getColegioId());

                ViewBag.Secciones = new SelectList(Secciones, "SeccionId", "Nombre");
            }

            private void CargaUnidades()
            {
                var Unidades = new UnidadBL().ObtenerListado();

                ViewBag.Unidades = new SelectList(Unidades, "UnidadId", "Nombre");
            }

            private void CargaTipos()
            {
                var Tipos = new ActividadTipoBL().ObtenerListado();

                ViewBag.Tipos = new SelectList(Tipos, "TipoId", "Nombre");
            }

            private byte[] GetReportBytes(string reportPath, DataSet reportDataSource, decimal pageWidth = 13.38m, decimal pageHeight = 8.5m, decimal MarginLeft = 1m, decimal MarginRight = 1m)
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

        // GET: Maestro
        [Permiso("Colegio.Maestro.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Maestro(a)", "Listado");

            List<Maestro> Maestros = new List<Maestro>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Maestros = new MaestroBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Maestros = new MaestroBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Maestros.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Maestro_Cursos_Asignados.Ver_Listado")]
        public ActionResult Curso(int? page)
        {
            CustomHelper.setTitulo("Maestro(a)", "Cursos Asignados");

            List<CursoMaestroModel> Cursos = new List<CursoMaestroModel>();

            try
            {
                Cursos = new MaestroBL().ObtenerCursosAsignadoxID(CustomHelper.getColegioId(), CustomHelper.getID(), 0);
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }           

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(Cursos.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Maestro_Cursos_Asignados.Ver_Listado")]
        public ActionResult Curso_Actitudinal(int? page)
        {
            CustomHelper.setTitulo("Maestro(a)", "Cursos Actitudinal Asignados");

            List<CursoMaestroModel> Cursos = new List<CursoMaestroModel>();

            try
            {
                Cursos = new MaestroBL().ObtenerCursosAsignadoxID(CustomHelper.getColegioId(), CustomHelper.getID(), 1);
            }
            catch (Exception ex)
            {
                ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                return View("~/Views/Shared/Error.cshtml");
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(Cursos.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Maestro_Grado.Ver_Nota")]
        public ActionResult Nota_x_Grado(int? page)
        {
            CustomHelper.setTitulo("Grado", "Notas");

            List<GradoxCicloModel> Grados = new List<GradoxCicloModel>();

            try
            {
                Grados = new MaestroBL().ObtenerGradoxId(CustomHelper.getColegioId(), CustomHelper.getID());
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

        [Permiso("Colegio.Maestro_Grado.Ver_Nota")]
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

        [Permiso("Colegio.Maestro.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Maestro(a)", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Maestro.Crear")]
        public ActionResult Crear(Maestro modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new MaestroBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Maestro-Success"] = strMensaje;
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

        [Permiso("Colegio.Maestro.Editar")]
        public ActionResult Editar(long id)
        {
            Maestro MaestroActual = new MaestroBL().ObtenerxId(id, false, false);

            if (MaestroActual == null || MaestroActual.MaestroId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = MaestroActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = MaestroActual.Activo == false ? strAtributo : "";

            this.CargaControles();
            return View(MaestroActual);
        }

        [HttpPost]
        [Permiso("Colegio.Maestro.Editar")]
        public ActionResult Editar(Maestro modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new MaestroBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Maestro-Success"] = strMensaje;
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

        [Permiso("Colegio.Maestro.Detalle")]
        public ActionResult Detalle(long id)
        {
            Maestro MaestroActual = new MaestroBL().ObtenerxId(id, true, true);

            if (MaestroActual == null || MaestroActual.MaestroId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Detalle");
            
            return View(MaestroActual);
        }

        [Permiso("Colegio.Maestro.Asignar_Curso")]
        public ActionResult Asignar_Curso(long id)
        {
            Maestro MaestroActual = new MaestroBL().ObtenerxId(id, true, false);

            if (MaestroActual == null || MaestroActual.MaestroId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Asignar Curso");

            if (MaestroActual.Cursos != null && MaestroActual.Cursos.Count() > 0)
            {
                List<string> NombreGrado = MaestroActual.Cursos.Select(x => x.Grado.Nombre).ToList();
                List<string> NombreJornada = MaestroActual.Cursos.Select(x => x.Grado.Jornada.Nombre).ToList();
                List<string> Grados = new List<string>();

                for (int i = 0; i < NombreGrado.Count; i++)
                {
                    Grados.Add(string.Format("{0} - {1}", NombreGrado[i], NombreJornada[i]));
                }

                ViewBag.gradoIds = MaestroActual.Cursos.Select(x => x.GradoId).ToList();
                ViewBag.nombreGradoIds = Grados;
                ViewBag.seccionIds = MaestroActual.Cursos.Select(x => x.SeccionId).ToList();
                ViewBag.nombreSeccionIds = MaestroActual.Cursos.Select(x => x.Seccion.Nombre).ToList();
                ViewBag.cursoIds = MaestroActual.Cursos.Select(x => x.CursoId).ToList();
                ViewBag.nombreCursoIds = MaestroActual.Cursos.Select(x => x.Curso.Nombre).ToList();
            }
            else
            {
                ViewBag.gradoIds = 0;
                ViewBag.nombreGradoIds = "";
                ViewBag.seccionIds = 0;
                ViewBag.nombreSeccionIds = "";
                ViewBag.cursoIds = 0;
                ViewBag.nombreCursoIds = "";
            }

            this.CargaNiveles();
            this.CargaSecciones();
            return View(MaestroActual);
        }

        [HttpPost]
        [Permiso("Colegio.Maestro.Asignar_Curso")]
        public ActionResult Asignar_Curso(Maestro modelo, long[] gradoIds, string[] nombreGradoIds, long[] seccionIds, string[] nombreSeccionIds, long[] cursoIds, string[] nombreCursoIds)
        {
            if (gradoIds == null || gradoIds.Length == 0)
            {
                ModelState.AddModelError("", "Se le informa que debe de asignar un curso al maestro(a)");
            }
            else
            {
                modelo.Cursos = new List<MaestroCurso>();
                for (int i = 0; i < gradoIds.Length; i++)
                {
                    MaestroCurso Curso = new MaestroCurso();
                    Curso.GradoId = gradoIds[i];
                    Curso.SeccionId = seccionIds[i];
                    Curso.CursoId = cursoIds[i];

                    modelo.Cursos.Add(Curso);
                }
            }

            if (ModelState.IsValid)
            {
                string strMensaje = new MaestroBL().GuardarAsignacionCurso(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Maestro_Curso-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            ViewBag.gradoIds = gradoIds;
            ViewBag.nombreGradoIds = nombreGradoIds;
            ViewBag.seccionIds = seccionIds;
            ViewBag.nombreSeccionIds = nombreSeccionIds;
            ViewBag.cursoIds = cursoIds;
            ViewBag.nombreCursoIds = nombreCursoIds;


            this.CargaNiveles();
            this.CargaSecciones();
            return View(new MaestroBL().ObtenerxId(modelo.MaestroId, true, false));
        }

        [Permiso("Colegio.Maestro.Actividad")]
        public ActionResult Actividad(string id)
        {
            CursoMaestroModel CursoActual = new MaestroBL().ObtenerCursoAsignadoxId(id, CustomHelper.getColegioId(), true, false, false);

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Actividad");

            this.CargaUnidades();
            this.CargaTipos();
            return View(CursoActual);
        }

        [Permiso("Colegio.Maestro.Eliminar_Actividad")]
        public ActionResult Eliminar_Actividad(long id)
        {
            Actividad ActividadActual = new ActividadBL().ObtenerxId(id);

            if (ActividadActual == null || ActividadActual.ActividadId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Eliminar Actividad");
            
            return View(ActividadActual);
        }

        [HttpPost]
        [Permiso("Colegio.Maestro.Eliminar_Actividad")]
        public ActionResult Eliminar_Actividad(Actividad modelo)
        {
            if (ModelState.IsValid)
            {
                string strMensaje = new ActividadBL().Eliminar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Maestro_Eliminar_Actividad-Success"] = strMensaje;
                    return RedirectToAction("Curso");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }
            
            return View(new ActividadBL().ObtenerxId(modelo.ActividadId));
        }

        [Permiso("Colegio.Maestro.Nota")]
        public ActionResult Nota(string id)
        {
            CursoMaestroModel CursoActual = new MaestroBL().ObtenerCursoAsignadoxId(id, CustomHelper.getColegioId(), false, true, false);

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Notas");
            
            return View(CursoActual);
        }

        [Permiso("Colegio.Maestro.Cuadro_x_Curso")]
        public ActionResult Cuadro_x_Curso(string id)
        {
            CursoMaestroModel CursoActual = new MaestroBL().ObtenerCursoAsignadoxId(id, CustomHelper.getColegioId(), false, true, false);

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Cuadro De Actividades Por Curso");
            
            return View(CursoActual);
        }

        [Permiso("Colegio.Maestro.Asistencia_x_Curso")]
        public ActionResult Asistencia_x_Curso(string id)
        {
            CursoMaestroModel CursoActual = new MaestroBL().ObtenerCursoAsignadoxId(id, CustomHelper.getColegioId(), false, false, true);

            if (CursoActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Maestro(a)", "Asistencia Por Curso");

            return View(CursoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Maestro.Asistencia_x_Curso")]
        public ActionResult Asistencia_x_Curso(CursoMaestroModel modelo, long[] alumnoId, int[] asistencia, string[] comentario)
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

            string strMensaje = new GradoBL().GuardarAsistencia(Asistencias, modelo.ColegioId, modelo.CursoId, modelo.GradoId, modelo.SeccionId, CustomHelper.getUsuarioId(), DateTime.Today);

            if (strMensaje.Equals("OK"))
            {
                TempData["Maestro_Asistencia-Success"] = strMensaje;
                return RedirectToAction("Asistencia_x_Curso", new { id = modelo.CursoMaestroId.ToString() });
            }
            else
            {
                ModelState.AddModelError("", strMensaje);
            }

            return View(new MaestroBL().ObtenerCursoAsignadoxId(modelo.CursoMaestroId.ToString(), CustomHelper.getColegioId(), false, false, true));
        }

        [Permiso("Colegio.Maestro_Grado.Ver_Nota")]
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

        [Permiso("Colegio.Maestro.Boleta_x_Asistencia")]
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
                var bytes = GetReportBytes(reportPath, Notas, 8.5m, 11.0m, 0m, 0m);

                return File(bytes, "application/pdf");
            }

            return View();
        }

        [HttpPost]
        [ActionName("NuevaActividad")]
        public JsonResult NuevaActividad(Actividad modelo)
        {
            modelo.ResponsableId = CustomHelper.getUsuarioId();
            string Mensaje = new ActividadBL().Guardar(modelo);

            if (Mensaje.Equals("OK"))
            {
                return Json(new { Operacion = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Operacion = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GuardarNota")]
        public JsonResult GuardarNota(ActividadNotaModel modelo)
        {         
            string Mensaje = new ActividadBL().GuardarNota(modelo, CustomHelper.getUsuarioId());

            if (Mensaje.Equals("OK"))
            {
                return Json(new { Operacion = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Operacion = false }, JsonRequestBehavior.AllowGet);
        }
    }
}