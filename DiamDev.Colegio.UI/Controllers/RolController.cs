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
    public class RolController : Controller
    {
        #region Metodos Privados

            private List<Permiso> Permisos()
            {
                return new PermisoBL().ObtenerListado();
            }

        #endregion

        // GET: Rol
        [Permiso("Colegio.Rol.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Rol", "Listado");

            List<Rol> Roles = new List<Rol>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Roles = new RolBL().Buscar(search);
                }
                else
                {
                    Roles = new RolBL().ObtenerListado().ToList();
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
            return View(Roles.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Rol.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Rol", "Nuevo");

            return View(new Rol() { PermisoIds = Permisos() });
        }

        [HttpPost]
        [Permiso("Colegio.Rol.Crear")]
        public ActionResult Crear(Rol modelo, string[] permisosIds)
        {
            if (permisosIds == null || permisosIds.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un permiso");
            }

            modelo.Permisos = new List<RolPermiso>();
            for (int i = 0; i < permisosIds.Length; i++)
            {
                RolPermiso Permiso = new RolPermiso();
                Permiso.PermisoId = permisosIds[i];
                modelo.Permisos.Add(Permiso);
            }

            if (ModelState.IsValid)
            {
                string strMensaje = new RolBL().Guardar(modelo);

                if (strMensaje.Contains("OK"))
                {
                    TempData["Rol-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            modelo.PermisoIds = Permisos();
            return View(modelo);
        }

        [Permiso("Colegio.Rol.Editar")]
        public ActionResult Editar(long id)
        {
            Rol RolActual = new RolBL().ObtenerxId(id);

            if (RolActual == null)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Rol", "Editar");

            RolActual.PermisoIds = Permisos();
            return View(RolActual);
        }

        [HttpPost]
        [Permiso("Colegio.Rol.Editar")]
        public ActionResult Editar(Rol modelo, string[] permisosIds)
        {
            if (permisosIds == null || permisosIds.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un permiso");
            }

            modelo.Permisos = new List<RolPermiso>();
            for (int i = 0; i < permisosIds.Length; i++)
            {
                RolPermiso Permiso = new RolPermiso();
                Permiso.PermisoId = permisosIds[i];
                modelo.Permisos.Add(Permiso);
            }

            if (ModelState.IsValid)
            {
                string strMensaje = new RolBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Rol-Success"] = strMensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            modelo.PermisoIds = Permisos();
            return View(modelo);
        }
    }
}