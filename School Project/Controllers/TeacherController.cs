using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace School_Project.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        
        [Authorize]
        public ActionResult Index()
        {
            LoginController Log = new LoginController();
            string i, n, tekst;
            Log.metoda(out i,out n);
            tekst = "Witaj " + i + " " + n;

            //powitanie = "Witaj " + imie + " " + nazwisko;
            //ViewBag.imie = imie;
            //ViewBag.nazwisko = nazwisko;
            ViewBag.powitanie = tekst;

            return View("Index", "~/Views/Main_Layout.cshtml");
        }


       

        

        [Authorize]
        public ActionResult Home()
        {
            return View("Index");
        }

    }
}