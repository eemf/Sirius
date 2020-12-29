using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DiamDev.Colegio.BLL;
using DiamDev.Colegio.Entities;
using DiamDev.Colegio.UI.App_Start;
using PagedList;

namespace DiamDev.Colegio.UI.Controllers
{
    [Authorize]
    [Seguridad]
    [HandleError]
    public class AlumnoController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {
                var Generos = new GeneroBL().ObtenerListado();
                var Estados = new EstadoCivilBL().ObtenerListado();

                ViewBag.Generos = new SelectList(Generos, "GeneroId", "Nombre");
                ViewBag.Estados = new SelectList(Estados, "EstadoId", "Nombre");
            }

        #endregion

        // GET: Alumno
        [Permiso("Colegio.Alumno.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Alumno(a)", "Listado");

            List<Alumno> Alumnos = new List<Alumno>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Alumnos = new AlumnoBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Alumnos = new AlumnoBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Alumnos.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Alumno.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Alumno(a)", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Alumno.Crear")]
        public ActionResult Crear(Alumno modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new AlumnoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Alumno-Success"] = strMensaje;
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

        [Permiso("Colegio.Alumno.Editar")]
        public ActionResult Editar(long id)
        {
            Alumno AlumnoActual = new AlumnoBL().ObtenerxId(id, false, false, false, false);

            if (AlumnoActual == null || AlumnoActual.AlumnoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Alumno(a)", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = AlumnoActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = AlumnoActual.Activo == false ? strAtributo : "";

            this.CargaControles();
            return View(AlumnoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Alumno.Editar")]
        public ActionResult Editar(Alumno modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new AlumnoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Alumno-Success"] = strMensaje;
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

        [Permiso("Colegio.Alumno.Detalle")]
        public ActionResult Detalle(long id)
        {
            Alumno AlumnoActual = new AlumnoBL().ObtenerxId(id, true, true, true, true);

            if (AlumnoActual == null || AlumnoActual.AlumnoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Alumno(a)", "Detalle");

            return View(AlumnoActual);
        }

        [HttpPost]
        public JsonResult ConsultaAlumnoAutocomplementar(string search)
        {
            return Json(new AlumnoBL().BuscarAlumnoxAutocompletar(search, CustomHelper.getColegioId()), JsonRequestBehavior.AllowGet);
        }

        [ActionName("ObtenerAlumnoxTextoLibre")]
        public JsonResult ObtenerAlumnoxTextoLibre(string search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                List<Alumno> Alumnos = new AlumnoBL().BuscarAlumnoxTextoLibre(search, CustomHelper.getColegioId());
                if (Alumnos != null && Alumnos.Count() > 0)
                {
                    return Json(new { Operacion = true, Data = Alumnos }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Operacion = false }, JsonRequestBehavior.AllowGet);
        }
    }
}