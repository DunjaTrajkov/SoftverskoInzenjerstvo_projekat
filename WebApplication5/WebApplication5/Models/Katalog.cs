using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class Katalog
    {
        public Katalog()
        {
            DSadrzi = new List<DSadrzi>();
            KSadrzi = new List<KSadrzi>();
            NSadrzi = new List<NSadrzi>();
        }

        [Key]
        public int Idkatalog { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Naziv kataloga je obavezno polje.")]
        [Display(Name = "Naziv kataloga.")]
        [StringLength(40, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Naziv { get; set; }

        [Range(0,2)]
        public int Kreirao { get; set; }

        public virtual ICollection<DSadrzi> DSadrzi { get; set; }
        public virtual ICollection<KSadrzi> KSadrzi { get; set; }
        public virtual ICollection<NSadrzi> NSadrzi { get; set; }
    }
}
