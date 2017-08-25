using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace School_Project.Models
{
    public class Data
    {
       
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Rodzaj { get; set; }

        
        public decimal Pesel { get; set; }
        public Nullable<int> UserID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy)", ApplyFormatInEditMode =true)]
        public Nullable<System.DateTime > Data_Urodzenia { get; set; }


        



        public Nullable<decimal> Telefon { get; set; }
        public string Miasto { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }

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