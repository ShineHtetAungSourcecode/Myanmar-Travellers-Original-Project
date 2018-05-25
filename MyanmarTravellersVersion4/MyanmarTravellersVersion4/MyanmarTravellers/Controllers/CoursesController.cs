using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyanmarTravellers.Models;
//Created by Thurein Myint Oo
//Modified by Htut Arkar Oo and Shine Htet Aung
//For managing Routes
namespace MyanmarTravellers.Controllers
{
    public class CoursesController : Controller
    {
        private MMTravellersEntities db = new MMTravellersEntities();
        private readonly string NEW_COURSE_DATE = "new_cousre_date";
        private readonly string NEW_COURSE_BUS_LINE = "new_course_bus_line";

        // GET: Courses
        [Authorize]
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Bus).Include(c => c.FromLocation).Include(c => c.ToLocation);
            return View(courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = db.Courses.Find(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }

            Bus bus = course.Bus;
            var rows = new List<List<Ticket>>();
            for (int i = 1; i <= bus.no_of_rows; i++)
            {
                var Tickets = course.Tickets
                    .Where(t => t.Seat.seat_no.StartsWith(i+"-"))
                    .OrderBy(t => t.Seat.seat_no)
                    .ToList();
                rows.Add(Tickets);
            }

            ViewBag.Rows = rows;
            var buslineid = course.Bus.busline_id;
            ViewBag.buslinename = db.BusLines.Find(buslineid).name;
            return View(course);
        }

        [Authorize]
        public ActionResult CreateStep1()
        {
            ViewBag.busline_id = new SelectList(db.BusLines, "id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateStep1([Bind(Include = "date,busline_id")] Course course)
        {
            Session[NEW_COURSE_DATE] = course.date;
            var busline = db.BusLines.Find(course.busline_id);
            Session[NEW_COURSE_BUS_LINE] = busline;
            return RedirectToAction("CreateStep2");
        }
        
        // GET: Courses/Create
        [Authorize]
        public ActionResult CreateStep2()
        {
            var busLine = Session[NEW_COURSE_BUS_LINE] as BusLine;
            var date = Convert.ToDateTime(Session[NEW_COURSE_DATE]);
            var assigned_buses = db.Courses
                .Where(c => c.date.Equals(date))
                .Where(c => c.Bus.busline_id == busLine.id)
                .Select(c => c.Bus)
                .ToList();
            var buses = db.Buses
                .Where(b => b.BusLine.id == busLine.id)
                .ToList()
                .Except(assigned_buses)
                .AsEnumerable()
                .ToList();
            ViewBag.bus_id = new SelectList(buses, "id", "plate_no");
            ViewBag.from_id = new SelectList(db.Locations, "id", "name");
            ViewBag.to_id = new SelectList(db.Locations, "id", "name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateStep2([Bind(Include = "bus_id,from_id,to_id,departure_time,fee_per_seat")] Course course)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Step 1: Saves the course
                        course.date = Convert.ToDateTime(Session[NEW_COURSE_DATE]);
                        course.created_at = DateTime.Now;
                        course.updated_at = DateTime.Now;
                        db.Courses.Add(course);
                        db.SaveChanges();

                        //Step 2: Clear the session data
                        Session[NEW_COURSE_DATE] = null;
                        Session[NEW_COURSE_BUS_LINE] = null;

                        //Step 3: Creates a series of tickets
                        db.Entry(course).Reference(c => c.Bus).Load();
                        var tickets = MakeTickets(course);
                        db.Tickets.AddRange(tickets);
                        db.SaveChanges();

                        //Step 4: Commit the transaction
                        transaction.Commit();
                        return RedirectToAction("Index");
                    } catch(Exception e)
                    {
                        ModelState.AddModelError("", "Error occured while creating new coursea and tickets");
                        transaction.Rollback();
                        ViewBag.bus_id = new SelectList(db.Buses, "id", "plate_no", course.bus_id);
                        ViewBag.from_id = new SelectList(db.Locations, "id", "name", course.from_id);
                        ViewBag.to_id = new SelectList(db.Locations, "id", "name", course.to_id);
                        return View(course);
                    }
                }
                    
            }

            ViewBag.bus_id = new SelectList(db.Buses, "id", "plate_no", course.bus_id);
            ViewBag.from_id = new SelectList(db.Locations, "id", "name", course.from_id);
            ViewBag.to_id = new SelectList(db.Locations, "id", "name", course.to_id);
            return View(course);
        }

        public ActionResult Search(DateTime date, String from, String to)
        {
            var courses = db.Courses
                .Where(c => c.date.Equals(date))
                .Where(c => c.from_id.Equals(from))
                .Where(c => c.to_id.Equals(to))
                .ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.OldDate = date.ToString("yyyy-MM-dd");
            ViewBag.OldFrom = from;
            ViewBag.OldTo = to;
            return View(courses);
        }



        // GET: Courses/Delete/5
        [Authorize]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(long id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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


        private List<Ticket> MakeTickets(Course course)
        {
            var tickets = new List<Ticket>();
            foreach(Seat seat in course.Bus.Seats)
            {
                var ticket = new Ticket()
                {
                    course_id = course.id,
                    seat_id = seat.id,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                tickets.Add(ticket);
            }
            return tickets;
        }
    }
}
