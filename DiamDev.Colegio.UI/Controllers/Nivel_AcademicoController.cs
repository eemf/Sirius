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
    public class Nivel_AcademicoController : Controller
    {
        // GET: Nivel_Academico
        [Permiso("Colegio.Nivel_Academico.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Nivel Academico", "Listado");

            List<NivelAcademico> Niveles = new List<NivelAcademico>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Niveles = new NivelAcademicoBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Niveles = new NivelAcademicoBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Niveles.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Nivel_Academico.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Nivel Academico", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";
            
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Nivel_Academico.Crear")]
        public ActionResult Crear(NivelAcademico modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new NivelAcademicoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Nivel_Academico-Success"] = strMensaje;
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

        [Permiso("Colegio.Nivel_Academico.Editar")]
        public ActionResult Editar(long id)
        {
            NivelAcademico NivelAcademicoActual = new NivelAcademicoBL().ObtenerxId(id);

            if (NivelAcademicoActual == null || NivelAcademicoActual.NivelId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Nivel Academico", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = NivelAcademicoActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = NivelAcademicoActual.Activo == false ? strAtributo : "";
            
            return View(NivelAcademicoActual);
        }

        [HttpPost]
        [Permiso("Colegio.Nivel_Academico.Editar")]
        public ActionResult Editar(NivelAcademico modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new NivelAcademicoBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Nivel_Academico-Success"] = strMensaje;
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