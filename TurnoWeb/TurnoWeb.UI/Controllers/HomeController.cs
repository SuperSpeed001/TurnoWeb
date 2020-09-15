namespace TurnoWeb.UI.Controllers
{
 
    using System.Web.Mvc;
    using TurnosWeb.UI;

    public class HomeController : Controller
    {
        public ActionResult _TesterDc()
        {
           
            return View("Index");
        }

    }
}