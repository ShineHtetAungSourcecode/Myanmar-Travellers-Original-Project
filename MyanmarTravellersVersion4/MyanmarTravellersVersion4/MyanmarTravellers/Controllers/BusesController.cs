using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyanmarTravellers.Models;
//Cureated by Htut Arkar Oo and Thurein Myint Oo
//For managing buses
namespace MyanmarTravellers.Controllers
{
    public class BusesController : Controller
    {
        private MMTravellersEntities db = new MMTravellersEntities();

        // GET: Buses
        [Authorize]
        public ActionResult Index()
        {
            var buses = db.Buses.Include(b => b.BusLine);
            return View(buses.ToList());
        }

        // GET: Buses/Details/5
        [Authorize]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bus bus = db.Buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        // GET: Buses/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.busline_id = new SelectList(db.BusLines, "id", "name");
            return View();
        }

        // POST: Buses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "id,plate_no,seats_per_row,no_of_rows,busline_id")] Bus bus)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Step 1: Save the bus in the database
                        bus.created_at = DateTime.Now;
                        bus.updated_at = DateTime.Now;
                        db.Buses.Add(bus);
                        db.SaveChanges();

                        //Step 2: Creates the seats
                        var new_seats = this.MakeSeats(bus);
                        db.Seats.AddRange(new_seats);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {

                    }

                }

                return RedirectToAction("Index");
            }

            ViewBag.busline_id = new SelectList(db.BusLines, "id", "name", bus.busline_id);
            return View(bus);
        }

        // GET: Buses/Edit/5
        [Authorize]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bus bus = db.Buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            ViewBag.busline_id = new SelectList(db.BusLines, "id", "name", bus.busline_id);
            return View(bus);
        }

        // POST: Buses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "id,plate_no,seats_per_row,no_of_rows,busline_id")] Bus bus)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Step 1: Get the original bus instance from db.
                        var org_bus = db.Buses.AsNoTracking().Where(B => B.id == bus.id).FirstOrDefault();

                        //Step 2: Check if changes are made on Seat Per Row and No of Rows
                        if (org_bus.seats_per_row != bus.seats_per_row || org_bus.no_of_rows != bus.no_of_rows)
                        {
                            //Step 2.1: Removes the old seats.
                            db.Seats.RemoveRange(org_bus.Seats);
                            db.SaveChanges();

                            //Step 2.2: Creates the new seats from new bus data
                            var new_seats = this.MakeSeats(bus);
                            db.Seats.AddRange(new_seats);
                            db.SaveChanges();
                        }

                        //Step 3: Save the changes
                        bus.updated_at = DateTime.Now;
                        db.Entry(bus).State = EntityState.Modified;
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        ViewBag.busline_id = new SelectList(db.BusLines, "id", "name", bus.busline_id);
                        return View(bus);
                    }

                    return RedirectToAction("Index");
                }
            }
            ViewBag.busline_id = new SelectList(db.BusLines, "id", "name", bus.busline_id);
            return View(bus);
        }

        // GET: Buses/Delete/5
        [Authorize]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bus bus = db.Buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        // POST: Buses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(long id)
        {
            Bus bus = db.Buses.Find(id);
            try
            {
            db.Buses.Remove(bus);
            db.SaveChanges();
            return RedirectToAction("Index");
            } catch(Exception e)
            {
                this.ModelState.AddModelError("", "Cannot delete this bus since several entities depend on it.");
            }
            return View(bus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Returns a seat list
        //Seats no are created in the {row}{col}. E.g. 1-A, 2-C
        private List<Seat> MakeSeats(Bus bus)
        {
            int A = 65;
            var new_seats = new List<Seat>();
            for (int i = 1; i <= bus.no_of_rows; i++)
            {
                for (int ii = 0; ii < bus.seats_per_row; ii++)
                {
                    char seat_suffix = (char)(A + ii);
                    string seat_no = i.ToString() + "-" + seat_suffix;
                    Seat seat = new Seat
                    {
                        bus_id = bus.id,
                        seat_no = seat_no
                    };
                    new_seats.Add(seat);
                }
            }
            return new_seats;
        }
    }
}
