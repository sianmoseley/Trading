using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradingPlatform1.Models;
using System.Data;
using System.Data.Entity;
using System.Net;
using Rotativa;


namespace TradingPlatform1.Controllers
{
    public class TradeController : Controller
    {
        private TradeContext db = new TradeContext();

        // GET: Trade
        public ActionResult Index()
        {
            return View(db.Trades.ToList());
        }

        // GET: Trade/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = db.Trades.Find(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        // POST: Trade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trade trade = db.Trades.Find(id);
            db.Trades.Remove(trade);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = db.Trades.Find(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        [HttpPost, ActionName("Update")]
        public ActionResult Update(int id)
        {
            Trade trade = db.Trades.Find(id);

            //update status
            trade.Status = "Processed";
            //update processed date
            trade.ProcessedDate = DateTime.Now;

            //DateTime pDateAndTime = DateTime.Now;
            //trade.ProcessedDate = pDateAndTime.ToString("yyyy-MM-dd");
            //alert database of statechange
            db.Entry(trade).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.success = "Trade successfully processed!";
            return View(trade);
        }

        public ActionResult ViewNote(int id)
        {
            Trade trade = db.Trades.Find(id);
            return View(trade);
        }

        public ActionResult DownloadPDF(int id)
        {
            //pdf creation
            var pdf = new ActionAsPdf("ViewNote", new { id = id });
            return pdf;
        }

        public ActionResult Dashboard(string tradetype, string sortOrder)
        {

            //query database for processed trades
            var data = db.Trades.Where(model => model.Status == "Processed");

            //order by date
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            switch (sortOrder)
            {
                case "Date":
                    data = data.OrderBy(model => model.ProcessedDate);
                    break;
                case "date_desc":
                    data = data.OrderByDescending(model => model.ProcessedDate);
                    break;
                default: //standard 
                    data = db.Trades.Where(model => model.Status == "Processed");
                    break;
            }

            //set drop down list - needs work!
            ViewBag.TradeType = (from r in db.Trades select r.TradeType).Distinct();

            if (!String.IsNullOrEmpty(tradetype))
            {
                data = data.Where(model => model.TradeType.Contains(tradetype));
            }

            ViewBag.Count = data.ToList().Count();
            ViewBag.Price = data.Sum(model => model.Price);

            return View(data.ToList());
        }


    }
}