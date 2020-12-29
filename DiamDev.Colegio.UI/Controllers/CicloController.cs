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
    public class CicloController : Controller
    {
        // GET: Ciclo
        [Permiso("Colegio.Ciclo_Escolar.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Ciclo", "Listado");

            List<Ciclo> Ciclos = new List<Ciclo>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Ciclos = new CicloBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Ciclos = new CicloBL().ObtenerListado(true, CustomHelper.getColegioId());
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
            return View(Ciclos.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Ciclo_Escolar.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Ciclo", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";
            
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Ciclo_Escolar.Crear")]
        public ActionResult Crear(Ciclo modelo, bool activo)
        {
            if (ModelState.IsValid)
            {
                modelo.ColegioId = CustomHelper.getColegioId();
                modelo.Activo = activo;               

                string strMensaje = new CicloBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Ciclo-Success"] = strMensaje;
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

        [Permiso("Colegio.Ciclo_Escolar.Editar")]
        public ActionResult Editar(long id)
        {
            Ciclo CicloActual = new CicloBL().ObtenerxId(id);

            if (CicloActual == null || CicloActual.CicloId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Ciclo", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = CicloActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = CicloActual.Activo == false ? strAtributo : "";
            
            return View(CicloActual);
        }

        [HttpPost]
        [Permiso("Colegio.Ciclo_Escolar.Editar")]
        public ActionResult Editar(Ciclo modelo, bool activo)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new CicloBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Ciclo-Success"] = strMensaje;
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