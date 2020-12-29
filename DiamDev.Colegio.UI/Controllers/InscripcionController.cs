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
    public class InscripcionController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {                
                var Ciclos = new CicloBL().ObtenerListado(false, CustomHelper.getColegioId());
                var Niveles = new NivelAcademicoBL().ObtenerListado(false, CustomHelper.getColegioId());
                var Jornadas = new JornadaBL().ObtenerListado(false, CustomHelper.getColegioId());
                var Secciones = new SeccionBL().ObtenerListado(false, CustomHelper.getColegioId());
                         
                ViewBag.Ciclos = new SelectList(Ciclos, "CicloId", "Nombre");
                ViewBag.Niveles = new SelectList(Niveles, "NivelId", "Nombre");
                ViewBag.Jornadas = new SelectList(Jornadas, "JornadaId", "Nombre");
                ViewBag.Secciones = new SelectList(Secciones, "SeccionId", "Nombre");
            }

        #endregion

        // GET: Inscripcion
        [Permiso("Colegio.Inscripcion.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Inscripción", "Listado");

            List<Inscripcion> Inscripciones = new List<Inscripcion>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Inscripciones = new InscripcionBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Inscripciones = new InscripcionBL().ObtenerListado(CustomHelper.getColegioId());
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
            return View(Inscripciones.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Inscripcion.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Inscripción", "Nueva");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Inscripcion.Crear")]
        public ActionResult Crear(Inscripcion modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.ResponsableId = CustomHelper.getUsuarioId();
                modelo.Activo = activo;               

                string strMensaje = new InscripcionBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Inscripcion-Success"] = strMensaje;
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

        [Permiso("Colegio.Inscripcion.Eliminar")]
        public ActionResult Eliminar(long id)
        {
            Inscripcion InscripcionActual = new InscripcionBL().ObtenerxId(id, true);

            if (InscripcionActual == null || InscripcionActual.InscripcionId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Inscripción", "Editar");          
            
            return View(InscripcionActual);
        }

        [HttpPost]
        [Permiso("Colegio.Inscripcion.Eliminar")]
        public ActionResult Eliminar(Inscripcion modelo)
        {
            string strMensaje = new InscripcionBL().Eliminar(modelo);

            if (strMensaje.Equals("OK"))
            {
                TempData["Inscripcion_Eliminar-Success"] = strMensaje;
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", strMensaje);
            }

            return View(new InscripcionBL().ObtenerxId(modelo.InscripcionId, true));
        }
    }
}