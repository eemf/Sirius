﻿using DiamDev.Colegio.BLL;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DiamDev.Colegio.UI.App_Start
{
    public class PermisoAttribute : AuthorizeAttribute
    {
        public string Permiso { get; set; }

        public PermisoAttribute()
        { }

        public PermisoAttribute(string Permiso)
        {
            this.Permiso = Permiso;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            IPrincipal user = httpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            return new RolBL().AutorizacionPermisoxUsuario(user.Identity.Name, this.Permiso);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Seguridad", action = "NoAccess" }));
        }
    }
}