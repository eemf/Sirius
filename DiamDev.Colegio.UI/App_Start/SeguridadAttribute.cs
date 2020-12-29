using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DiamDev.Colegio.UI.App_Start
{
    public class SeguridadAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["Nombre"] == null || HttpContext.Current.Session["Usuario"] == null || HttpContext.Current.Session["Encabezado"] == null || HttpContext.Current.Session["SubEncabezado"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Seguridad" }, { "Action", "Login" } });
            }

            base.OnActionExecuting(filterContext);
        }
    }
}