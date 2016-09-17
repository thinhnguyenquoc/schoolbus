using SchoolBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolBus.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                MyMap mymap = new MyMap();
                using (var dbct = new UsersContext())
                {
                    mymap.studentaddresses = dbct.StudentAddresses.ToList();
                    foreach (var add in mymap.studentaddresses)
                    {
                        add.Ward = null;
                    }
                    mymap.wards = dbct.Wards.ToList();
                    foreach (var ward in mymap.wards)
                    {
                        ward.StudentAddresses = null;
                    }
                    mymap.districts = dbct.Districts.ToList();
                    foreach (var district in mymap.districts)
                    {
                        district.Wards = null;
                    }
                    mymap.schools = dbct.SchoolProfiles.ToList();
                    foreach (var school in mymap.schools)
                    {
                        school.Student = null;
                    }
                    return View(mymap);
                }
            }
            catch (Exception e)
            {

            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
