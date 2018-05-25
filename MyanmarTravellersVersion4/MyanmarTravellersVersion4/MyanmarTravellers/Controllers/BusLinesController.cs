using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyanmarTravellers.Models;
//Created by Thurein Myint Oo
//For Managing BusLines
namespace MyanmarTravellers.Controllers
{
    public class BusLinesController : Controller
    {
        private MMTravellersEntities db = new MMTravellersEntities();

        // GET: BusLines
        [Authorize]        
        public ActionResult Index()
        {
            return View(db.BusLines.ToList());
        }

        // GET: BusLines/Details/5
        [Authorize]        
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusLine busLine = db.BusLines.Find(id);
            if (busLine == null)
            {
                return HttpNotFound();
            }
            return View(busLine);
        }

        // GET: BusLines/Create
        [Authorize]        
        public ActionResult Create()
        {
            return View();
        }

        // POST: BusLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]        
        public ActionResult Create([Bind(Include = "name,address,phone,email")] BusLine busLine)
        {
            if (ModelState.IsValid)
            {
                busLine.created_at = DateTime.Now;
                busLine.updated_at = DateTime.Now;
                db.BusLines.Add(busLine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busLine);
        }

        // GET: BusLines/Edit/5
        [Authorize]        
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusLine busLine = db.BusLines.Find(id);
            if (busLine == null)
            {
                return HttpNotFound();
            }
            return View(busLine);
        }

        // POST: BusLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,address,phone,email")] BusLine busLine)
        {
            if (ModelState.IsValid)
            {
                busLine.updated_at = DateTime.Now;
                db.Entry(busLine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busLine);
        }

        // GET: BusLines/Delete/5
        [Authorize]        
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusLine busLine = db.BusLines.Find(id);
            if (busLine == null)
            {
                return HttpNotFound();
            }
            return View(busLine);
        }

        // POST: BusLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]        
        public ActionResult DeleteConfirmed(long id)
        {
            BusLine busLine = db.BusLines.Find(id);
            db.BusLines.Remove(busLine);
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
    }
}
