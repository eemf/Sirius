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
    public class SeccionController : Controller
    {
        // GET: Seccion
        [Permiso("Colegio.Seccion.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Sección", "Listado");

            List<Seccion> Secciones = new List<Seccion>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Secciones = new SeccionBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Secciones = new SeccionBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Secciones.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Seccion.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Sección", "Nueva");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";
            
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Seccion.Crear")]
        public ActionResult Crear(Seccion modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new SeccionBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Seccion-Success"] = strMensaje;
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

            return View(modelo);
        }

        [Permiso("Colegio.Seccion.Editar")]
        public ActionResult Editar(long id)
        {
            Seccion SeccionActual = new SeccionBL().ObtenerxId(id);

            if (SeccionActual == null || SeccionActual.SeccionId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Sección", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = SeccionActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = SeccionActual.Activo == false ? strAtributo : "";
            
            return View(SeccionActual);
        }

        [HttpPost]
        [Permiso("Colegio.Seccion.Editar")]
        public ActionResult Editar(Seccion modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new SeccionBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Seccion-Success"] = strMensaje;
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
            
            return View(modelo);
        }
    }
}