using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradingPlatform1.Models;

namespace TradingPlatform1.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(Client model)
        {
            using (TradeContext db = new TradeContext())
            {
                var clientDetails = db.Clients.Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefault();
                if (clientDetails == null)
                {

                    return View("Index", model);
                }
                else
                {
                    Session["Username"] = clientDetails.Username;
                    Session["clientID"] = clientDetails.ClientID;
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}