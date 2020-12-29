using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class ColegioController : Controller
    {

        // GET: Colegio
        [Permiso("Colegio.Colegio.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Colegio", "Listado");

            List<Entities.Colegio> Colegios = new List<Entities.Colegio>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Colegios = new ColegioBL().Buscar(search).ToList();
                }
                else
                {
                    Colegios = new ColegioBL().ObtenerListado(true);
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
            return View(Colegios.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Colegio.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Colegio", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";
            
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Colegio.Crear")]
        public ActionResult Crear(Entities.Colegio modelo, HttpPostedFileBase logoApp, bool activo)
        {
            if (logoApp != null)
            {
                modelo.Fotografia = new ColegioLogo();
                if (logoApp != null)
                {
                    byte[] FileData = new byte[logoApp.ContentLength + 1];
                    logoApp.InputStream.Read(FileData, 0, logoApp.ContentLength);
                                       
                    modelo.Fotografia = new ColegioLogo() { Nombre = logoApp.FileName, Content = FileData, ContentType = logoApp.ContentType, Length = logoApp.ContentLength };
                }
            }

            if (ModelState.IsValid)
            {
                modelo.Activo = activo;               

                string strMensaje = new ColegioBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Colegio-Success"] = strMensaje;
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

        [Permiso("Colegio.Colegio.Editar")]
        public ActionResult Editar(long id)
        {
            Entities.Colegio ColegioActual = new ColegioBL().ObtenerxId(id);

            if (ColegioActual == null || ColegioActual.ColegioId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Colegio", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = ColegioActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = ColegioActual.Activo == false ? strAtributo : "";
            
            return View(ColegioActual);
        }

        [HttpPost]
        [Permiso("Colegio.Colegio.Editar")]
        public ActionResult Editar(Entities.Colegio modelo, HttpPostedFileBase logoApp, bool activo)
        {
            if (logoApp != null)
            {
                modelo.Fotografia = new ColegioLogo();
                if (logoApp != null)
                {
                    byte[] FileData = new byte[logoApp.ContentLength + 1];
                    logoApp.InputStream.Read(FileData, 0, logoApp.ContentLength);

                    modelo.Fotografia = new ColegioLogo() { Nombre = logoApp.FileName, Content = FileData, ContentType = logoApp.ContentType, Length = logoApp.ContentLength };
                }
            }

            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                
                string strMensaje = new ColegioBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Colegio-Success"] = strMensaje;
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