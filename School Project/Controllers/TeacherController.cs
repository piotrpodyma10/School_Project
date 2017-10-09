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
                             Nazwa_Klasy = table1.Nazwa,


                         }).Distinct().ToList();


            return View("~/Views/Teacher/Grades_View.cshtml", "~/Views/Main_Layout.cshtml", Klasy);
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

       


    }

    }
