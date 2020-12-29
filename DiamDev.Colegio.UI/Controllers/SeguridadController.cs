using DiamDev.Colegio.BLL;
using DiamDev.Colegio.Entities;
using DiamDev.Colegio.UI.App_Start;
using DiamDev.Colegio.UI.Models;
using Sistema.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DiamDev.Colegio.UI.Controllers
{
    [Authorize]
    [HandleError]
    public class SeguridadController : Controller
    {
        // GET: Seguridad
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Token = string.Concat(model.Usuario, model.Password, model.Usuario);
                    string Key = Criptografia.Base64StringAHexString(Criptografia.EncriptarSha512(Token));
                    string Mensaje = new UsuarioBL().ValidarUsuario(model.Usuario, Key);

                    if (Mensaje.Equals("OK"))
                    {
                        Usuario UsuarioActual = new UsuarioBL().ObtenerxLogin(model.Usuario, true);
                        FormsAuthentication.SetAuthCookie(model.Usuario, true);

                        CustomHelper.setUsuario(model.Usuario);
                        CustomHelper.setColegio(UsuarioActual.Colegio);

                        if (!UsuarioActual.ReiniciarPassword)
                        {
                            return RedirectToAction("Dashboard", "Inicio");
                        }
                        else
                        {
                            return RedirectToAction("ReiniciarPassword", new { id = UsuarioActual.UsuarioId });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = string.Format("Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                    return View("~/Views/Shared/Error.cshtml");
                }
            }

            ModelState.AddModelError("", "El usuario o la clave son incorrectos.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Seguridad");
        }

        public ActionResult Menu()
        {
            return PartialView("~/Views/Shared/_Menu.cshtml", new MenuBL().ObtenerMenuxUsuario(System.Web.HttpContext.Current.User.Identity.Name));
        }

        public ActionResult NoAccess()
        {
            return View();
        }

        public ActionResult ReiniciarPassword(long id)
        {
            Usuario UsuarioActual = new UsuarioBL().ObtenerxId(id, false);

            if (UsuarioActual == null)
            {
                return HttpNotFound();
            }

            return View(new UsuarioModel() { UsuarioId = UsuarioActual.UsuarioId, Login = UsuarioActual.Login });
        }

        [HttpPost]
        public ActionResult ReiniciarPassword(UsuarioModel model)
        {
            if (ModelState.IsValid)
            {
                string strMensaje = string.Empty;

                strMensaje = new UsuarioBL().ActualizarPassword(new Usuario() { UsuarioId = model.UsuarioId, Login = model.Login, Password = model.PasswordActual, NuevoPassword = model.PasswordNuevo, ConfirmarPassword = model.PasswordConfirmacion });

                if (strMensaje.Equals("OK"))
                {
                    return RedirectToAction("Dashboard", "Inicio");
                }
                else
                {
                    ModelState.AddModelError("", strMensaje);
                }
            }

            return View(model);
        }
    }
}