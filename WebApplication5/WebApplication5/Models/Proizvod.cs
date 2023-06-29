using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class Proizvod
    {
        public Proizvod()
        {
            KSadrzi = new List<KSadrzi>();
        }
        [Key]
        public int  Idproizvoda { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Naziv proizvoda je obavezno polje")]
        [Display(Name = "Naziv proizvoda")]
        [StringLength(30, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Naziv { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Opis proizvoda je obavezno polje")]
        [Display(Name = "Opis proizvoda")]
        [StringLength(100, MinimumLength = 5)]
        [DataType(DataType.Text)]
        public string Opis { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tip je obavezno polje ")] //0-hrana, 1-dekoracija
        [Range(0, 2)]
        public int Tip { get; set; }
       // [Required(AllowEmptyStrings = false, ErrorMessage = "Naziv proizvoda je obavezno polje")]
        [Display(Name = "Vrsta dekoracije")]
        [StringLength(30, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string VrstaDekoracije { get; set; }
     //   [Required(AllowEmptyStrings = false, ErrorMessage = "Boja je obavezno polje")]
        [Display(Name = "Boja dekoracije")]
        [StringLength(30, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Boja { get; set; }
        [Range(0, 99999)]
        public decimal? CenaDekoracije { get; set; }
       // [Required(AllowEmptyStrings = false, ErrorMessage = "Vrsta obroka je obavezno polje")]
        [Display(Name = "Vrsta obroka")]
        [StringLength(30, MinimumLength = 3)]
        [DataType(DataType.Text)]
        public string VrstaObroka { get; set; }
        [Range(0, 10000)]
        public decimal? Gramaza { get; set; }
        public decimal? CenaPorcije { get; set; }
        [Range(0, 10000)]
        public string PutanjaDoSlike { get; set; }

        public virtual ICollection<KSadrzi> KSadrzi { get; set; }
    }
}
