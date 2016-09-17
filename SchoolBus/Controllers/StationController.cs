using SchoolBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolBus.Controllers
{
    public class StationController : Controller
    {
        //
        // GET: /Station/

        public ActionResult Index()
        {
            
            List<Station> stations = new List<Station>();
            using (var dbct = new UsersContext())
            {
                ViewBag.Wards = dbct.Wards.ToList();
                stations = dbct.Stations.Where(x=>x.WardId==9).ToList();
            }
            return View(stations);
        }

        public ActionResult ChooseWard(int Id)
        {
            List<Station> stations = new List<Station>();
            using (var dbct = new UsersContext())
            {
                ViewBag.Wards = dbct.Wards.ToList();
                stations = dbct.Stations.Where(x => x.WardId == Id).ToList();
            }
            return View(stations);
        }

        public ActionResult ShowMap()
        {
            List<Station> stations = new List<Station>();
            using (var dbct = new UsersContext())
            {
                ViewBag.Wards = dbct.Wards.ToList();
                stations = dbct.Stations.Where(x => x.WardId == 9).ToList();
            }
            foreach (var i in stations)
            {
                i.Ward = null;
            }
            return View(stations);
        }

        [HttpPost]
        public ActionResult ShowMap(int Id)
        {
            List<Station> stations = new List<Station>();
            using (var dbct = new UsersContext())
            {
                ViewBag.Wards = dbct.Wards.ToList();
                stations = dbct.Stations.Where(x => x.WardId == Id).ToList();
            }
            foreach (var i in stations)
            {
                i.Ward = null;
            }
            return View(stations);
        }
    }
}
