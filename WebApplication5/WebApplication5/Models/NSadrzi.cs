using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication5.Models
{
    [BindProperties(SupportsGet = true)]
    public  class NSadrzi
    {
        public int Idnarudzbine { get; set; }
        public int Idkatalog { get; set; }
        public int Kolicina { get; set; }

        public virtual Katalog IdkatalogNavigation { get; set; }
        public virtual Narudzbina IdnarudzbineNavigation { get; set; }
    }
}
