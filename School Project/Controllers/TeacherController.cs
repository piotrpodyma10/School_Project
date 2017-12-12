using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using School_Project.Models;

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
            Log.metoda(out i, out n);
            tekst = "Witaj " + i + " " + n;

            ViewBag.powitanie = tekst;

            int ID;
            Log.wyciaganie_ID(out ID);

            SzkolaEntities db = new SzkolaEntities();
            Grades Grades = new Grades();

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








            return View("Index", "~/Views/Main_Layout.cshtml", Zmienna);

        }


        [Authorize]
        public ActionResult Home()
        {
            return View("Index");
        }



        public JsonResult GetEvents()
        {

            using (SzkolaEntities School = new SzkolaEntities())
            {
                School.Configuration.LazyLoadingEnabled = false;


                LoginController Log = new LoginController();
                int ID;
                Log.wyciaganie_ID(out ID);

                var result = (from n in School.Teacher
                              where n.UserID == ID
                              select n.ClassID).Distinct().ToList();

                //var result2 = Convert.ToInt32(result);



                var events = School.Events.ToList().Where(xx => result.Contains(xx.ClassID));

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














        [Authorize]
        public ActionResult Grades()
        {
            int User;
            LoginController Log = new LoginController();
            Log.wyciaganie_ID(out User);


            if (TempData.ContainsKey("Informacja"))
            {
                ViewBag.Informacja = TempData["Informacja"];
            }
            else if (TempData.ContainsKey("Brak_Danych"))
            {
                ViewBag.Informacja = TempData["Brak_Danych"];
            }
            else if ((TempData.ContainsKey("Brak_Danych")) && TempData.ContainsKey("Informacja"))
            {
                ViewBag.Informacja = TempData["Informacja"];
            }



            SzkolaEntities db = new SzkolaEntities();
            Grades Grades = new Grades();

            var Zmienna = (from table1 in db.Teacher
                           join table2 in db.School_Class on table1.ClassID equals table2.ClassID
                           where
                           User == table1.UserID
                           select new Grades {
                               ClassID = table2.ClassID,
                               Nazwa_Klasy = table2.Nazwa,
                               TeacherID = table1.TeacherID,
                               Przedmiot = table1.Przedmiot

                           }).Distinct().ToList();


            return View("~/Views/Teacher/Grades.cshtml", "~/Views/Main_Layout.cshtml", Zmienna);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Class_Number(FormCollection fc)
        {

            string Pobrane_Dane = fc["ClassID_TeacherID"];
            ViewBag.ClassID = Pobrane_Dane;

            var Przekazana_Wartosc = Pobrane_Dane.Split(' ');
            var ClassID = Convert.ToInt32(Przekazana_Wartosc[0]);
            var TeacherID = Convert.ToInt32(Przekazana_Wartosc[1]);

            ViewBag.TeacherID = TeacherID;

            SzkolaEntities db = new SzkolaEntities();
            Grades Grades = new Grades();

            var Student_Grades = (from table1 in db.User
                                  join table2 in db.Student on table1.UserID equals table2.UserID
                                  where
                                  table2.ClassID == ClassID
                                  select new Grades
                                  {
                                      Imie = table1.Imie,
                                      Nazwisko = table1.Nazwisko,
                                      StudentID = table2.StudentID,




                                  }).ToList();






            return View("Class_Number", "~/Views/Main_Layout.cshtml", Student_Grades);

        }


        [Authorize]
        public ActionResult Come_Back()
        {
            return RedirectToAction("Grades");
        }

        [Authorize]
        public ActionResult Come_Back_Grades()
        {
            return RedirectToAction("Grades_View");
        }



        [HttpPost]
        //public ActionResult Save_Data(Grades model)
        public ActionResult Save_Data(FormCollection fc)
        {
            try
            {
                SzkolaEntities db = new SzkolaEntities();
                School_Grades Grades = new School_Grades();

                string Forma_Zaliczenia4 = fc["Uwagi"];
                string[] Forma_Zaliczenia = Forma_Zaliczenia4.Split(',');
                string Ocena4 = fc["Ocena"];
                string[] Ocena = Ocena4.Split(',');
                string Student4 = fc["StudentID"];
                string[] Student = Student4.Split(',');
                string TeacherID = fc["TeacherID"];

                //float Grade;
                //double Grade4;
                //double Ocena = Convert.ToDouble(fc["Ocena"]);

                for (int i = 0; i < Forma_Zaliczenia.Length; i++)
                {



                    if ((Forma_Zaliczenia[i] == "X" || Forma_Zaliczenia[i] == null || Ocena[i] == "X"))
                    {

                        TempData["Brak_Danych"] = "Niestety nie udało się Tobie wprowadzić żadnych ocen!";

                        continue;
                        //Grade = float.Parse(Ocena[i]);
                        //Grade4 = double.Parse(Ocena[i]);

                    }
                    else
                    {
                        Grades.Uwagi = Forma_Zaliczenia[i];
                        Grades.StudentID = Convert.ToInt32(Student[i]);

                        Grades.Ocena = Convert.ToDouble(Ocena[i].Replace(".", ","));


                        Grades.TeacherID = Convert.ToInt32(TeacherID);
                        Grades.Data = DateTime.Now;

                        db.School_Grades.Add(Grades);
                        db.SaveChanges();


                        TempData["Informacja"] = "Wszystkie oceny zostały prawidłowo zapisane!";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Come_Back");
        }



        public ActionResult Grades_View()
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
            List<double> Ls_Polski = new List<double>();
            List<double> Ls_Chemia = new List<double>();


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
                    else if (item.Trim() == "Język Polski")
                    {
                        foreach (var item2 in Klasa.Where(xx => xx.Przedmiot.Trim() == "Język Polski"))
                        {

                            suma = item2.Ocena + suma;
                            ilosc = ilosc + 1;



                        }
                        wynik = Math.Round(suma / ilosc, 2);
                        Ls_Polski.Add(wynik);
                    }
                    else if (item.Trim() == "Chemia")
                    {
                        foreach (var item2 in Klasa.Where(xx => xx.Przedmiot.Trim() == "Chemia"))
                        {

                            suma = item2.Ocena + suma;
                            ilosc = ilosc + 1;



                        }
                        wynik = Math.Round(suma / ilosc, 2);
                        Ls_Chemia.Add(wynik);
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

                if (Ls_Polski.Count != count)
                {
                    Ls_Polski.Add(wynik2);
                }

                if (Ls_Chemia.Count != count)
                {
                    Ls_Chemia.Add(wynik2);
                }


            }

            ViewBag.Lista = Ls_Matematyka;
            ViewBag.Lista2 = Ls_Angielski;

            ViewBag.Lista3 = Ls_Polski;
            ViewBag.Lista4 = Ls_Chemia;

            List<string> Ls_Matma = new List<string>();
            foreach (var test in ViewBag.Lista)
            {
                var test2 = test.ToString().Replace(",", ".");
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




            List<string> Ls_Pol = new List<string>();
            foreach (var test in ViewBag.Lista3)
            {
                var test2 = test.ToString().Replace(",", ".");
                Ls_Pol.Add(test2);
            }

            var Pol = string.Join(",", Ls_Pol);
            ViewBag.Pol = Pol;



            List<string> Ls_Chem = new List<string>();
            foreach (var test in ViewBag.Lista4)
            {
                var test2 = test.ToString().Replace(",", ".");
                Ls_Chem.Add(test2);
            }

            var Chem = string.Join(",", Ls_Chem);
            ViewBag.Chem = Chem;






            var Klasy_II = (from table1 in db.School_Class
                            select new Grades
                            {
                                ClassID = table1.ClassID,
                                Nazwa_Klasy = table1.Nazwa,


                            }).Distinct().ToList();

            ViewBag.Klasy_II = Klasy_II;


            return View("~/Views/Teacher/Grades_View.cshtml", "~/Views/Main_Layout.cshtml", Klasy_II);
        }


        [HttpPost]
        public ActionResult Grades_View_I(FormCollection fc)
        {
            string ClassID = fc["ClassID"];
            int ID_Class = Convert.ToInt32(ClassID);


            Session["ClassID"] = ID_Class;
            //TempData["ClassID"] = ID_Class;

            return RedirectToAction("Grades_View_II");
        }




        public ActionResult Grades_View_II()
        {

            //string ClassID = fc["ClassID"];
            //int ID_Class = Convert.ToInt32(ClassID);
            int ID_Class = Convert.ToInt32(Session["ClassID"]);

            var Przedmiot = "Matematyka";

            if (TempData.ContainsKey("Przedmiot"))
            {

                Przedmiot = TempData["Przedmiot"].ToString().Trim();

            }
            else
            {
                ///*TempData*/["Pusto"] = "Matematyka";
                Przedmiot = "Matematyka";
            }

            SzkolaEntities db = new SzkolaEntities();
            School_Grades Grades = new School_Grades();




            Data Dane = new Data();

            var Zaciaganie = (from table1 in db.User
                              join table2 in db.Student on table1.UserID equals table2.UserID
                              where
                              table2.ClassID == ID_Class
                              select new Data
                              {
                                  Imie = table1.Imie,
                                  Nazwisko = table1.Nazwisko,
                                  StudentID = table2.StudentID

                              }).ToList();

            DataGradesModel DGM = new DataGradesModel();
            ViewBag.Zaciaganie = Zaciaganie;



            var Pobrane = (from table2 in db.Student
                           join table3 in db.School_Grades on table2.StudentID equals table3.StudentID
                           join table4 in db.Teacher on table3.TeacherID equals table4.TeacherID
                           where
                           table2.ClassID == ID_Class &&
                           table4.Przedmiot == Przedmiot

                           select new Grades
                           {

                               Przedmiot = table4.Przedmiot,
                               Ocena = table3.Ocena,
                               Uwagi = table3.Uwagi,
                               StudentID2 = table2.StudentID,
                               Data = table3.Data

                           }).ToList();

            ViewBag.Pobrane = Pobrane;


            double suma = 0;
            int ilosc = 0;
            double wynik;

            foreach (var item in Pobrane)
            {

                suma = suma + item.Ocena;
                ilosc = ilosc + 1;


            }
            if (ilosc == 0)
            {
                ViewBag.Wynik50 = "Brak Ocen";
            }
            else
            {
                wynik = Math.Round(suma / ilosc, 2);
                ViewBag.Wynik50 = " Średnia całej klasy wynosi: " + wynik;
            }

            var Przedmioty = (from table1 in db.Teacher
                              select new Grades
                              {
                                  przedmiot2 = table1.Przedmiot
                              }).Distinct().ToList();
            ViewBag.Przedmioty = Przedmioty;


            return View("~/Views/Teacher/Grades_View_II.cshtml", "~/Views/Main_Layout.cshtml", Pobrane);
        }



        [HttpPost]
        public ActionResult GetPrzedmiot(FormCollection fc1)
        {

            string Przedmiot = fc1["Przedmiot"];
            TempData["Przedmiot"] = Przedmiot;

            Session["ClassID"] = Session["ClassID"];


            return RedirectToAction("Grades_View_II");

        }



        [Authorize]
        public ActionResult Rank_View()
        {

            
            SzkolaEntities db = new SzkolaEntities();
            Grades Grades = new Grades();


            var Ranking = (from table1 in db.School_Class
                           join table2 in db.Student on table1.ClassID equals table2.ClassID
                           join table3 in db.School_Grades on table2.StudentID equals table3.StudentID
                           join table4 in db.Teacher on table3.TeacherID equals table4.TeacherID
                           join table5 in db.User on table2.UserID equals table5.UserID
                           group new { table5, table3, table1 } by new { table5.Imie, table5.Nazwisko, table1.Nazwa }
                           into grp
                           orderby grp.Average(x => x.table3.Ocena) descending
                           select new Grades
                           {
                               Imie = grp.Key.Imie,
                             Nazwisko = grp.Key.Nazwisko,
                             Ocena = Math.Round(grp.Average(x => x.table3.Ocena), 2),
                             Nazwa_Klasy = grp.Key.Nazwa


                            

        }).Take(10);


            ViewBag.Ranking = Ranking;

            List<string> Przedmioty = new List<string>(new string[] { "Matematyka", "Język Polski", "Język Angielski", "Chemia" });

            foreach (var Przedmiot in Przedmioty)
            {

                var SmallRanking = (from table1 in db.School_Class
                               join table2 in db.Student on table1.ClassID equals table2.ClassID
                               join table3 in db.School_Grades on table2.StudentID equals table3.StudentID
                               join table4 in db.Teacher on table3.TeacherID equals table4.TeacherID
                               join table5 in db.User on table2.UserID equals table5.UserID
                               where table4.Przedmiot.Trim() == Przedmiot
                               group new { table5, table3, table1 } by new { table5.Imie, table5.Nazwisko, table1.Nazwa }
                         into grp
                               orderby grp.Average(x => x.table3.Ocena) descending
                               select new Grades
                               {
                                   Imie = grp.Key.Imie,
                                   Nazwisko = grp.Key.Nazwisko,
                                   Ocena = Math.Round(grp.Average(x => x.table3.Ocena),2),
                                   Nazwa_Klasy = grp.Key.Nazwa



                               }).Take(3);


                if (Przedmiot == "Matematyka")
                {
                    ViewBag.Matematyka = SmallRanking;
                }
                else if (Przedmiot == "Język Polski")
                {
                    ViewBag.Polski = SmallRanking;
                }
                else if (Przedmiot == "Język Angielski")
                {
                    ViewBag.Angielski = SmallRanking;
                }
                else if (Przedmiot == "Chemia")
                {
                    ViewBag.Chemia = SmallRanking;
                }




            }


            return View("~/Views/Teacher/Rank_View.cshtml", "~/Views/Main_Layout.cshtml");
        }


    }

    }
