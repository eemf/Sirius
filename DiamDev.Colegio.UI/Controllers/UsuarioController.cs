using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class UsuarioController : Controller
    {
        #region Metodos Privados

            private void CargaControles()
            {                
                var Colegios = new ColegioBL().ObtenerListado(false);
                var Roles = new RolBL().ObtenerxResponsable(CustomHelper.getUsuarioId());

                ViewBag.Colegios = new SelectList(Colegios, "ColegioId", "Nombre");
                ViewBag.Roles = new SelectList(Roles, "RolId", "Nombre");
            }

        #endregion

        // GET: Usuario
        [Permiso("Colegio.Usuario.Ver_Listado")]
        public ActionResult Index(int? page, string search)
        {
            CustomHelper.setTitulo("Usuario", "Listado");

            List<Usuario> Usuarios = new List<Usuario>();

            try
            {
                if (!string.IsNullOrWhiteSpace(search) && search != null)
                {
                    Usuarios = new UsuarioBL().Buscar(search, CustomHelper.getColegioId()).ToList();
                }
                else
                {
                    Usuarios = new UsuarioBL().ObtenerListado(CustomHelper.getColegioId());
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
            return View(Usuarios.ToPagedList(pageNumber, pageSize));
        }

        [Permiso("Colegio.Usuario.Crear")]
        public ActionResult Crear()
        {
            CustomHelper.setTitulo("Usuario", "Nuevo");

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = strAtributo;
            ViewBag.ActivoNo = "";

            ViewBag.ReiniciarPasswordSi = "";
            ViewBag.ReiniciarPasswordNo = strAtributo;        

            this.CargaControles();
            return View();
        }

        [HttpPost]
        [Permiso("Colegio.Usuario.Crear")]
        public ActionResult Crear(Usuario modelo, bool activo, bool reiniciarPassword)
        {
            if (ModelState.IsValid)
            {
                if (CustomHelper.getUsuarioId() == 20200928001)
                {
                    modelo.Administrador = true;
                }

                modelo.Activo = activo;
                modelo.ReiniciarPassword = reiniciarPassword;

                string strMensaje = new UsuarioBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Usuario-Success"] = strMensaje;
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

            ViewBag.ReiniciarPasswordSi = reiniciarPassword == true ? strAtributo : "";
            ViewBag.ReiniciarPasswordNo = reiniciarPassword == false ? strAtributo : "";        

            this.CargaControles();
            return View(modelo);
        }

        [Permiso("Colegio.Usuario.Editar")]
        public ActionResult Editar(long id)
        {
            Usuario UsuarioActual = new UsuarioBL().ObtenerxId(id, true);

            if (UsuarioActual == null || UsuarioActual.UsuarioId == 0)
            {
                return HttpNotFound();
            }

            CustomHelper.setTitulo("Usuario", "Editar");          

            string strAtributo = "checked='checked'";

            ViewBag.ActivoSi = UsuarioActual.Activo == true ? strAtributo : "";
            ViewBag.ActivoNo = UsuarioActual.Activo == false ? strAtributo : "";

            ViewBag.ReiniciarPasswordSi = UsuarioActual.ReiniciarPassword == true ? strAtributo : "";
            ViewBag.ReiniciarPasswordNo = UsuarioActual.ReiniciarPassword == false ? strAtributo : "";        

            this.CargaControles();
            return View(UsuarioActual);
        }

        [HttpPost]
        [Permiso("Colegio.Usuario.Editar")]
        public ActionResult Editar(Usuario modelo, bool activo, bool reiniciarPassword)
        {
            if (ModelState.IsValid)
            {  
                modelo.Activo = activo;
                modelo.ReiniciarPassword = reiniciarPassword;

                string strMensaje = new UsuarioBL().Guardar(modelo);

                if (strMensaje.Equals("OK"))
                {
                    TempData["Usuario-Success"] = strMensaje;
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

            ViewBag.ReiniciarPasswordSi = reiniciarPassword == true ? strAtributo : "";
            ViewBag.ReiniciarPasswordNo = reiniciarPassword == false ? strAtributo : "";         

            this.CargaControles();
            return View(modelo);
        }
    }
}