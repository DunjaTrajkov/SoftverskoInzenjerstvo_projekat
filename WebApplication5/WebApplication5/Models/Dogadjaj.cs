using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class Dogadjaj
    {
        public Dogadjaj()
        {
            DSadrzi = new List<DSadrzi>();
        }
        [Key]
        public int Iddogadjaja { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vrsta dogadjaja je obavezno polje.")]
        [StringLength(30, MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string VrstaDogadjaja { get; set; }

        public virtual ICollection<DSadrzi> DSadrzi { get; set; }
    }
}
