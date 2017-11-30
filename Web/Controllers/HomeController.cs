using System.Web.Mvc;
using Web.Filters.Web;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        [CompressFilter]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}