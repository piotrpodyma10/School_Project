using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using School_Project.Models;

namespace School_Project.Controllers
{
    public class StudentController : Controller
    {


        [Authorize]
        public ActionResult Index_Student(string wiadomosc)
        {

            LoginController Log = new LoginController();
            string imie, nazwisko, tekst;
            Log.metoda(out imie, out nazwisko);
            tekst = "Witaj " + imie + " " + nazwisko;


            ViewBag.powitanie = tekst;



            return View("Index_Student", "~/Views/Main_Layout.cshtml");
        }


        public ActionResult Student_Grades()
        {

            int UserID;
            LoginController Log = new LoginController();
            Log.wyciaganie_ID(out UserID);

            

            SzkolaEntities db = new SzkolaEntities();
            School_Grades Grades = new School_Grades();

            var Pobrane = (from table2 in db.Student
                           join table3 in db.School_Grades on table2.StudentID equals table3.StudentID
                           join table4 in db.Teacher on table3.TeacherID equals table4.TeacherID
                           join table5 in db.School_Class on table2.ClassID equals table5.ClassID
                           where
                           table2.UserID == UserID
                           select new Grades
                           {
                               ClassID = table2.ClassID,
                               Nazwa_Klasy = table5.Nazwa,
                               Przedmiot = table4.Przedmiot,
                               Ocena = table3.Ocena,
                               Uwagi = table3.Uwagi,
                               Data = table3.Data,
                               TeacherID = table4.TeacherID

                           }).ToList();

            var ID = Pobrane.FirstOrDefault().ClassID;

            var Przedmioty = (from table1 in db.Teacher
                              join table2 in db.User 
                              on table1.UserID equals table2.UserID
                              where table1.ClassID == ID
                              
                              select new Grades
                              {

                                  Imie2 = table2.Imie,
                                  Nazwisko2 = table2.Nazwisko,
                                  przedmiot2 = table1.Przedmiot
                              }).Distinct().ToList();


            ViewBag.Przedmioty = Przedmioty;





            return View("Student_Grades", "~/Views/Main_Layout.cshtml", Pobrane);
        }


        public ActionResult Student_Come_Back()
        {
            return RedirectToAction("Index_Student");
        }



    }
}