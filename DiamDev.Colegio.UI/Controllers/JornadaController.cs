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
    public class JornadaController : Controller
    {
        // GET: Jornada
        [Permiso("Colegio.Jornada.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Jornada", "Listado");

            List<Jornada> Jornadas = new List<Jornada>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Jornadas = new JornadaBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Jornadas = new JornadaBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Jornadas.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Jornada.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Jornada", "Nueva");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";
            
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Jornada.Crear")]
        public ActionResult Crear(Jornada modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new JornadaBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Jornada-Success"] = strMensaje;
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

        [Permiso("Colegio.Jornada.Editar")]
        public ActionResult Editar(long id)
        {
            Jornada JornadaActual = new JornadaBL().ObtenerxId(id);

            if (JornadaActual == null || JornadaActual.JornadaId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Jornada", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = JornadaActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = JornadaActual.Activo == false ? strAtributo : "";
            
            return View(JornadaActual);
        }

        [HttpPost]
        [Permiso("Colegio.Jornada.Editar")]
        public ActionResult Editar(Jornada modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new JornadaBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Jornada-Success"] = strMensaje;
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