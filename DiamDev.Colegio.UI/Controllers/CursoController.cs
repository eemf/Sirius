using System;
using System.Collections;
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
    public class CursoController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {
                var Tipos = new TipoPonderacionBL().ObtenerListado();
                var Niveles = new NivelAcademicoBL().ObtenerListado(false, CustomHelper.getColegioId());

                ViewBag.Tipos = new SelectList(Tipos, "TipoId", "Nombre");
                ViewBag.Niveles = new SelectList(Niveles, "NivelId", "Nombre");
            }

        #endregion

        // GET: Curso
        [Permiso("Colegio.Curso.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Curso", "Listado");

            List<Curso> Cursos = new List<Curso>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Cursos = new CursoBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Cursos = new CursoBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Cursos.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Curso.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Curso", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.MinisterialSi = strAtributo;
            ViewBag.MinisterialNo = "";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Curso.Crear")]
        public ActionResult Crear(Curso modelo, bool ministerial, bool activo, long[] gradoIds, string[] nombreGradoIds)
        {
            if (gradoIds == null || gradoIds.Length == 0)
            {
                ModelState.AddModelError("", "Se le informa que debe de asignar un grado al curso");
            }
            else
            {
                modelo.Grados = new List<CursoGrado>();
                for (int i = 0; i < gradoIds.Length; i++)
                {
                    CursoGrado Grado = new CursoGrado();
                    Grado.GradoId = gradoIds[i];                

                    modelo.Grados.Add(Grado);
                }
            }

            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Ministerial = ministerial;
                modelo.Activo = activo;               

                string strMensaje = new CursoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Curso-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            string strAtributo = "checked='checked'";

            ViewBag.MinisterialSi = ministerial == true ? strAtributo : "";
            ViewBag.MinisterialNo = ministerial == false ? strAtributo : "";

            ViewBag.ActivoSi = activo == true ? strAtributo : "";
            ViewBag.ActivoNo = activo == false ? strAtributo : "";

            ViewBag.gradoIds = gradoIds;
            ViewBag.nombreGradoIds = nombreGradoIds;

            this.CargaControles();
            return View(modelo);
        }

        [Permiso("Colegio.Curso.Editar")]
        public ActionResult Editar(long id)
        {
            Curso CursoActual = new CursoBL().ObtenerxId(id, true);

            if (CursoActual == null || CursoActual.CursoId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Curso", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.MinisterialSi = CursoActual.Ministerial == true ? strAtributo : "";
            ViewBag.MinisterialNo = CursoActual.Ministerial == false ? strAtributo : "";

            ViewBag.ActivoSi = CursoActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = CursoActual.Activo == false ? strAtributo : "";

            if (CursoActual.Grados != null && CursoActual.Grados.Count() > 0)
            {
                List<string> NombreGrado = CursoActual.Grados.Select(x => x.Grado.Nombre).ToList();
                List<string> NombreJornada = CursoActual.Grados.Select(x => x.Grado.Jornada.Nombre).ToList();                
                List<string> Grados = new List<string>();

                for (int i = 0; i < NombreGrado.Count; i++)
                {
                    Grados.Add(string.Format("{0} - {1}", NombreGrado[i], NombreJornada[i]));
                }

                ViewBag.gradoIds = CursoActual.Grados.Select(x => x.GradoId).ToList();
                ViewBag.nombreGradoIds = Grados;
            }
            else
            {
                ViewBag.gradoIds = 0;
                ViewBag.nombreGradoIds = "";
            }

            this.CargaControles();
            return View(CursoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Curso.Editar")]
        public ActionResult Editar(Curso modelo, bool ministerial, bool activo, long[] gradoIds, string[] nombreGradoIds)
        {
            if (gradoIds == null || gradoIds.Length == 0)
            {
                ModelState.AddModelError("", "Se le informa que debe de asignar un grado al curso");
            }
            else
            {
                modelo.Grados = new List<CursoGrado>();
                for (int i = 0; i < gradoIds.Length; i++)
                {
                    CursoGrado Grado = new CursoGrado();
                    Grado.GradoId = gradoIds[i];

                    modelo.Grados.Add(Grado);
                }
            }

            if (ModelState.IsValid)
            {
                modelo.Ministerial = ministerial;
                modelo.Activo = activo;

                string strMensaje = new CursoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Curso-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            string strAtributo = "checked='checked'";

            ViewBag.MinisterialSi = ministerial == true ? strAtributo : "";
            ViewBag.MinisterialNo = ministerial == false ? strAtributo : "";

            ViewBag.ActivoSi = activo == true ? strAtributo : "";
            ViewBag.ActivoNo = activo == false ? strAtributo : "";

            ViewBag.gradoIds = gradoIds;
            ViewBag.nombreGradoIds = nombreGradoIds;

            this.CargaControles();
            return View(modelo);
        }

        [ActionName("ObtenerCursoxGrado")]
        public JsonResult ObtenerCursoxGrado(long id)
        {
            IList _result = new List<SelectListItem>();
            _result = new CursoBL().ObtenerxGradoId(id, CustomHelper.getColegioId()).Select(m => new SelectListItem() { Text = m.Nombre, Value = m.CursoId.ToString() }).ToList();
            return Json(_result, JsonRequestBehavior.AllowGet);
        }
    }
}