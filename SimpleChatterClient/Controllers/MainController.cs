using System.Web.Mvc;

namespace SimpleChatterClient.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}