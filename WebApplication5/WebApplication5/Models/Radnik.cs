using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class Radnik
    {
        public Radnik()
        {
            Narudzbina = new List<Narudzbina>();
        }
        [Key]
        public int Idradnika { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ime zaposlenog radnika je obavezno polje")]
        [Display(Name = "Ime radnika")]
        [StringLength(40, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Ime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Prezime zaposlenog radnika je obavezno polje")]
        [Display(Name = "Prezime radnika")]
        [StringLength(40, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Prezime { get; set; }
        public string Telefon { get; set; }
        public bool Slobodan { get; set; }

        public virtual ICollection<Narudzbina> Narudzbina { get; set; } //strani za narudzbina
    }
}
