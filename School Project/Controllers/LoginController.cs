using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using School_Project.Models;
using System.Web.Security;


namespace School_Project.Controllers
{
    public class LoginController : Controller
    {

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Auth(FormCollection fc)
        {
            string UserName = fc["UserName"];
            string Password = fc["Password"];


            SzkolaEntities db = new SzkolaEntities();
            var adm = (from ad in db.User
                       where
                            ad.UserName == UserName &&
                            ad.Password == Password
                       select ad).FirstOrDefault();

            if (adm == null)
            {
                ViewBag.Error = "Wprowadziłeś zły login lub hasło";
                return View("index");
            }
            else
            {
                //Trim - powoduje usuwanie spacji / blank signów
                if (adm.Rodzaj.Trim() == "Student")
                {
                    Session["Name"] = adm.UserName;
                    FormsAuthentication.SetAuthCookie(adm.UserName, false);
                    var text = "Witaj " + adm.UserName + "!";

                    return RedirectToAction("index", "Home", new { wiadomosc = text });
                }

                if (adm.Rodzaj.Trim() == "Nauczyciel")
                {
                    Session["Name"] = adm.UserName;
                    FormsAuthentication.SetAuthCookie(adm.UserName, false);
             
                    imie1 = adm.Imie.Trim();
                    nazw1 = adm.Nazwisko.Trim();
                    rodzaj = adm.Rodzaj.Trim();
                    ID = adm.UserID;


                    //ViewData["MyProduct"] = dane;
                    //, new { imie = adm.Imie, nazwisko = adm.Nazwisko }

                    //, new Models.User { ImieM = adm.Imie }

                    return RedirectToAction("index", "Teacher");
                }

                else
                {
                    return RedirectToAction("index");
                }
            }
        }


        public static string rodzaj { get; set; }
        public static string imie1 { get; set; }
        public static string nazw1 { get; set; }
        public static int ID { get; set; }

        public void metoda(out string imie, out string nazw)
        {
            imie = imie1;
            nazw = nazw1;
        }

        public ActionResult Refresh()
        {
            var tekst = "";
            tekst = rodzaj;
            

            return RedirectToAction("index", "Teacher");
        }
        
        [Authorize]
        public ActionResult Personal_Data()
        {
            


            SzkolaEntities db = new SzkolaEntities();
            //var dane = (from table in db.Personal_Data
            //               join table2 in db.User on table.UserID equals table2.UserID
            //               where
            //               ID == table2.UserID
            //               select new Personal_Data
            //               {
            //                   Imie = table2.Imie,
            //                   Nazwisko = table2.Nazwisko,
            //                   Rodzaj = table2.Rodzaj,
            //                   Pesel = table.Pesel,
            //                   Telefon = table.Telefon,
            //                   Miasto = table.Miasto,
            //                   Adres = table.Adres,
            //                   Data_Urodzenia = table.Data_Urodzenia,
            //                   Email = table.Email
            //               }).ToList();


            List<Personal_Data> UserData = db.Personal_Data.ToList();
            Data Data = new Data();

            List<Data> UserDataList = UserData.Where(emp => emp.UserID == ID).Select(x => new Data
            {
                Imie = x.User.Imie,
                Nazwisko = x.User.Nazwisko,
                Pesel = x.Pesel,
                Miasto = x.Miasto,
                Adres = x.Adres,
                Email = x.Email,
                Data_Urodzenia = x.Data_Urodzenia,
                Telefon = x.Telefon,
                Rodzaj = x.User.Rodzaj
                
            }).ToList();




            return View("~/Views/Personal_Data/Personal_Data.cshtml", "~/Views/Main_Layout.cshtml", UserDataList);
        }



        public ActionResult Logout()
        {
            Session["Name"] = "";
            FormsAuthentication.SignOut();
            return RedirectToAction("index");
        }


        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(FormCollection fc)
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

    }
}