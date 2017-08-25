using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using School_Project.Models;

namespace School_Project.Controllers
{
    public class HomeController : Controller
    {

        

        // GET: Home
        [Authorize]
        public ActionResult Index(string wiadomosc)
        {
            ViewBag.message = wiadomosc;
            ViewBag.Text = "Siemano";
            return View();
        }

      


    }
}