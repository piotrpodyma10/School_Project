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
        public ActionResult Index()
        {

            SzkolaEntities db = new SzkolaEntities();
            Grades Grades = new Grades();

            var Klasy = (from table1 in db.School_Class
                         select new Grades
                         {
                             ClassID = table1.ClassID,
                             Nazwa_Klasy = table1.Nazwa
                         });

            ViewBag.Klasy = Klasy;

            var Oceny = (from table1 in db.School_Class
                         join table2 in db.Student on table1.ClassID equals table2.ClassID
                         join table3 in db.School_Grades on table2.StudentID equals table3.StudentID
                         join table4 in db.Teacher on table3.TeacherID equals table4.TeacherID
                         select new Grades
                         {
                             Nazwa_Klasy = table1.Nazwa,
                             ClassID = table1.ClassID,
                             Ocena = table3.Ocena,
                             Przedmiot = table4.Przedmiot                     
                            
                         });

            var Zmienna_Klasy = Klasy.ToList();
            

            List<double> Ls_Matematyka = new List<double>();
            List<double> Ls_Angielski = new List<double>();
            
            int count = 0;

            for (var i = 1; i < Zmienna_Klasy.Count + 1; i++)
            {

                var Klasa = Oceny.Where(xx => xx.ClassID == i);

                count++;


                foreach (var item in Klasa.Select(xx => xx.Przedmiot).Distinct())
                {

                   double suma = 0;
                   int ilosc = 0;
                   double wynik = 0;


                    if (item.Trim() == "Matematyka")
                    {

                        foreach (var item2 in Klasa.Where(xx => xx.Przedmiot.Trim() == "Matematyka"))
                        {

                            suma = item2.Ocena + suma;
                            ilosc = ilosc + 1;

                            

                        }
                        wynik = Math.Round(suma / ilosc, 2);
                        Ls_Matematyka.Add(wynik);


                    }
                    else if (item.Trim() == "Język Angielski")
                    {
                        foreach (var item2 in Klasa.Where(xx => xx.Przedmiot.Trim() == "Język Angielski"))
                        {

                            suma = item2.Ocena + suma;
                            ilosc = ilosc + 1;

                            

                        }
                        wynik = Math.Round(suma / ilosc, 2);
                        Ls_Angielski.Add(wynik);
                    }
                    else
                    {
                        continue;
                    }

                }
                var wynik2 = 0;

                if (Ls_Matematyka.Count != count)
                {
                    Ls_Matematyka.Add(wynik2);
                }


                if (Ls_Angielski.Count != count)
                {
                    Ls_Angielski.Add(wynik2);
                }
                
            }

            ViewBag.Lista = Ls_Matematyka;
            ViewBag.Lista2 = Ls_Angielski;


            List<string> Ls_Matma = new List<string>();
            foreach(var test in ViewBag.Lista)
            {
                var test2 = test.ToString().Replace(",",".");
                Ls_Matma.Add(test2);
            }

            var Matma = string.Join(",", Ls_Matma);
            ViewBag.Matma = Matma;


            List<string> Ls_Ang = new List<string>();
            foreach (var test in ViewBag.Lista2)
            {
                var test2 = test.ToString().Replace(",", ".");
                Ls_Ang.Add(test2);
            }

            var Ang = string.Join(",", Ls_Ang);
            ViewBag.Ang = Ang;





           




            return View(Oceny);
        }


        public ActionResult New()
        {

            SzkolaEntities db = new SzkolaEntities();
            Grades Grades = new Grades();
            int ID = 4;
            var Zmienna = (from table0 in db.User
                           join table1 in db.Teacher on table0.UserID equals table1.UserID
                           join table2 in db.School_Class on table1.ClassID equals table2.ClassID
                           where
                           table0.UserID == ID
                           select new Grades
                           {
                               ClassID = table2.ClassID,
                               Nazwa_Klasy = table2.Nazwa


                           }).Distinct().ToList();

            return View(Zmienna);
        }


        public JsonResult GetEvents()
        {

            using (SzkolaEntities School = new SzkolaEntities())
            {
                School.Configuration.LazyLoadingEnabled = false;

                var events = School.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            
        }





        [HttpPost]
        public JsonResult SaveEvent(Models.Events e)
        {
            var status = false;
            using (SzkolaEntities School = new SzkolaEntities())
            {
                if (e.EventID > 0)
                {
                    //Update the event
                    var v = School.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                        v.ClassID = e.ClassID;
                    }
                }
                else
                {
                    //var date = e.Start;

                    //DateTime date2 = DateTime.Parse(date.ToString("yyyy-dd-MM"));
                    ////var date2 = date.ToString("yyyy-dd-MM");

                    ////DateTime date2 = DateTime.Parse(date);
                    


                    //var end_date = e.End;
                    //if (end_date == null) { end_date = null; }
                    //else {
                    //    DateTime end_date2 = DateTime.Parse(end_date.Value.ToString("yyyy-dd-MM"));
                    //}


                    ////var date3 = date.ToString("yyyy-dd-MM");
                    ////DateTime nowadata = DateTime.Parse(date3);
                    ////e.Start = date2;
                    //e.End = end_date;

          
                    School.Events.Add(e);
                }

                School.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (SzkolaEntities School = new SzkolaEntities())
            {
                var v = School.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    School.Events.Remove(v);
                    School.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }




    }
}