using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class Korisnik
    {
        public Korisnik()
        {
            Narudzbina = new List<Narudzbina>();
        }
        [Key]
        public int Idkorisnika { get; set; }
        //[Required( ErrorMessage = "Ime je obavezno polje.")]
        [StringLength(60, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Ime { get; set; }
        [StringLength(50, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Prezime { get; set; }
       // [Required(ErrorMessage = "E-mail je obavezno polje.")]
        [StringLength(50, MinimumLength = 4)]
        [DataType(DataType.Text)]

        public string Email { get; set; }

        [Required(ErrorMessage = "Username je obavezno polje")]
        [StringLength(50, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Required( ErrorMessage = "Sifra je obavezno polje")]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Minimum 6 karaktera")]
        [DataType(DataType.Password)]
        public string Sifra { get; set; }

        //[Required(ErrorMessage = "Moras potvrditi sifru")]
        [NotMapped]
        [DataType(DataType.Password)]
        public string PotvrdiSifru { get; set; }


        [RegularExpression(@"^(\d{10})$",ErrorMessage = "Broj telefona je obavezno polje")]
        [StringLength(15, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string BrojTelefona { get; set; }
        //[Required(ErrorMessage = "Tip je  je obavezno polje.")]
        [StringLength(30, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Tip { get; set; }

        public virtual ICollection<Narudzbina> Narudzbina { get; set; }
    }
}
