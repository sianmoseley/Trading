using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradingPlatform1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var clientID = Session["clientID"];
            return View();
        }

    }
}