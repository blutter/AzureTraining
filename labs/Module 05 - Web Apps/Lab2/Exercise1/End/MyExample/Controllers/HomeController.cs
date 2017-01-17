using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View((object)ConfigurationManager.AppSettings["SLOT_SETTING"]);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}