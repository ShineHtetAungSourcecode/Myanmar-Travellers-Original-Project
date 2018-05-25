using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyanmarTravellers.Models;
//Created by Shine Htet Aung and Htut Arkar Oo
//For managing Locations
namespace MyanmarTravellers.Controllers
{
    public class LocationsController : Controller
    {
        private MMTravellersEntities db = new MMTravellersEntities();

        // GET: Locations
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Locations.ToList());
        }

        // GET: Locations/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: Locations/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "id,name")] Location location)
        {
            if (ModelState.IsValid)
            {
                try
                {

                location.id = location.id.ToUpper();
                location.created_at = DateTime.Now;
                location.updated_at = DateTime.Now;
                db.Locations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
                } catch(Exception e)
                {
                    this.ModelState.AddModelError("", "Item with such name already exist");
                }
            }

            return View(location);
        }

        // GET: Locations/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "id,name")] Location location)
        {
            if (ModelState.IsValid)
            {
                try
                {

                location.id = location.id.ToUpper();
                location.updated_at = DateTime.Now;
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
                } catch (Exception e)
                {
                    this.ModelState.AddModelError("", "Item with such name already exist");
                }
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(string id)
        {
            Location location = db.Locations.Find(id);
            try
            {
                db.Locations.Remove(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            } catch(Exception e )
            {
                this.ModelState.AddModelError("", "This city can't be deleted because other entities depends on this city.");


            }
            return View(location);
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
