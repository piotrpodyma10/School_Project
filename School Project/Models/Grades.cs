using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace School_Project.Models
{
    public class Grades
    {
        // User Table

        public string Imie { get; set; }
        public string Nazwisko { get; set; }


        // School_Class Table

        
        public string Nazwa_Klasy { get; set; }


        // Teacher Table

        public int TeacherID { get; set; }
        public string Przedmiot { get; set; }

        // School_Grades

        public int GradeID { get; set; }
        public double Ocena { get; set; }
        
        
        public string Uwagi { get; set; }
        public System.DateTime Data { get; set; }

        //Student

        public int StudentID { get; set; }
        public int UserID { get; set; }
        public int ClassID { get; set; }

        // Dodatkowe pola - Testowanie
        public string Imie2 { get; set; }
        public string Nazwisko2 { get; set; }

        public int StudentID2 { get; set; }
        public string przedmiot2 { get; set; }



    }
}