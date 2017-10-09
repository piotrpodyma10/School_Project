using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace School_Project.Models
{
    public class Data
    {
       
        //User Table

        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Rodzaj { get; set; }
        public Nullable<int> UserID { get; set; }


        // Personal_Data Table
        
        public Nullable<decimal> Telefon { get; set; }
        public string Miasto { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public decimal Pesel { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy)", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Data_Urodzenia { get; set; }

        //Student Table

        public int StudentID { get; set; }
        //public int UserID { get; set; }
        public int ClassID { get; set; }


    }

    public static class Format
    {
        // Metoda do tworzenia nowego formatu daty z formatu nullable
        public static string ToShortDateString(this DateTime? date)
        {
            if (date == null) { return null; }
            return date.Value.ToString("dd/MM/yyyy");
        }

        
    }


}