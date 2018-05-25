using MyanmarTravellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//Modified by Htut Arkar Oo
//For managing About and Contact pages
namespace MyanmarTravellers.Controllers
{
    public class HomeController : Controller
    {

        private MMTravellersEntities db = new MMTravellersEntities();

        public ActionResult Index()
        {
            ViewBag.Locations = db.Locations.ToList();
            return View();
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