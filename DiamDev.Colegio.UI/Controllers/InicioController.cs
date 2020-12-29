using DiamDev.Colegio.UI.App_Start;
using System.Web.Mvc;

namespace DiamDev.Colegio.UI.Controllers
{
    [HandleError]
    public class InicioController : Controller
    {
        // GET: Inicio
        [Authorize]
        public ActionResult Dashboard()
        {
            CustomHelper.setTitulo("Dashboard", "Inicio");
            return View();
        }
    }
}