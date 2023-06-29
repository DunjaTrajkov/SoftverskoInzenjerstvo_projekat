using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class Narudzbina
    {
        public Narudzbina()
        {
            NSadrzi = new List<NSadrzi>();
        }
        [Key]
        public int Idnarudzbine { get; set; }
        
        [Display(Name = "Datum narucivanja")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Datum narucivanja je obavezno polje.")]
        public DateTime DatumNarucivanja { get; set; }
        [Display(Name = "Datum isporuke")]
        
        [DataType(DataType.Date)]
        public DateTime DatumIsporuke { get; set; }
        [Display(Name = "Vreme isporuke")]
        [DataType(DataType.Time)]
        public TimeSpan VremeIsporuke { get; set; }
        [Display(Name = "Adresa isporuke")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Adresa isporuke je obavezno polje")]
        public string AdresaIsporuke { get; set; }
        [Display(Name = "Ukupna cena")]
        [Range(0,999999)]
        public decimal UkupnaCena { get; set; }

        //0 kreirana, 1 porucena,2 prihvacena od strane menadzera,  3 da je isporucena, 4 odbijena
        [Range(0,6)]
        public int StatusNarudzbine { get; set; } 


        public int Idradnika { get; set; }
        public int Idkorisnika { get; set; }

        public virtual Korisnik IdkorisnikaNavigation { get; set; }
        public virtual Radnik IdradnikaNavigation { get; set; }
        public virtual ICollection<NSadrzi> NSadrzi { get; set; }
    }
}
